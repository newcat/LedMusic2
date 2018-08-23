using LedMusic2.Reactive;
using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeOptions;
using System;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Mix Color", NodeCategory.COLOR)]
    class MixColorNode : NodeBase
    {

        private float factor = 0.5f;
        private SelectOption<ReactiveListItem<string>> optMode = new SelectOption<ReactiveListItem<string>>("Mode");

        public MixColorNode() : base()
        {

            AddInput("Factor", 0.5);
            AddInput<LedColorArray>("Color 1");
            AddInput<LedColorArray>("Color 2");
            AddOutput<LedColorArray>("Output");

            var modeOptions = new ReactiveCollection<ReactiveListItem<string>>();
            foreach (string s in new string[] { "Mix", "Add", "Multiply", "Subtract", "Screen", "Divide", "Difference", "Darken", "Lighten",
                                                "Overlay", "Dodge", "Burn", "Hue", "Saturation", "Value", "Color", "Soft Light", "Linear Light"})
            {
                modeOptions.Add(new ReactiveListItem<string>(s));
            }
            optMode.SetOptions(modeOptions);
            optMode.SelectedId.Set(modeOptions[0].Id.ToString());
            Options.Add(optMode);

            Calculate();

        }

        public MixColorNode(JToken j) : this()
        {
            LoadState(j);
        }

        public override bool Calculate()
        {

            LedColorArray colorsA = Inputs.GetNodeInterface<LedColorArray>("Color 1").Value;
            LedColorArray colorsB = Inputs.GetNodeInterface<LedColorArray>("Color 2").Value;

            if (colorsA == null || colorsB == null)
                return false;

            factor = (float)Inputs.GetNodeInterface<double>("Factor").Value;
            string mode = optMode.Value.Get();

            int length = Math.Max(colorsA.Count, colorsB.Count);
            LedColorArray result = new LedColorArray(new LedColor[length]);

            for (int i = 0; i < length; i++)
            {

                LedColor a = i < colorsA.Count && colorsA[i] != null ? colorsA[i] : new LedColorRGB(0, 0, 0);
                LedColor b = i < colorsB.Count && colorsB[i] != null ? colorsB[i] : new LedColorRGB(0, 0, 0);

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
