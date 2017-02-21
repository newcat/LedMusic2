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

        private int bpm;
        private float offset;

        //TODO
        private float maxIntensity;
        private float[] averageIntensity;

        public float GetIntensity(int frame)
        {
            return 0;
        }

        public float GetBeatValue(int frame, bool onlyUseDetected = false)
        {

            float time = frame * 1.0f / GlobalProperties.Instance.FPS;
            float bps = bpm / 60f;
            float spb = 1f / bps;

            if (onlyUseDetected)
            {
                return 0;
            } else
            {
                int lastBeatNumber = (int)Math.Floor(time * bps);
                float lastBeatTime = lastBeatNumber * spb;
                return 1f - (time - lastBeatTime) / spb;
            }

        }

        public bool IsBeat(int frame, bool onlyUseDetected = false)
        {
            return GetBeatValue(frame) > GetBeatValue(frame - 1);
        }

        internal void Detect(string file)
        {
            Task.Run(() => _detect(file));
        }

        private void _detect(string file)
        {

            ProgressViewModel prg = new ProgressViewModel("Beat detection");
            prg.Progress = 0;
            MainViewModel.Instance.AddProgress(prg);

            beats.Clear();

            ISampleSource sampleSrc = CodecFactory.Instance.GetCodec(file).ToSampleSource().ToStereo();
            sampleRate = sampleSrc.WaveFormat.SampleRate;
            long length = sampleSrc.Length;

            //averageIntensity = new float[totalFrames];

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

                if (sampleSrc.Position % 2500 == 0)
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
            Parallel.For(0, beats.Count - 1, (i) =>
            {
                float delta = beats[i + 1] - beats[i];
                int calculatedBpm = (int)Math.Round((1 / delta) * 60);
                histogram.AddOrUpdate(calculatedBpm, 1, (x, y) => ++y);
            });

            //bring into range 80 - 180
            List<int> toRemove = new List<int>();
            foreach (var x in histogram)
            {
                if (x.Key < 80)
                {
                    int b = x.Key;
                    while (b < 80)
                        b *= 2;
                    histogram.AddOrUpdate(b, x.Value, (k, v) => v + x.Value);
                    toRemove.Add(x.Key);
                } else if (x.Key > 180)
                {
                    int b = x.Key;
                    while (b > 180)
                        b /= 2;
                    histogram.AddOrUpdate(b, x.Value, (k, v) => v + x.Value);
                    toRemove.Add(x.Key);
                }
            }
            foreach (int i in toRemove)
            {
                int v = 0;
                histogram.TryRemove(i, out v);
            }           

            bpm = histogram.OrderByDescending(x => x.Value).First().Key;
            while (bpm < 80)
                bpm *= 2;

            //create a csv string for visualizing
            //TODO: Remove after debugging
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> x in histogram)
            {
                sb.AppendFormat("{0},{1}\n", x.Key, x.Value);
            }
            string csv = sb.ToString();

            prg.Name = "Matching BPM " + bpm;
            prg.Progress = 0;

            object progressUpdateLockObject = new object();
            float bps = bpm / 60f;

            ConcurrentDictionary<float, float> fitting = new ConcurrentDictionary<float, float>();
            int progress = 0;
            for (int i = 0; i < beats.Count; i++)
            {
                int n = (int)Math.Floor(beats[i] / bps);
                float localOffset = beats[i] - n * bps;

                float deviation = 0;
                float lastBeat = beats.Last();
                int j = 0;

                while (j / bps <= lastBeat)
                {
                    float targetTime = (j / bps) + localOffset;
                    float delta = findClosestBeat(targetTime) - targetTime;
                    deviation += delta * delta;
                    j++;
                }

                fitting.AddOrUpdate(offset, deviation, (k, v) => deviation);

                lock (progressUpdateLockObject)
                {
                    progress++;
                    prg.Progress = (int)(progress * 100f / beats.Count);
                }
            }

            offset = fitting.OrderBy(x => x.Value).First().Key;

            MainViewModel.Instance.Infotext = string.Format("BPM: {0} | offset: {1}", bpm, offset.ToString("N2"));
            MainViewModel.Instance.RemoveProgress(prg);

        }

        private float findClosestBeat(float targetTime)
        {

            //find last beat before targetTime
            int rangeMin = 0;
            int rangeMax = beats.Count;
            int index = 0;

            while (true)
            {

                index = rangeMin + (rangeMax - rangeMin) / 2;

                if (rangeMax - rangeMin == 0)
                    return beats[rangeMin];

                if (rangeMax - rangeMin == 1)
                {
                    if (index == beats.Count - 1 || Math.Abs(beats[index] - targetTime) < Math.Abs(beats[index + 1] - targetTime))
                        return beats[index];
                    else
                        return beats[index + 1];
                }

                if (beats[index] == targetTime)
                    return beats[index];
                else if (beats[index] < targetTime)
                    rangeMin = index;
                else
                    rangeMax = index;

            }

        }

    }
}
