using LedMusic2.BrowserInterop;
using System.Linq;
using System.Reflection;

namespace LedMusic2.Outputs
{
    public class OutputManager : ReactiveObject
    {

        public override string ReactiveName => "OutputManager";
        public ReactiveCollection<OutputBase> Outputs { get; } = new ReactiveCollection<OutputBase>("Outputs");
        public ReactiveCollection<OutputType> OutputTypes { get; } = new ReactiveCollection<OutputType>("OutputType");

        public void FillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs" && !t.IsAbstract);
            var outputs = outputClasses.Where(t => t.GetCustomAttribute<OutputAttribute>() != null);

            foreach (var t in outputs)
                OutputTypes.Add(new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t));

        }

    }
}
