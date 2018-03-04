using AttachedCommandBehavior;
using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.Outputs;
using LedMusic2.Outputs.OutputModels;
using LedMusic2.VstInterop;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

// TODO: Try to find out, why we have so many exceptions
// TODO: Make VST Interop functional
// TODO: Add Scene Naming

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

            CmdAddScene = new SimpleCommand
            {
                ExecuteDelegate = (o) =>
                {
                    var newScene = new NodeEditorViewModel();
                    Scenes.Add(newScene);
                    DisplayedSceneIndex = Scenes.IndexOf(newScene);
                }
            };

            CmdDeleteScene = new SimpleCommand
            {
                ExecuteDelegate = (o) =>
                {
                    var index = DisplayedSceneIndex;
                    DisplayedSceneIndex = -1;
                    Scenes.RemoveAt(index);
                },
                CanExecuteDelegate = (o) => DisplayedSceneIndex >= 0
            };

            CmdSelectGlobalScene = new SimpleCommand
            {
                ExecuteDelegate = (o) => { DisplayedSceneIndex = -1; }
            };

        }
        #endregion

        #region Properties
        private int _activeSceneIndex = -1;
        public int ActiveSceneIndex
        {
            get { return _activeSceneIndex; }
            set
            {
                if (value < -1 || value >= Scenes.Count)
                    _activeSceneIndex = -1;
                else
                    _activeSceneIndex = value;

                if (_activeSceneIndex == -1)
                    Infotext = "Active Scene: None";
                else
                    Infotext = string.Format("Active Scene: [{0}] {1}", _activeSceneIndex, "TODO" /* TODO: Scenes[_activeSceneIndex].Name*/); 

            }
        }

        private int _displayedSceneIndex = -1;
        public int DisplayedSceneIndex
        {
            get { return _displayedSceneIndex; }
            set
            {

                _displayedSceneIndex = value;

                GlobalScene.IsDisplayed = false;
                foreach (var s in Scenes) s.IsDisplayed = false;

                if (_displayedSceneIndex == -1)
                    GlobalScene.IsDisplayed = true;
                else
                    Scenes[_displayedSceneIndex].IsDisplayed = true;

                NotifyPropertyChanged();
                NotifyPropertyChanged("DisplayedScene");

            }
        }

        public NodeEditorViewModel DisplayedScene
        {
            get { return DisplayedSceneIndex == -1 ? GlobalScene : Scenes[DisplayedSceneIndex]; }
        }

        private NodeEditorViewModel _globalScene = new NodeEditorViewModel();
        public NodeEditorViewModel GlobalScene
        {
            get { return _globalScene; }
            set
            {
                _globalScene = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<NodeEditorViewModel> Scenes { get; } =
            new ObservableCollection<NodeEditorViewModel>();

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
        public SimpleCommand CmdAddScene { get; private set; }
        public SimpleCommand CmdDeleteScene { get; private set; }
        public SimpleCommand CmdSelectGlobalScene { get; private set; }
        #endregion
        #endregion

        #region Private Fields
        private DispatcherTimer calculationTimer;
        #endregion

        #region Public Methods
        public void Initialize()
        {

            FillOutputTypes();

            Outputs.Add(new DummyOutput());
            GlobalScene.Nodes.Add(new OutputNode(new Point(600.0, 150.0), GlobalScene));
            DisplayedSceneIndex = -1;

            StartProcessing();

        }

        public void End()
        {
            foreach (var s in Scenes)
                s.Dispose();
            VstInputManager.Instance.Shutdown();
        }

        public void SelectScene(NodeEditorViewModel scene)
        {
            if (Scenes.IndexOf(scene) >= 0)
                DisplayedSceneIndex = Scenes.IndexOf(scene);
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
            GlobalScene.CalculateAllNodes();
            if (ActiveSceneIndex > -1)
                Scenes[ActiveSceneIndex].CalculateAllNodes();
            if (DisplayedSceneIndex != -1 && DisplayedSceneIndex != ActiveSceneIndex)
                DisplayedScene.CalculateAllNodes();
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
        private int CalcTotalProgressValue()
        {
            var val = 0;
            foreach (var x in _progresses)
            {
                val += x.Progress;
            }
            return (int)Math.Floor((double)val / _progresses.Count);
        }

        private void FillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs" && !t.IsAbstract);
            var outputs = outputClasses.Where(t => t.GetCustomAttribute<OutputAttribute>() != null);

            foreach (var t in outputs)
                OutputTypes.Add(new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t));

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

                foreach (var sc in Scenes) sc.Dispose();
                Scenes.Clear();
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
            foreach (var sc in Scenes)
                scenesX.Add(sc.GetXmlElement());

            XElement outputsX = new XElement("outputs");
            foreach (OutputBase o in Outputs)
                outputsX.Add(o.GetXmlElement());

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

                    case "scenes":
                        foreach (var sceneX in n.Elements())
                        {
                            var scene = new NodeEditorViewModel();
                            scene.LoadFromXml(sceneX);
                            Scenes.Add(scene);
                        }
                        break;

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