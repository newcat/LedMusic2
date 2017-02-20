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

        private int _ledCount = 30;
        public int LedCount
        {
            get { return _ledCount; }
            set
            {
                _ledCount = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentProjectFile { get; set; }

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