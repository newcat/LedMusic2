using AttachedCommandBehavior;
using LedMusic2.Enums;
using LedMusic2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;

namespace LedMusic2.ViewModels
{
    public class NodeOptionViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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

        private NodeOptionType _optionType = NodeOptionType.NUMBER;
        public NodeOptionType OptionType
        {
            get { return _optionType; }
            set
            {
                _optionType = value;
                NotifyPropertyChanged();
            }
        }

        public object Value
        {
            get { return getValue(); }
            set
            {
                setValue(value);
                NotifyPropertyChanged();
            }
        }

        //OptionType == NUMBER
        private double _minValue = 0;
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                NotifyPropertyChanged();
            }
        }

        private double _maxValue = 0;
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                NotifyPropertyChanged();
            }
        }

        private SimpleCommand _cmdDecreaseValue = new SimpleCommand();
        public SimpleCommand CmdDecreaseValue { get { return _cmdDecreaseValue; } }

        private SimpleCommand _cmdIncreaseValue = new SimpleCommand();
        public SimpleCommand CmdIncreaseValue { get { return _cmdIncreaseValue; } }

        //OptionType == SELECTION
        private ObservableCollection<string> _options = new ObservableCollection<string>();
        public ObservableCollection<string> Options
        {
            get { return _options; }
            set
            {
                _options = value;
                NotifyPropertyChanged();
            }
        }

        //OptionType == COLOR
        private SimpleCommand _cmdPickColor = new SimpleCommand();
        public SimpleCommand CmdPickColor { get { return _cmdPickColor; } }

        //Internal Values
        private double _valDouble;
        private Color _valColor;
        private bool _valBool;
        private string _valString;

        public NodeOptionViewModel(NodeOptionType type, string name)
        {
            OptionType = type;
            Name = name;

            _cmdDecreaseValue.ExecuteDelegate = (o) => Value = _valDouble - 1;
            _cmdIncreaseValue.ExecuteDelegate = (o) => Value = _valDouble + 1;
            _cmdPickColor.ExecuteDelegate = pickColor;

        }

        private object getValue()
        {
            switch (OptionType)
            {
                case NodeOptionType.BOOL:
                    return _valBool;
                case NodeOptionType.COLOR:
                    return _valColor;
                case NodeOptionType.NUMBER:
                    return _valDouble;
                case NodeOptionType.SELECTION:
                    return _valString;
            }

            return null;
        }

        private void setValue(object value)
        {
            switch (OptionType)
            {
                case NodeOptionType.BOOL:
                    _valBool = (bool)value;
                    break;
                case NodeOptionType.COLOR:
                    _valColor = (Color)value;
                    break;
                case NodeOptionType.NUMBER:
                    if (!(value is double))
                    {
                        double parsed = 0;
                        if (double.TryParse(value.ToString(), out parsed))
                            _valDouble = Math.Min(MaxValue, Math.Max(parsed, MinValue));
                        else
                            _valDouble = MinValue;
                    } else
                    {
                        _valDouble = Math.Min(MaxValue, Math.Max((double)value, MinValue));
                    }
                    break;
                case NodeOptionType.SELECTION:
                    _valString = (string)value;
                    break;
            }
        }

        private void pickColor(object o)
        {

            var c = _valColor.getColorRGB();
            var dlg = new ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);
            dlg.ShowDialog();
            dlg.Dispose();

        }

    }
}
