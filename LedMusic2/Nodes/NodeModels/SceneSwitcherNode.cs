using LedMusic2.NodeEditor;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Scene Switcher", NodeCategory.OUTPUT)]
    class SceneSwitcherNode : NodeBase
    {

        private NodeInterface<double> sceneInput;
        private NodeOption displayedSceneNameOption = new NodeOption(NodeOptionType.TEXT, "Displayed Scene");

        public SceneSwitcherNode(Point initPosition, NodeEditorViewModel parentVm) : base(initPosition, parentVm)
        {
            sceneInput = AddInput<double>("Scene Index");
        }

        public override bool Calculate()
        {

            var sceneToActivate = (int)((double)sceneInput.GetValue());

            if (sceneToActivate >= -1 && sceneToActivate < MainViewModel.Instance.Scenes.Count &&
                sceneToActivate != MainViewModel.Instance.ActiveSceneIndex)
            {
                MainViewModel.Instance.ActiveSceneIndex = sceneToActivate;
                return true;
            }
            else
                return false;

        }

    }
}
