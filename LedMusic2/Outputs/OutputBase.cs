using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;

namespace LedMusic2.Outputs
{
    public abstract class OutputBase : VMBase
    {

        public abstract string DefaultName { get; }
        public abstract Type ViewType { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        protected OutputBase()
        {
            Name = DefaultName;
        }

        public abstract void CalculationDone(LedColor[] calculationResult);

    }
}
