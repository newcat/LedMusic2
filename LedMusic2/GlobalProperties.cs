using LedMusic2.ViewModels;

namespace LedMusic2
{

    class GlobalProperties
    {

        public int FPS { get; set; } = 30;
        public int Resolution { get; set; } = 30;

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