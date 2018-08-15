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
    public class MainViewModel : ReactiveObject
    {

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

        public OutputManager OutputManager { get; set; } = new OutputManager();
        public VstInputManager VstManager { get; set; } = new VstInputManager();

        public MainViewModel() {
        }

        public new void Initialize()
        {

            OutputManager.FillOutputTypes();
            OutputManager.AddOutput(new DummyOutput());

            Scenes.CommandHandler = new Action<string, JToken, ReactiveCollection<Scene>>(scenesCommandHandler);
            addScene();
            var globalScene = Scenes[0];
            globalScene.Name.Set("Global Scene");

            var outputNode = new OutputNode();
            var numberNode = new DoubleValueNode();
            var conn = new Connection(numberNode.Outputs[0], outputNode.Inputs[0]);
            globalScene.Nodes.Add(outputNode);
            globalScene.Nodes.Add(numberNode);
            globalScene.Connections.Add(conn);

            DisplayedSceneId.Set(globalScene.Id);

        }

        public void End()
        {
            VstManager.Shutdown();
        }        

        public void Tick()
        {
            VstManager.UpdateValues();
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

    }
}