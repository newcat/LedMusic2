using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.VstInterop;
using System.Windows;
using System;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("VST Channel", NodeCategory.INPUT)]
    class VstNode : NodeBase
    {

        private NodeInterface<double> niValue;
        private NodeInterface<double> niNote;
        private readonly NodeOption channelOption;
        private readonly NodeOption midiChannelOption;

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

            MinWidth = 150;

            niValue = AddOutput<double>("Value");
            niNote = AddOutput<double>("Note");

            midiChannelOption = new NodeOption(NodeOptionType.NUMBER, "MIDI Channel")
            {
                MinValue = 1,
                MaxValue = 16
            };
            midiChannelOption.DisplayValue = 1;
            Options.Add(midiChannelOption);

            channelOption = new NodeOption(NodeOptionType.CUSTOM, "Channel", typeof(VstSelection), this);
            Options.Add(channelOption);

        }

        public override bool Calculate()
        {

            if (SelectedChannel == null)
                return false;

            var changed = false;

            if (SelectedChannel.Type == VstChannelType.MIDI)
            {
                var midiChannel = (int)((double)midiChannelOption.RenderValue) - 1;
                midiChannel = Math.Min(15, Math.Max(0, midiChannel));
                if (SelectedChannel.Notes[midiChannel].Number != niNote.Value)
                {
                    niNote.SetValue((double)SelectedChannel.Notes[midiChannel].Number);
                    changed = true;
                }
                if (SelectedChannel.Notes[midiChannel].Velocity != niValue.Value)
                {
                    niValue.SetValue((double)SelectedChannel.Notes[midiChannel].Velocity);
                    changed = true;
                }
            } else
            {
                if (SelectedChannel.Value != niValue.Value)
                {
                    niValue.SetValue(SelectedChannel.Value);
                    changed = true;
                }
            }            

            return changed;

        }
    }
}
