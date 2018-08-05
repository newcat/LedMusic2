using LedMusic2.Reactive;
using System.Linq;
using System.Reflection;

namespace LedMusic2.Outputs
{
    public class OutputManager : ReactiveObject
    {
        
        public ReactiveCollection<OutputBase> Outputs { get; }
            = new ReactiveCollection<OutputBase>();

        public ReactiveCollection<OutputType> OutputTypes { get; }
            = new ReactiveCollection<OutputType>();

        public void FillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs.OutputModels" && !t.IsAbstract);
            var outputs = outputClasses.Where(t => t.GetCustomAttribute<OutputAttribute>() != null);

            foreach (var t in outputs)
                OutputTypes.Add(new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t));

        }

        public void AddOutput(OutputBase o)
        {
            Outputs.Add(o);
        }

    }
}
