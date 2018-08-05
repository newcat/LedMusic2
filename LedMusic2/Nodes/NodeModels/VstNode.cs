﻿using LedMusic2.Nodes.NodeOptions;
using LedMusic2.Reactive;
using LedMusic2.VstInterop;
using System;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("VST Channel", NodeCategory.INPUT)]
    class VstNode : NodeBase
    {

        private NodeInterface<double> niValue;
        private NodeInterface<double> niNote;
        private readonly SelectOption<ReactiveListItem<string>> channelOption;
        private readonly NumberOption midiChannelOption;

        public VstChannel SelectedChannel { get; set; }

        public VstNode() : base()
        {
            
            niValue = AddOutput<double>("Value");
            niNote = AddOutput<double>("Note");

            midiChannelOption = new NumberOption("MIDI Channel");
            midiChannelOption.MinValue.Set(1);
            midiChannelOption.MaxValue.Set(16);
            midiChannelOption.Value.Set(1);
            Options.Add(midiChannelOption);

            channelOption = new SelectOption<ReactiveListItem<string>>("Channel");
            Options.Add(channelOption);

        }

        public override bool Calculate()
        {

            if (SelectedChannel == null)
                return false;

            var changed = false;

            if (SelectedChannel.Type == VstChannelType.MIDI)
            {
                var midiChannel = (int)midiChannelOption.Value.Get() - 1;
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
