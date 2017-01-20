namespace LedMusic2.ViewModels
{
    public class ProgressViewModel : VMBase
    {

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        private int _progress = 0;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged();
            }
        }

        public ProgressViewModel(string name)
        {
            Name = name;
        }

    }
}
