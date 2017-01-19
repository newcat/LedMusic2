using AttachedCommandBehavior;
using LedMusic2.Enums;
using LedMusic2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

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

        #region OptionType == NUMBER
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
        #endregion

        #region OptionType == SELECTION
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
        #endregion

        #region OptionType == COLOR
        private SimpleCommand _cmdPickColor = new SimpleCommand();
        public SimpleCommand CmdPickColor { get { return _cmdPickColor; } }
        #endregion

        #region OptionType == PREVIEW
        private LinearGradientBrush _previewBrush;
        public LinearGradientBrush PreviewBrush
        {
            get { return _previewBrush; }
            set
            {
                _previewBrush = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region OptionType == CUSTOM
        private FrameworkElement _customUIElement;
        public FrameworkElement CustomUIElement
        {
            get { return _customUIElement; }
            set
            {
                _customUIElement = value;
                NotifyPropertyChanged();
            }
        }

        private object _customViewModel;
        public object CustomViewModel
        {
            get { return _customViewModel; }
            set
            {
                _customViewModel = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Internal Values
        private double _valDouble = 0.0;
        private LedColor _valColor = new LedColorRGB(0, 0, 0);
        private bool _valBool = false;
        private string _valString = "";
        private LedColor[] _valColorArray = { new LedColorRGB(0, 0, 0) };
        #endregion

        public NodeOptionViewModel(NodeOptionType type, string name)
        {

            OptionType = type;
            Name = name;

            _cmdDecreaseValue.ExecuteDelegate = (o) => Value = _valDouble - 1;
            _cmdIncreaseValue.ExecuteDelegate = (o) => Value = _valDouble + 1;
            _cmdPickColor.ExecuteDelegate = pickColor;

            calcPreviewBrush();

        }

        public NodeOptionViewModel(NodeOptionType type, string name,
            Type customUIControlType, object viewmodelInstance)
        {
            if (type != NodeOptionType.CUSTOM)
                throw new ArgumentException(
                    "When using a custom UI for the options, set NodeOptionType to CUSTOM.");

            if (!customUIControlType.IsSubclassOf(typeof(FrameworkElement)))
                throw new ArgumentException(
                    "customUIControl needs to be a subclass of FrameworkElement.");

            OptionType = type;
            Name = name;
            CustomViewModel = viewmodelInstance;

            CustomUIElement = (FrameworkElement)Activator.CreateInstance(customUIControlType);
            CustomUIElement.DataContext = CustomViewModel;

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
                case NodeOptionType.PREVIEW:
                    return _valColorArray;
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
                    _valColor = (LedColor)value;
                    break;
                case NodeOptionType.NUMBER:
                    parseNumber(value);
                    break;
                case NodeOptionType.SELECTION:
                    _valString = (string)value;
                    break;
                case NodeOptionType.PREVIEW:
                    _valColorArray = (LedColor[])value;
                    calcPreviewBrush();
                    break;
            }
        }

        private void parseNumber(object value)
        {

            double val = 0;

            if (!(value is double))
            {
                double parsed = 0;
                if (double.TryParse(value.ToString(), out parsed))
                    val = parsed;
                else
                    val = MinValue;
            }
            else
            {
                val = (double)value;
            }

            if (MaxValue == 0 && MinValue == 0)
            {
                _valDouble = val;
            } else
            {
                _valDouble = Math.Min(MaxValue, Math.Max(val, MinValue));
            }

            NotifyPropertyChanged("Value");

        }

        private void pickColor(object o)
        {

            var c = _valColor.getColorRGB();
            var dlg = new ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var resultColor = dlg.Color;
                _valColor = new LedColorRGB(resultColor.R, resultColor.G, resultColor.B);
                NotifyPropertyChanged("Value");
            }
            dlg.Dispose();            

        }

        private void calcPreviewBrush()
        {
            if (_valColorArray == null)
                return;

            var len = _valColorArray.Length;
            var coll = new GradientStopCollection();
            for (int i = 0; i < len; i++)
            {
                if (_valColorArray[i] == null)
                {
                    coll.Add(new GradientStop(Color.FromRgb(0, 0, 0), (double)i / len));
                } else
                {
                    var c = _valColorArray[i].getColorRGB();
                    coll.Add(new GradientStop(Color.FromRgb(c.R, c.G, c.B), (double)i / len));
                }                    
            }
            PreviewBrush = new LinearGradientBrush(coll, 0);
        }

    }
}
