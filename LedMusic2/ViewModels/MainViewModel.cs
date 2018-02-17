using AttachedCommandBehavior;
using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Helpers;
using LedMusic2.Interfaces;
using LedMusic2.Models;
using LedMusic2.Nodes;
using LedMusic2.Outputs;
using LedMusic2.VstInterop;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace LedMusic2.ViewModels
{
    public class MainViewModel : VMBase, IExportable
    {

        #region Singleton and Constructor
        private static MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance
        {
            get { return _instance; }
        }
        private MainViewModel() {

            calculationTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(1000 / GlobalProperties.Instance.FPS)
            };
            calculationTimer.Tick += OnCalculationTimerTick;

            CmdPlayPause = new SimpleCommand
            {
                ExecuteDelegate = (o) =>
                {
                    if (IsProcessingPaused)
                        StartProcessing();
                    else
                        StopProcessing();
                }
            };

            CmdSaveProject = new SimpleCommand
            {
                ExecuteDelegate = (o) => Save()
            };

            CmdOpenProject = new SimpleCommand
            {
                ExecuteDelegate = (o) => Load()
            };

            CmdConfigureOutputs = new SimpleCommand
            {
                ExecuteDelegate = (o) => new OutputConfigurator().ShowDialog()
            };

        }
        #endregion

        #region Properties        

        private bool _isAddNodePanelOpen = false;
        public bool IsAddNodePanelOpen
        {
            get { return _isAddNodePanelOpen; }
            set
            {
                _isAddNodePanelOpen = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<NodeCategoryModel> _nodeCategories = new ObservableCollection<NodeCategoryModel>();
        public ObservableCollection<NodeCategoryModel> NodeCategories
        {
            get { return _nodeCategories; }
            set
            {
                _nodeCategories = value;
                NotifyPropertyChanged();
            }
        }

        #region Progress
        private ObservableCollection<ProgressViewModel> _progresses = new ObservableCollection<ProgressViewModel>();
        public ObservableCollection<ProgressViewModel> Progresses
        {
            get { return _progresses; }
            set
            {
                _progresses = value;
                NotifyPropertyChanged();
            }
        }

        public string ProgressText
        {
            get
            {
                if (_progresses.Count == 0)
                    return "";
                else if (_progresses.Count == 1)
                    return _progresses[0].Name;
                else
                    return _progresses.Count + " operations running...";
            }
        }

        public int ProgressValue
        {
            get
            {
                if (_progresses.Count == 0)
                    return 0;
                else if (_progresses.Count == 1)
                    return _progresses[0].Progress;
                else
                    return CalcTotalProgressValue();
            }
        }
        #endregion

        private bool _isProcessingPaused = true;
        public bool IsProcessingPaused
        {
            get { return _isProcessingPaused; }
            set
            {
                _isProcessingPaused = value;
                NotifyPropertyChanged();
            }
        }

        private string _infotext = "";
        public string Infotext
        {
            get { return _infotext; }
            set
            {
                _infotext = value;
                NotifyPropertyChanged();
            }
        }

        #region Outputs
        private ObservableCollection<OutputBase> _outputs = new ObservableCollection<OutputBase>();
        public ObservableCollection<OutputBase> Outputs
        {
            get { return _outputs; }
        }

        private ObservableCollection<OutputType> _outputTypes = new ObservableCollection<OutputType>();
        public ObservableCollection<OutputType> OutputTypes
        {
            get { return _outputTypes; }
        }
        #endregion

        #region Commands
        public SimpleCommand CmdPlayPause { get; private set; }
        public SimpleCommand CmdSaveProject { get; private set; }
        public SimpleCommand CmdOpenProject { get; private set; }
        public SimpleCommand CmdConfigureOutputs { get; private set; }
        #endregion
        #endregion

        #region Private Fields
        private DispatcherTimer calculationTimer;
        #endregion

        #region Public Methods
        public void Initialize()
        {

            FillNodeCategories();
            FillOutputTypes();

            StartProcessing();

        }

        public void End()
        {
            //TODO: Dispose of every scenery
        }

        #region Nodes
        public void StartProcessing()
        {
            calculationTimer.Start();
            IsProcessingPaused = false;
        }

        public void StopProcessing()
        {
            calculationTimer.Stop();
            IsProcessingPaused = true;
        }

        public void CalculateAllNodes()
        {
            //TODO: Calculate nodes of global scenery and then currently active scenery
        }

        public void OnCalculationTimerTick(object sender, EventArgs e)
        {
            CalculateAllNodes();
        }       
        #endregion

        #region Progress
        public void AddProgress(ProgressViewModel vm)
        {
            vm.PropertyChanged += Progress_PropertyChanged;
            _progresses.Add(vm);
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }

        public void RemoveProgress(ProgressViewModel vm)
        {
            _progresses.Remove(vm);
            vm.PropertyChanged -= Progress_PropertyChanged;
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }
        #endregion
        #endregion

        #region PropertyChangedEvents
        private void Progress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("ProgressText");
            NotifyPropertyChanged("ProgressValue");
        }
        #endregion

        #region Private Methods
        private void FillNodeCategories()
        {

            //Get all node classes
            var nodeClasses = Assembly.GetCallingAssembly().GetTypes().Where((t) => string.Equals(t.Namespace, "LedMusic2.Nodes") && !t.IsAbstract);
            foreach (NodeCategory category in Enum.GetValues(typeof(NodeCategory)))
            {
                var c = new NodeCategoryModel(Enum.GetName(typeof(NodeCategory), category));
                var n = nodeClasses.Where((t) => {
                        var x = t.GetCustomAttribute(typeof(NodeAttribute));
                        if (x == null)
                            return false;
                        return ((NodeAttribute)x).Category == category;
                    });

                foreach (var x in n)
                    c.NodeTypes.Add(new NodeType(((NodeAttribute)x.GetCustomAttribute(typeof(NodeAttribute))).Name, x));

                NodeCategories.Add(c);
            }

        }

        private void FillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs" && !t.IsAbstract);
            var outputs = outputClasses.Where(t => t.GetCustomAttribute<OutputAttribute>() != null);

            foreach (var t in outputs)
                OutputTypes.Add(new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t));

        }

        private int CalcTotalProgressValue()
        {
            var val = 0;
            foreach (var x in _progresses)
            {
                val += x.Progress;
            }
            return (int)Math.Floor((double)val / _progresses.Count);
        }
        #endregion

        #region Saving and Loading
        private void Save()
        {

            SaveFileDialog sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "lmp",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (sfd.ShowDialog() != true) return;

            Stream s = sfd.OpenFile();

            XDocument doc = new XDocument
            {
                Declaration = new XDeclaration("1.0", "UTF-8", "yes")
            };
            doc.Add(GetXmlElement());
            doc.Save(s);
            s.Close();

        }

        private void Load()
        {

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "LedMusic Project File|*.lmp",
                Multiselect = false
            };
            if (ofd.ShowDialog() != true)
                return;

            Stream s = ofd.OpenFile();

            try
            {

                XDocument doc = XDocument.Load(s);

                //TODO: Dispose of scenes
                Outputs.Clear();

                LoadFromXml((XElement)doc.FirstNode);

                CalculateAllNodes();                

            } catch (XmlException e)
            {
                MessageBox.Show("Failed to open project file: " + e.Message);
                Console.WriteLine(e.StackTrace);
            } finally
            {
                s.Close();
            }

        }

        private void LoadOutput(XElement outputX)
        {

            string type = outputX.Attribute("type").Value;
            foreach (var t in OutputTypes)
            {
                if (t.Name == type)
                {
                    OutputBase outputInstance = (OutputBase)t.T.GetConstructor(Type.EmptyTypes).Invoke(null);
                    outputInstance.LoadFromXml(outputX);
                    Outputs.Add(outputInstance);
                }
            }

        }

        public XElement GetXmlElement()
        {

            XElement rootX = new XElement("ledmusicproject");

            XElement scenesX = new XElement("scenes");
            //TODO

            XElement outputsX = new XElement("outputs");
            foreach (OutputBase o in Outputs)
            {
                outputsX.Add(o.GetXmlElement());
            }
            rootX.Add(outputsX);

            return rootX;

        }

        public void LoadFromXml(XElement rootX)
        {

            ProgressViewModel prg = new ProgressViewModel("Loading project");
            AddProgress(prg);
                        
            int totalElements = rootX.Elements().Count();
            int counter = 0;

            foreach (XElement n in rootX.Elements())
            {

                switch (n.Name.LocalName)
                {

                    //TODO: Scenes

                    case "outputs":
                        foreach (XElement outputX in n.Elements())
                            LoadOutput(outputX);
                        break;

                }

                counter++;
                prg.Progress = (int)(1.0 * counter / totalElements) * 100;

            }

            RemoveProgress(prg);

        }
        #endregion

    }
}
