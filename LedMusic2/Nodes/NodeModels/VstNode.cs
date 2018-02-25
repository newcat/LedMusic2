using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.VstInterop;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("VST Channel", NodeCategory.INPUT)]
    class VstNode : NodeBase
    {

        private NodeInterface<double> niValue;
        private NodeInterface<double> niNote;
        private NodeOption channelOption;

        private VstChannel _selectedChannel;
        public VstChannel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                _selectedChannel = value;
                NotifyPropertyChanged();
            }
        }

        public VstNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            niValue = AddOutput<double>("Value");
            niNote = AddOutput<double>("Note");

            channelOption = new NodeOption(NodeOptionType.CUSTOM, "Channel", typeof(VstSelection), this);

        }

        public override bool Calculate()
        {

            if (SelectedChannel == null)
                return false;

            var changed = false;

            if (SelectedChannel.Value != niValue.Value)
            {
                niValue.SetValue(SelectedChannel.Value);
                changed = true;
            }

            if (SelectedChannel.Note != niNote.Value)
            {
                niNote.SetValue(SelectedChannel.Note);
                changed = true;
            }

            return changed;

        }
    }
}
