using LedMusic2.ViewModels;

namespace LedMusic2.Sound
{
    public class WaveformModel : VMBase
    {

        private float[] _samples;
        public float[] Samples
        {
            get { return _samples; }
            set
            {
                _samples = value;
                NotifyPropertyChanged();
            }
        }

    }
}
