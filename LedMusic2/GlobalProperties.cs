﻿using LedMusic2.ViewModels;

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

        private double _bpm = 130;
        public double BPM
        {
            get { return _bpm; }
            set
            {
                _bpm = value;
                NotifyPropertyChanged();
            }
        }

        private double _beatOffset = 0;
        public double BeatOffset
        {
            get { return _beatOffset; }
            set
            {
                _beatOffset = value;
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