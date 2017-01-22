using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Spectrum", NodeCategory.INPUT)]
    class SpectrumNode : NodeBase
    {
        public SpectrumNode(Point initPosition) : base(initPosition)
        {

            _inputs.Add(new NodeInterface<double>("Lower Frequency", ConnectionType.NUMBER, this, true));
            _inputs.Add(new NodeInterface<double>("Upper Frequency", ConnectionType.NUMBER, this, true));

            _outputs.Add(new NodeInterface<double>("Value", ConnectionType.NUMBER, this, false));

        }

        public override bool Calculate()
        {

            var lowerFreq = ((NodeInterface<double>)_inputs.GetNodeInterface("Lower Frequency")).Value;
            var upperFreq = ((NodeInterface<double>)_inputs.GetNodeInterface("Upper Frequency")).Value;

            int lowerFrequencyIndex = SoundEngine.Instance.GetFftBandIndex((float)lowerFreq);
            int upperFrequencyIndex = SoundEngine.Instance.GetFftBandIndex((float)upperFreq);

            //Check if we already got a wave source and if not, return 0
            if (lowerFrequencyIndex == -1 || upperFrequencyIndex == -1)
            {
                setOutput(0);
                return false;
            }

            float[] fftData = SoundEngine.Instance.GetCurrentFftData();
            double totalLevel = 0;

            for (int i = lowerFrequencyIndex; i <= upperFrequencyIndex; i++)
            {
                totalLevel += fftData[i] * 9;
            }

            double level = totalLevel / (upperFrequencyIndex - lowerFrequencyIndex);
            if (double.IsNaN(level) || level < 0)
            {
                setOutput(0);
                return false;
            }

            setOutput(level);
            return true;

        }

        private void setOutput(double x)
        {
            ((NodeInterface<double>)_outputs[0]).SetValue(x);
        }

    }
}
