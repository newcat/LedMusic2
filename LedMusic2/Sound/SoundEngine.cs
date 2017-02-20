using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace LedMusic2.Sound
{
    class SoundEngine : VMBase
    {

        #region Properties
        private ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();
        public ObservableCollection<MMDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                NotifyPropertyChanged();
            }
        }

        private int _volume = 50;
        public int Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (soundOut != null)
                    soundOut.Volume = Math.Min(1f, Math.Max(value / 100f, 0f));
                NotifyPropertyChanged();
            }
        }

        public PlaybackState PlaybackState
        {
            get
            {
                if (soundOut != null)
                    return soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public bool CanPlay
        {
            get
            {
                return soundOut != null && waveSource != null && !buildingWaveform &&
                  (PlaybackState == PlaybackState.Stopped || PlaybackState == PlaybackState.Paused);
            }
        }

        public bool CanPause
        {
            get
            {
                return soundOut != null && waveSource != null && PlaybackState == PlaybackState.Playing;
            }
        }

        public bool CanStop
        {
            get
            {
                return soundOut != null && waveSource != null;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (waveSource != null)
                    return waveSource.GetPosition();
                return TimeSpan.Zero;
            }
            set
            {
                if (waveSource != null && value.TotalMilliseconds >= 0 && value < Length)
                    waveSource.SetPosition(value);
                NotifyPropertyChanged();
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (waveSource != null)
                    return waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        private WaveformModel _waveformModel = new WaveformModel();
        public WaveformModel WaveformModel
        {
            get { return _waveformModel; }
            set
            {
                _waveformModel = value;
                NotifyPropertyChanged();
            }
        }

        private string _file = "";
        public string File
        {
            get { return _file; }
            private set
            {
                _file = value;
                NotifyPropertyChanged();
            }
        }

        public BeatDetector BeatDetector { get { return beatDetector; } }
        #endregion

        #region Constants
        private const int BLOCK_SIZE = 1000;
        public const FftSize FFT_SIZE = FftSize.Fft8192;
        #endregion

        #region Fields
        //Display
        private readonly DispatcherTimer positionTimer = new DispatcherTimer(DispatcherPriority.Render);

        //FFT
        private Thread fftThread;
        private object fftLockObject = new object();
        private FftProvider fftProvider = new FftProvider(2, FFT_SIZE);
        private float[] currentFftData = new float[(int)FFT_SIZE];

        //Waveform
        private bool buildingWaveform = false;

        //Sound out
        private ISoundOut soundOut;
        private IWaveSource waveSource;
        
        //Beat detection
        BeatDetector beatDetector = new BeatDetector();
        #endregion

        #region Constructor
        private static readonly SoundEngine _instance = new SoundEngine();
        public static SoundEngine Instance
        {
            get
            {
                return _instance;
            }
        }

        private SoundEngine()
        {
            loadDevices();

            positionTimer.Interval = TimeSpan.FromMilliseconds(1000 / 60d);
            positionTimer.Tick += PositionTimer_Tick;
            positionTimer.IsEnabled = true;
        }
        #endregion

        #region Public Methods
        public async Task<bool> OpenFile(string filename)
        {

            CleanupPlayback();

            File = filename;

            var src = CodecFactory.Instance.GetCodec(filename).ToSampleSource();
            var notificationSource = new SingleBlockNotificationStream(src);
            notificationSource.SingleBlockRead += (s, e) => fftProvider.Add(e.Left, e.Right);

            waveSource = notificationSource.ToWaveSource();

            if (!waveSource.CanSeek)
            {
                Debug.WriteLine("WaveSource doesnt support seeking.");
                return false;
            }

            soundOut = new WasapiOut(false, AudioClientShareMode.Shared, 10, System.Threading.ThreadPriority.AboveNormal);
            soundOut.Initialize(waveSource);
            soundOut.Stopped += SoundOut_Stopped;
            Volume = _volume;

            buildingWaveform = true;
            ISampleSource sampleBuildingSource = CodecFactory.Instance.GetCodec(filename).ToSampleSource().ToMono();
            await Task.Run(() => createSampleList(sampleBuildingSource));
            buildingWaveform = false;

            NotifyPropertyChanged("CanPlay");
            NotifyPropertyChanged("CanPause");
            NotifyPropertyChanged("CanStop");

            fftThread = new Thread(calculateFft);
            fftThread.Start();

            beatDetector.Detect(File);

            //To enable the play button
            CommandManager.InvalidateRequerySuggested();

            return true;

        }

        private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (e.HasError)
                Debug.WriteLine(e.Exception);
            Stop();
        }

        public void Play()
        {
            if (CanPlay)
            {
                soundOut.Play();
                NotifyPropertyChanged("CanPlay");
                NotifyPropertyChanged("CanPause");
                NotifyPropertyChanged("CanStop");
            }
        }

        public void Pause()
        {
            if (CanPause)
            {
                soundOut.Pause();
                NotifyPropertyChanged("CanPlay");
                NotifyPropertyChanged("CanPause");
                NotifyPropertyChanged("CanStop");
            }
        }

        public void Stop()
        {
            if (CanStop && soundOut.PlaybackState != PlaybackState.Stopped)
            {
                soundOut.Stop();
                waveSource.SetPosition(TimeSpan.Zero);
                NotifyPropertyChanged("Position");
                NotifyPropertyChanged("CanPlay");
                NotifyPropertyChanged("CanPause");
                NotifyPropertyChanged("CanStop");
            }
        }

        public float GetCurrentSample()
        {

            double currentTime = Position.TotalSeconds;
            if (currentTime <= 0)
                return 0f;

            int sampleNumber = (int)Math.Floor((currentTime * waveSource.WaveFormat.SampleRate) / BLOCK_SIZE);

            if (sampleNumber >= 0 && sampleNumber < WaveformModel.Samples.Length)
                return WaveformModel.Samples[sampleNumber];
            else
                return 0f;

        }

        public float[] GetCurrentFftData()
        {
            lock (fftLockObject)
            {
                return currentFftData;
            }
        }

        public int GetFftBandIndex(float frequency)
        {
            if (waveSource == null)
                return -1;

            int fftSize = (int)FFT_SIZE;
            double f = waveSource.WaveFormat.SampleRate / 2.0;
            return (int)((frequency / f) * (fftSize / 2));
        }

        public void CleanupPlayback()
        {
            if (fftThread != null)
                fftThread.Abort();

            if (soundOut != null)
            {
                soundOut.Dispose();
                soundOut = null;
            }

            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }
        #endregion

        #region Private Methods
        private void loadDevices()
        {
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                        Devices.Add(device);
                }
            }
        }

        private void createSampleList(ISampleSource sampleSource)
        {

            if (sampleSource == null)
                throw new ArgumentNullException("sampleSource");

            var newSamples = new float[sampleSource.Length / BLOCK_SIZE + 2];
            var buffer = new float[BLOCK_SIZE];
            float blockMaxValue;
            int x = 0;

            var progress = new ProgressViewModel("Building waveform");
            MainViewModel.Instance.AddProgress(progress);

            while (sampleSource.Read(buffer, 0, BLOCK_SIZE) > 0)
            {
                blockMaxValue = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (Math.Abs(buffer[i]) > blockMaxValue)
                        blockMaxValue = Math.Abs(buffer[i]);
                }
                newSamples[x] = blockMaxValue;

                x++;
                if (x % 500 == 0)
                {
                    WaveformModel.Samples = newSamples;
                }

                if (x % 250 == 0)
                {
                    progress.Progress = (int)(100 * ((double)sampleSource.Position / sampleSource.Length));
                }

            }

            WaveformModel.Samples = newSamples;
            MainViewModel.Instance.RemoveProgress(progress);

        }

        private void calculateFft()
        {
            try
            {
                while (true)
                {
                    lock (fftLockObject)
                    {
                        fftProvider.GetFftData(currentFftData);
                    }
                    Thread.Sleep(10);
                }
            } catch (ThreadAbortException)
            {
                Debug.WriteLine("FFT thread ended.");
            }            
        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            if (PlaybackState == PlaybackState.Playing)
                NotifyPropertyChanged("Position");
        }
        #endregion

    }
}
