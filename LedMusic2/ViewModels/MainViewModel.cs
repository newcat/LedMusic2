using LedMusic2.BrowserInterop;
using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.Outputs;
using LedMusic2.Outputs.OutputModels;
using LedMusic2.VstInterop;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

// TODO: Make VST Interop functional
// TODO: Add Scene Naming

namespace LedMusic2.ViewModels
{
    public class MainViewModel : VMBase, IExportable
    {

        public static MainViewModel Instance { get; } = new MainViewModel();
        private MainViewModel() {

            calculationTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(1000 / GlobalProperties.Instance.FPS)
            };
            calculationTimer.Tick += OnCalculationTimerTick;

        }

        private int _activeSceneIndex = 0;
        public int ActiveSceneIndex
        {
            get { return _activeSceneIndex; }
            set
            {
                _activeSceneIndex = value < 0 || value >= Scenes.Count ? 0 : value;
                NotifyPropertyChanged();
            }
        }

        private int _displayedSceneIndex = 0;
        public int DisplayedSceneIndex
        {
            get { return _displayedSceneIndex; }
            set
            {
                _displayedSceneIndex = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isRunning = true;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                NotifyPropertyChanged();
            }
        }

        public SynchronizedCollection<NodeEditorViewModel> Scenes { get; } = new SynchronizedCollection<NodeEditorViewModel>();
        public SynchronizedCollection<ProgressViewModel> Progresses { get; } = new SynchronizedCollection<ProgressViewModel>();
        public OutputManager OutputManager { get; } = new OutputManager();

        #region Private Fields
        private DispatcherTimer calculationTimer;
        #endregion

        #region Public Methods
        public void Initialize()
        {

            OutputManager.FillOutputTypes();
            OutputManager.Outputs.Add(new DummyOutput());

            Scenes[0].Nodes.Add(new OutputNode(new Point(600.0, 150.0), Scenes[0]));
            DisplayedSceneIndex = 0;

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

        public void StartProcessing()
        {
            calculationTimer.Start();
            IsRunning = false;
        }

        public void StopProcessing()
        {
            calculationTimer.Stop();
            IsRunning = true;
        }

        public void CalculateAllNodes()
        {
            Scenes[0].CalculateAllNodes();
            if (ActiveSceneIndex > 0)
                Scenes[ActiveSceneIndex].CalculateAllNodes();
            if (DisplayedSceneIndex > 0 && DisplayedSceneIndex != ActiveSceneIndex)
                Scenes[DisplayedSceneIndex].CalculateAllNodes();
        }

        public void OnCalculationTimerTick(object sender, EventArgs e)
        {
            VstInputManager.Instance.UpdateValues();
            CalculateAllNodes();
        }

        #region Saving and Loading
        private void save()
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

        private void load()
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
                OutputManager.Outputs.Clear();

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
            foreach (var t in OutputManager.OutputTypes)
            {
                if (t.Name == type)
                {
                    OutputBase outputInstance = (OutputBase)t.T.GetConstructor(Type.EmptyTypes).Invoke(null);
                    outputInstance.LoadFromXml(outputX);
                    OutputManager.Outputs.Add(outputInstance);
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
            foreach (OutputBase o in OutputManager.Outputs)
                outputsX.Add(o.GetXmlElement());

            rootX.Add(outputsX);

            return rootX;

        }

        public void LoadFromXml(XElement rootX)
        {

            ProgressViewModel prg = new ProgressViewModel("Loading project");
            Progresses.Add(prg);
                        
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

            Progresses.Remove(prg);

        }
        #endregion

    }
}