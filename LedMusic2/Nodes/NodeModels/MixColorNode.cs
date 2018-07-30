using LedMusic2.LedColors;
using LedMusic2.NodeEditor;
using System;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Mix Color", NodeCategory.COLOR)]
    class MixColorNode : NodeBase
    {

        private float factor = 0.5f;
        private NodeOption optMode;

        public MixColorNode() : base()
        {

            AddInput("Factor", 0.5);
            AddInput<LedColor[]>("Color 1");
            AddInput<LedColor[]>("Color 2");
            AddOutput<LedColor[]>("Output");

            optMode = new NodeOption(NodeOptionType.SELECTION, "Mode");
            foreach (string s in new string[] { "Mix", "Add", "Multiply", "Subtract", "Screen", "Divide", "Difference", "Darken", "Lighten",
                                                "Overlay", "Dodge", "Burn", "Hue", "Saturation", "Value", "Color", "Soft Light", "Linear Light"})
            {
                optMode.Options.Add(s);
            }
            optMode.Value.Set("Overlay");
            Options.Add(optMode);

            Calculate();

        }

        public override bool Calculate()
        {

            LedColor[] colorsA = (LedColor[])Inputs.GetNodeInterface("Color 1").GetValue();
            LedColor[] colorsB = (LedColor[])Inputs.GetNodeInterface("Color 2").GetValue();

            if (colorsA == null || colorsB == null)
                return false;

            factor = (float)(double)Inputs.GetNodeInterface("Factor").GetValue();
            string mode = (string)optMode.Value.Get();

            int length = Math.Max(colorsA.Length, colorsB.Length);
            LedColor[] result = new LedColor[length];

            for (int i = 0; i < length; i++)
            {

                LedColor a = i < colorsA.Length && colorsA[i] != null ? colorsA[i] : new LedColorRGB(0, 0, 0);
                LedColor b = i < colorsB.Length && colorsB[i] != null ? colorsB[i] : new LedColorRGB(0, 0, 0);

                switch (mode)
                {
                    case "Mix":
                        result[i] = a.Mix(b, factor);
                        break;
                    case "Add":
                        result[i] = a.Add(b, factor);
                        break;
                    case "Multiply":
                        result[i] = a.Multiply(b, factor);
                        break;
                    case "Subtract":
                        result[i] = a.Subtract(b, factor);
                        break;
                    case "Screen":
                        result[i] = a.Screen(b, factor);
                        break;
                    case "Divide":
                        result[i] = a.Divide(b, factor);
                        break;
                    case "Difference":
                        result[i] = a.Difference(b, factor);
                        break;
                    case "Darken":
                        result[i] = a.Darken(b, factor);
                        break;
                    case "Lighten":
                        result[i] = a.Lighten(b, factor);
                        break;
                    case "Overlay":
                        result[i] = a.Overlay(b, factor);
                        break;

                    default:
                        result[i] = a.Overlay(b, factor);
                        break;
                }

            }

            Outputs[0].SetValue(result);
            return true;

        }

    }
}
