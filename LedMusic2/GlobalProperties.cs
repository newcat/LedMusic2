using LedMusic2.ViewModels;

namespace LedMusic2
{

    class GlobalProperties : VMBase
    {

        private int _fps = 30;
        public int FPS
        {
            get { return _fps; }
            set
            {
                _fps = value;
                NotifyPropertyChanged();
            }
        }

        private int _resolution = 30;
        public int Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value;
                NotifyPropertyChanged();
            }
        }

        #region Constructor
        private static GlobalProperties instance;

        private GlobalProperties() { }

        public static GlobalProperties Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalProperties();
                }
                return instance;
            }
        }
        #endregion

    }
}