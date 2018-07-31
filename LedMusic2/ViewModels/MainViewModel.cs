using LedMusic2.Reactive;
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
    public class MainViewModel : ReactiveObject, IExportable
    {

        //public static MainViewModel Instance { get; } = new MainViewModel();
        public static MainViewModel Instance { get; } = null;

        public ReactivePrimitive<int> ActiveSceneIndex { get; }
            = new ReactivePrimitive<int>(0);

        public ReactivePrimitive<int> DisplayedSceneIndex { get; }
            = new ReactivePrimitive<int>(0);

        public ReactivePrimitive<bool> IsRunning { get; }
            = new ReactivePrimitive<bool>(true);

        public ReactiveCollection<NodeEditorViewModel> Scenes { get; }
            = new ReactiveCollection<NodeEditorViewModel>();

        public ReactiveCollection<ProgressViewModel> Progress { get; }
            = new ReactiveCollection<ProgressViewModel>();

        public OutputManager OutputManager { get; } = new OutputManager();

        private DispatcherTimer calculationTimer;

        public MainViewModel() {

            calculationTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(1000 / GlobalProperties.Instance.FPS)
            };
            calculationTimer.Tick += OnCalculationTimerTick;

            OutputManager.FillOutputTypes();
            OutputManager.Outputs.Add(new DummyOutput());

            Scenes.Add(new NodeEditorViewModel());
            Scenes[0].Nodes.Add(new OutputNode());
            DisplayedSceneIndex.Set(0);

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
                DisplayedSceneIndex.Set(Scenes.IndexOf(scene));
        }

        public void StartProcessing()
        {
            calculationTimer.Start();
            IsRunning.Set(false);
        }

        public void StopProcessing()
        {
            calculationTimer.Stop();
            IsRunning.Set(true);
        }

        public void CalculateAllNodes()
        {
            Scenes[0].CalculateAllNodes();
            if (ActiveSceneIndex.Get() > 0)
                Scenes[ActiveSceneIndex.Get()].CalculateAllNodes();
            if (DisplayedSceneIndex.Get() > 0 && DisplayedSceneIndex != ActiveSceneIndex)
                Scenes[DisplayedSceneIndex.Get()].CalculateAllNodes();
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
                if (t.Name.Get() == type)
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
            Progress.Add(prg);
                        
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
                prg.Progress.Set((int)(1.0 * counter / totalElements) * 100);

            }

            Progress.Remove(prg);

        }
        #endregion

    }
}