using LedMusic2.NodeConnection;
using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.Outputs;
using LedMusic2.Outputs.OutputModels;
using LedMusic2.Reactive;
using LedMusic2.VstInterop;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

// TODO: Make VST Interop functional
// TODO: Add Scene Naming

namespace LedMusic2.ViewModels
{
    public class MainViewModel : ReactiveObject, IExportable
    {

        public static MainViewModel Instance { get; } = new MainViewModel();

        public ReactivePrimitive<Guid> ActiveSceneId { get; }
            = new ReactivePrimitive<Guid>(Guid.Empty);

        public ReactivePrimitive<Guid> DisplayedSceneId { get; }
            = new ReactivePrimitive<Guid>(Guid.Empty);

        public ReactivePrimitive<bool> IsRunning { get; }
            = new ReactivePrimitive<bool>(true);

        public ReactiveCollection<Scene> Scenes { get; }
            = new ReactiveCollection<Scene>();

        public ReactiveCollection<ProgressViewModel> Progress { get; }
            = new ReactiveCollection<ProgressViewModel>();

        public OutputManager OutputManager { get; } = new OutputManager();

        public MainViewModel() {

            OutputManager.FillOutputTypes();
            OutputManager.AddOutput(new DummyOutput());

            Scenes.CommandHandler = new Action<string, JToken, ReactiveCollection<Scene>>(scenesCommandHandler);
            addScene();

            var outputNode = new OutputNode();
            var numberNode = new DoubleValueNode();
            var conn = new Connection(numberNode.Outputs[0], outputNode.Inputs[0]);
            Scenes[0].Name.Set("Global Scene");
            Scenes[0].Nodes.Add(outputNode);
            Scenes[0].Nodes.Add(numberNode);
            Scenes[0].Connections.Add(conn);
            DisplayedSceneId.Set(Scenes[0].Id);

        }

        public void End()
        {
            foreach (var s in Scenes)
                s.Dispose();
            VstInputManager.Instance.Shutdown();
        }        

        public void Tick()
        {
            VstInputManager.Instance.UpdateValues();
            calculateAllNodes();
        }

        private void scenesCommandHandler(string command, JToken payload, ReactiveCollection<Scene> coll)
        {
            var id = "";
            switch (command)
            {
                case "add":
                    addScene();
                    break;
                case "delete":
                    id = (payload as JValue).Value<string>();
                    if (!string.IsNullOrEmpty(id))
                        deleteScene(id);
                    break;
                case "select":
                    id = (payload as JValue).Value<string>();
                    if (!string.IsNullOrEmpty(id))
                        selectScene(id);
                    break;
            }
        }

        private void addScene()
        {
            var scene = new Scene();
            Scenes.Add(scene);
            selectScene(scene.Id.ToString());
        }

        private void selectScene(string id)
        {
            var scene = Scenes.FindById(id);
            if (scene != null)
                DisplayedSceneId.Set(scene.Id);
        }

        private void deleteScene(string id)
        {
            var scene = Scenes.FindById(id);
            if (scene == null || Scenes[0] == scene) return;
            if (Scenes.FindById(DisplayedSceneId.Get()) == scene)
                DisplayedSceneId.Set(Scenes[0].Id);
            Scenes.Remove(scene);
        }

        private void calculateAllNodes()
        {
            Scenes[0].CalculateAllNodes();
            if (ActiveSceneId.Get() != Guid.Empty)
                Scenes.FindById(ActiveSceneId.Get()).CalculateAllNodes();
            if (DisplayedSceneId.Get() != Guid.Empty && DisplayedSceneId != ActiveSceneId)
                Scenes.FindById(DisplayedSceneId.Get()).CalculateAllNodes();
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
            if (sfd.ShowDialog() != DialogResult.OK) return;

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
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Stream s = ofd.OpenFile();

            try
            {

                XDocument doc = XDocument.Load(s);

                foreach (var sc in Scenes) sc.Dispose();
                Scenes.Clear();
                OutputManager.Outputs.Clear();

                LoadFromXml((XElement)doc.FirstNode);

                calculateAllNodes();                

            } catch (XmlException e)
            {
                MessageBox.Show("Failed to open project file: " + e.Message);
                Console.WriteLine(e.StackTrace);
            } finally
            {
                s.Close();
            }

        }

        private void loadOutput(XElement outputX)
        {

            string type = outputX.Attribute("type").Value;
            foreach (var t in OutputManager.OutputTypes)
            {
                if (t.Name.Get() == type)
                {
                    OutputBase outputInstance = (OutputBase)t.T.GetConstructor(Type.EmptyTypes).Invoke(null);
                    outputInstance.LoadFromXml(outputX);
                    OutputManager.AddOutput(outputInstance);
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
            foreach (var o in OutputManager.Outputs)
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
                            var scene = new Scene();
                            scene.LoadFromXml(sceneX);
                            Scenes.Add(scene);
                        }
                        break;

                    case "outputs":
                        foreach (XElement outputX in n.Elements())
                            loadOutput(outputX);
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