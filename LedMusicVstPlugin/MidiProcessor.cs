using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;

namespace LedMusicVstPlugin
{
    internal sealed class MidiProcessor : IVstMidiProcessor
    {
        private readonly Plugin _plugin;

        /// <summary>
        /// Constructs a new Midi Processor.
        /// </summary>
        /// <param name="plugin">Must not be null.</param>
        public MidiProcessor(Plugin plugin)
        {
            _plugin = plugin;
        }

        public int ChannelCount
        {
            get { return 16; }
        }

        /// <summary>
        /// Midi events are received from the host on this method.
        /// </summary>
        /// <param name="events">A collection with midi events. Never null.</param>
        /// <remarks>
        /// Note that some hosts will only receieve midi events during audio processing.
        /// See also <see cref="IVstPluginAudioProcessor"/>.
        /// </remarks>
        public void Process(VstEventCollection events)
        {
            foreach (var ev in events)
            {
                if (ev.EventType == VstEventTypes.MidiEvent)
                {

                    var eventType = ev.Data[0] & 0xF0;

                    if (eventType == 0x80 || eventType == 0x90)
                    {
                        _plugin.interop.Process(ev.Data);
                    }

                }
            }
        }
    }
}
