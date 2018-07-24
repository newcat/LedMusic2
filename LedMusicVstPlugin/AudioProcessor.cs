using Jacobi.Vst.Core;
using Jacobi.Vst.Framework.Plugin;

namespace LedMusicVstPlugin
{
    class AudioProcessor : VstPluginAudioProcessorBase
    {

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            base.Process(inChannels, outChannels);
        }

    }
}
