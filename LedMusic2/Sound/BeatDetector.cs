using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LedMusic2.Sound
{
    class BeatDetector
    {

        private int sampleRate = 44100;
        private List<float> beats = new List<float>();

        public float[] AverageIntensity;

        public void Detect(string file)
        {
            Task.Run(() => _detect(file));
        }

        private void _detect(string file)
        {

            ProgressViewModel prg = new ProgressViewModel("Beat detection");
            prg.Progress = 0;
            MainViewModel.Instance.AddProgress(prg);

            ISampleSource sampleSrc = CodecFactory.Instance.GetCodec(file).ToSampleSource().ToStereo();
            sampleRate = sampleSrc.WaveFormat.SampleRate;
            long length = sampleSrc.Length;

            long totalFrames = (sampleSrc.Length / sampleRate) * GlobalProperties.Instance.FPS;
            AverageIntensity = new float[totalFrames];

            int energyBufferLength = sampleRate / 1024;
            float[] soundEnergyBuffer = new float[energyBufferLength];
            int startingIndex = 0;

            float[] sampleBuffer = new float[2048];

            while (sampleSrc.Read(sampleBuffer, 0, 2048) == 2048)
            {

                float instantEnergy = 0f;
                for (int i = 0; i < 2048; i += 2)
                {
                    instantEnergy += sampleBuffer[i] * sampleBuffer[i] + sampleBuffer[i + 1] * sampleBuffer[i + 1];
                }

                float averageEnergy = 0f;
                for (int i = 0; i < energyBufferLength; i++)
                {
                    averageEnergy += soundEnergyBuffer[i];
                }
                averageEnergy /= energyBufferLength;

                soundEnergyBuffer[startingIndex] = instantEnergy;
                startingIndex--;
                if (startingIndex < 0)
                    startingIndex += energyBufferLength;

                if (instantEnergy > 1.3 * averageEnergy)
                    beats.Add(1.0f * sampleSrc.Position / sampleRate);

                if (sampleSrc.Position % 5000 == 0)
                    prg.Progress = (int)((1.0f * sampleSrc.Position / length) * 100);

            }

            //filter beats that got recognized multiple times
            //this works for up to 180 BPM (0.3s between beats)
            for (int i = 0; i < beats.Count - 1; i++)
            {
                if (beats[i + 1] - beats[i] < 0.3)
                {
                    float x = (beats[i + 1] + beats[i]) / 2;
                    beats.RemoveAt(i + 1);
                    beats.RemoveAt(i);
                    beats.Add(x);
                    beats.Sort();
                    i = 0;
                }
            }

            ConcurrentDictionary<int, int> histogram = new ConcurrentDictionary<int, int>();
            Parallel.For(0, beats.Count, (i) =>
            {
                float delta = beats[i + 1] - beats[i];
                int calculatedBpm = (int)Math.Round((1 / delta) * 60);
                histogram.AddOrUpdate(calculatedBpm, 1, (x, y) => y++);
            });
            int bpm = histogram.OrderByDescending(x => x.Value).First().Key;

            prg.Name = "Matching BPM " + bpm;
            prg.Progress = 0;

            float bps = bpm / 60f;

            ConcurrentDictionary<int, int> fitting = new ConcurrentDictionary<int, int>();
            int progress = 0;
            Parallel.For(0, beats.Count, (i) => {
                //TODO: Continue here
                Interlocked.Add(ref progress, 1);
            });

            MainViewModel.Instance.RemoveProgress(prg);

        }

    }
}
