using LedMusic2.Nodes.NodeOptions;
using LedMusic2.ViewModels;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Scene Switcher", NodeCategory.OUTPUT)]
    class SceneSwitcherNode : NodeBase
    {

        private readonly NodeInterface<double> sceneInput;
        private readonly TextOption optDisplayedScene = new TextOption("Displayed Scene");

        public SceneSwitcherNode() : base()
        {
            sceneInput = AddInput<double>("Scene Index");
        }

        public override bool Calculate()
        {

            var sceneToActivate = (int)((double)sceneInput.GetValue());
            var scenes = MainViewModel.Instance.Scenes;

            if (sceneToActivate >= -1 && sceneToActivate < scenes.Count &&
                sceneToActivate != scenes.IndexOf(scenes.FindById(MainViewModel.Instance.ActiveSceneId.Get())))
            {
                MainViewModel.Instance.ActiveSceneId.Set(scenes[sceneToActivate].Id);
                return true;
            }
            else
                return false;

        }

    }
}
