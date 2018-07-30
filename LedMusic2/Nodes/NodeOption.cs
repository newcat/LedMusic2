using LedMusic2.BrowserInterop;
using LedMusic2.LedColors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{
    public class NodeOption : ReactiveObject, IReactiveListItem
    {

        public override string ReactiveName => "NodeOption";
        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name = new ReactivePrimitive<string>("Name", "");
        public ReactivePrimitive<NodeOptionType> Type = new ReactivePrimitive<NodeOptionType>("Type", NodeOptionType.NUMBER);
        public ReactivePrimitive<object> Value;

        //OptionType == NUMBER
        public ReactivePrimitive<double> MinValue = new ReactivePrimitive<double>("MinValue", 0.0);
        public ReactivePrimitive<double> MaxValue = new ReactivePrimitive<double>("MaxValue", 0.0);

        public List<string> Options = new List<string>();

        /*
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
        */

        #region Internal Values
        private double _valDouble = 0.0;
        private LedColor _valColor = new LedColorRGB(0, 0, 0);
        private bool _valBool = false;
        private string _valString = "";
        private LedColor[] _valColorArray = { new LedColorRGB(0, 0, 0) };
        #endregion

        public NodeOption(NodeOptionType type, string name)
        {

            Name.Set(name);
            Type.Set(type);

            Value = new ReactivePrimitive<object>("Value")
            {
                CustomGetter = (v) => getValue(),
                CustomSetter = (v) => { setDisplayValue(v, true); return getValue(); }
            };

            /*if (type == NodeOptionType.PREVIEW)
                CalcPreviewBrush();*/

        }

        public NodeOption(NodeOptionType type, string name,
            Type customUIControlType, object viewmodelInstance)
        {
            if (type != NodeOptionType.CUSTOM)
                throw new ArgumentException(
                    "When using a custom UI for the options, set NodeOptionType to CUSTOM.");

            if (!customUIControlType.IsSubclassOf(typeof(FrameworkElement)))
                throw new ArgumentException(
                    "customUIControl needs to be a subclass of FrameworkElement.");

            Name.Set(name);
            Type.Set(type);

        }

        private object getValue()
        {

            switch (Type.Get())
            {
                case NodeOptionType.BOOL:
                    return _valBool;
                case NodeOptionType.COLOR:
                    return _valColor;
                case NodeOptionType.NUMBER:
                    return _valDouble;
                case NodeOptionType.SELECTION:
                case NodeOptionType.TEXT:
                    return _valString;
                case NodeOptionType.PREVIEW:
                    return _valColorArray;
            }

            return null;
        }

        private void setDisplayValue(object value, bool byUser = false)
        {
            switch (Type.Get())
            {
                case NodeOptionType.BOOL:
                    _valBool = (bool)value;
                    break;
                case NodeOptionType.COLOR:
                    _valColor = (LedColor)value;
                    break;
                case NodeOptionType.NUMBER:
                    /*if (byUser || !(value is double))
                        ParseNumber(value);
                    else
                        _valDouble = (double)value;*/
                    break;
                case NodeOptionType.SELECTION:
                case NodeOptionType.TEXT:
                    _valString = (string)value;
                    break;
                case NodeOptionType.PREVIEW:
                    _valColorArray = (LedColor[])value;
                    //CalcPreviewBrush();
                    break;
            }

        }

        /*
        //OptionType == NUMBER
        private void ParseNumber(object value)
        {

            double val = 0;

            if (!(value is double))
            {
                if (double.TryParse(value.ToString(), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out double parsed))
                    val = parsed;
                else
                    val = MinValue.Get();
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

        }

        private void IncreaseNumber()
        {
            DisplayValue = _valDouble + (Keyboard.IsKeyDown(Key.LeftAlt) ? 0.1 : 1);
        }

        private void DecreaseNumber()
        {
            DisplayValue = _valDouble - (Keyboard.IsKeyDown(Key.LeftAlt) ? 0.1 : 1);
        }

        //OptionType == COLOR
        private void PickColor(object o)
        {

            var c = _valColor.GetColorRGB();
            var dlg = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B)
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var resultColor = dlg.Color;
                setDisplayValue(new LedColorRGB(resultColor.R, resultColor.G, resultColor.B), true);
            }
            dlg.Dispose();            

        }

        //OptionType == PREVIEW
        private void CalcPreviewBrush()
        {
            if (_valColorArray == null)
                return;

            var len = _valColorArray.Length;
            var coll = new GradientStopCollection();

            if (len == 1 && _valColorArray[0] != null)
            {
                var c = _valColorArray[0].GetColorRGB();
                coll.Add(new GradientStop(System.Windows.Media.Color.FromRgb(c.R, c.G, c.B), 0));
            }
            else if (len == 1)
            {
                coll.Add(new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0));
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    if (_valColorArray[i] == null)
                        coll.Add(new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), (double)i / (len - 1)));
                    else
                    {
                        var c = _valColorArray[i].GetColorRGB();
                        coll.Add(new GradientStop(System.Windows.Media.Color.FromRgb(c.R, c.G, c.B), (double)i / (len - 1)));
                    }
                }
            }
            
            PreviewBrush = new LinearGradientBrush(coll, 0);
        }
        */

        #region Saving and Loading
        public XElement GetXmlElement()
        {
            XElement nodeOptionX = new XElement("nodeoption");
            nodeOptionX.SetAttributeValue("type", (int)Type.Get());
            nodeOptionX.SetAttributeValue("name", Name);

            if (Type.Get() == NodeOptionType.NUMBER)
                nodeOptionX.Add(new XElement("value", _valDouble.ToString(CultureInfo.InvariantCulture)));
            else
                nodeOptionX.Add(new XElement("value", Value.Get()));

            return nodeOptionX;
        }

        public void LoadFromXml(XElement nodeOptionX)
        {
            foreach (XElement el in nodeOptionX.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "value":
                        Value.Set(parseValue(el.Value));
                        break;
                }
            }
        }

        private object parseValue(string s)
        {
            switch (Type.Get())
            {
                case NodeOptionType.BOOL:
                    return bool.Parse(s);
                case NodeOptionType.COLOR:
                    return LedColorRGB.Parse(s);
                case NodeOptionType.NUMBER:
                    return double.Parse(s, CultureInfo.InvariantCulture);
                case NodeOptionType.SELECTION:
                    return s;
                default:
                    return null;
            }
        }
#endregion

    }
}
