﻿using AttachedCommandBehavior;
using LedMusic2.Enums;
using LedMusic2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;

namespace LedMusic2.ViewModels
{
    public class NodeOptionViewModel : VMBase, IDisposable
    {

        #region ViewModel Properties
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

        public object DisplayValue
        {
            get { return getValue(true); }
            set
            {
                setDisplayValue(value, true);
                NotifyPropertyChanged();
            }
        }

        public object RenderValue
        {
            get { return getValue(false); }
        }

        private ObservableCollection<Keyframe> _keyframes = new ObservableCollection<Keyframe>();
        public ObservableCollection<Keyframe> Keyframes
        {
            get { return _keyframes; }
            set
            {
                _keyframes = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasKeyframes { get { return Keyframes.Count > 0; } }

        public bool IsKeyframe
        {
            get { return Keyframes.FirstOrDefault(x => x.Frame == MainViewModel.Instance.CurrentFrame) != null; }
        }
        #endregion

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

            _cmdDecreaseValue.ExecuteDelegate = (o) => DisplayValue = _valDouble - 1;
            _cmdIncreaseValue.ExecuteDelegate = (o) => DisplayValue = _valDouble + 1;
            _cmdPickColor.ExecuteDelegate = pickColor;

            if (type == NodeOptionType.PREVIEW)
                calcPreviewBrush();

            MainViewModel.Instance.PropertyChanged += MainVM_PropertyChanged;

        }

        

        private void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == "CurrentFrame")
            {
                setDisplayValue(getValue(false));
                NotifyPropertyChanged("IsKeyframe");
            }

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

        public void AddKeyframe()
        {
            var oldKeyframe = Keyframes.FirstOrDefault(x => x.Frame == MainViewModel.Instance.CurrentFrame);

            if (oldKeyframe != null)
            {
                oldKeyframe.Value = getValue(true);
            } else
            {
                Keyframes.Add(new Keyframe(MainViewModel.Instance.CurrentFrame, getValue(true)));
            }

            Keyframes.Sort();
            NotifyPropertyChanged("HasKeyframes");
            NotifyPropertyChanged("IsKeyframe");
        }

        private object getValue(bool ignoreKeyframes)
        {

            if (!ignoreKeyframes && Keyframes.Count > 0)
            {

                var frame = MainViewModel.Instance.CurrentFrame;

                if (OptionType == NodeOptionType.NUMBER)
                {

                    //Interpolate
                    Keyframe k = Keyframes.FirstOrDefault(x => x.Frame == frame);
                    double val = 0;

                    if (k == null)
                    {

                        Keyframe previous = Keyframes.LastOrDefault(x => x.Frame < frame);
                        Keyframe next = Keyframes.FirstOrDefault(x => x.Frame > frame);

                        if (previous == null)
                            val = (double)next.Value;
                        else if (next == null)
                            val = (double)previous.Value;
                        else
                        {

                            var prevVal = (double)previous.Value;
                            var nextVal = (double)next.Value;

                            double m = (nextVal - prevVal) / (next.Frame - previous.Frame);
                            val = prevVal + (frame - previous.Frame) * m;

                        }
                    } else
                    {
                        val = (double)k.Value;
                    }

                    return val;

                } else
                {

                    Keyframe k = Keyframes.LastOrDefault(x => x.Frame <= frame);

                    if (k != null)
                        return k.Value;
                    else
                        return null;

                }

            }

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

        private void setDisplayValue(object value, bool byUser = false)
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
                    if (byUser || !(value is double))
                        parseNumber(value);
                    else
                        _valDouble = (double)value;
                    break;
                case NodeOptionType.SELECTION:
                    _valString = (string)value;
                    break;
                case NodeOptionType.PREVIEW:
                    _valColorArray = (LedColor[])value;
                    calcPreviewBrush();
                    break;
            }

            if (byUser)
            {
                var keyframe = Keyframes.FirstOrDefault(x => x.Frame == MainViewModel.Instance.CurrentFrame);
                if (keyframe != null)
                {
                    keyframe.Value = getValue(true);
                }
            }

            NotifyPropertyChanged("RenderValue");
            NotifyPropertyChanged("DisplayValue");

        }

        //OptionType == NUMBER
        private void parseNumber(object value)
        {

            double val = 0;

            if (!(value is double))
            {
                double parsed = 0;
                if (double.TryParse(value.ToString(), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out parsed))
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

        }

        //OptionType == COLOR
        private void pickColor(object o)
        {

            var c = _valColor.GetColorRGB();
            var dlg = new ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var resultColor = dlg.Color;
                setDisplayValue(new LedColorRGB(resultColor.R, resultColor.G, resultColor.B), true);
            }
            dlg.Dispose();            

        }

        //OptionType == PREVIEW
        private void calcPreviewBrush()
        {
            if (_valColorArray == null)
                return;

            var len = _valColorArray.Length;
            var coll = new GradientStopCollection();

            if (len == 1 && _valColorArray[0] != null)
            {
                var c = _valColorArray[0].GetColorRGB();
                coll.Add(new GradientStop(Color.FromRgb(c.R, c.G, c.B), 0));
            }
            else if (len == 1)
            {
                coll.Add(new GradientStop(Color.FromRgb(0, 0, 0), 0));
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    if (_valColorArray[i] == null)
                        coll.Add(new GradientStop(Color.FromRgb(0, 0, 0), (double)i / (len - 1)));
                    else
                    {
                        var c = _valColorArray[i].GetColorRGB();
                        coll.Add(new GradientStop(Color.FromRgb(c.R, c.G, c.B), (double)i / (len - 1)));
                    }
                }
            }
            
            PreviewBrush = new LinearGradientBrush(coll, 0);
        }

        #region Saving and Loading
        public XElement GetXmlElement()
        {
            XElement nodeOptionX = new XElement("nodeoption");
            nodeOptionX.SetAttributeValue("type", (int)OptionType);
            nodeOptionX.SetAttributeValue("name", Name);

            if (HasKeyframes)
            {
                XElement keyframesX = new XElement("keyframes");
                foreach (Keyframe k in Keyframes)
                {
                    XElement keyframeX = new XElement("keyframe");
                    keyframeX.SetAttributeValue("frame", k.Frame);
                    keyframeX.Value = k.Value.ToString();
                    keyframesX.Add(keyframeX);
                }
                nodeOptionX.Add(keyframesX);
            }
            else
            {
                if (OptionType == NodeOptionType.NUMBER)
                    nodeOptionX.Add(new XElement("value", _valDouble.ToString(CultureInfo.InvariantCulture)));
                else
                    nodeOptionX.Add(new XElement("value", DisplayValue));
            }
            return nodeOptionX;
        }

        public void LoadFromXml(XElement nodeOptionX)
        {
            foreach (XElement el in nodeOptionX.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "value":
                        DisplayValue = parseValue(el.Value);
                        break;
                    case "keyframes":
                        foreach (XElement keyframeX in el.Elements())
                        {
                            Keyframes.Add(new Keyframe(int.Parse(keyframeX.Attribute("frame").Value), parseValue(keyframeX.Value)));
                        }
                        Keyframes.Sort();
                        break;
                }
            }
        }

        private object parseValue(string s)
        {
            switch (OptionType)
            {
                case NodeOptionType.BOOL:
                    return bool.Parse(s);
                case NodeOptionType.COLOR:
                    return LedColor.Parse(s);
                case NodeOptionType.NUMBER:
                    return double.Parse(s, CultureInfo.InvariantCulture);
                case NodeOptionType.SELECTION:
                    return s;
                default:
                    return null;
            }
        }
#endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //TODO
                    //MainViewModel.Instance.PropertyChanged -= MainVM_PropertyChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
