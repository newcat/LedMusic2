using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using System;

namespace LedMusicVstPlugin
{
    internal sealed class Plugin : VstPluginWithInterfaceManagerBase
    {

        private static readonly int UniquePluginId = new FourCharacterCode("3504").ToInt32();
        private const string PluginName = "LedMusic MIDI Receiver";
        private const string ProductName = "LedMusic";
        private const string VendorName = "newcat";
        private const int PluginVersion = 0001;

        private Guid guid = Guid.NewGuid();
        public LedMusicInterop interop;

        public Plugin() :
            base(PluginName,
                new VstProductInfo(ProductName, VendorName, PluginVersion),
                VstPluginCategory.Synth,
                VstPluginCapabilities.NoSoundInStop,
                0,
                UniquePluginId)
        {
            interop = new LedMusicInterop(guid, this);
        }

        public override void Open(IVstHost host)
        {
            base.Open(host);
            interop.Register();
        }

        public AudioProcessor AudioProcessor
        {
            get { return GetInstance<AudioProcessor>(); }
        }

        public MidiProcessor MidiProcessor
        {
            get { return GetInstance<MidiProcessor>(); }
        }

        protected override IVstPluginAudioProcessor CreateAudioProcessor(IVstPluginAudioProcessor instance)
        {

            if (instance == null)
            {
                return new AudioProcessor();
            }

            // TODO: implement a thread-safe wrapper.
            return base.CreateAudioProcessor(instance);
        }

        protected override IVstMidiProcessor CreateMidiProcessor(IVstMidiProcessor instance)
        {
            if (instance == null)
            {
                return new MidiProcessor(this);
            }

            // TODO: implement a thread-safe wrapper.
            return base.CreateMidiProcessor(instance);
        }

    }
}
