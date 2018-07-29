using LedMusic2.BrowserInterop;
using System.Linq;
using System.Reflection;

namespace LedMusic2.Outputs
{
    public class OutputManager
    {
        public SynchronizedCollection<OutputBase> Outputs { get; } = new SynchronizedCollection<OutputBase>();
        public SynchronizedCollection<OutputType> OutputTypes { get; } = new SynchronizedCollection<OutputType>();

        public void FillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs" && !t.IsAbstract);
            var outputs = outputClasses.Where(t => t.GetCustomAttribute<OutputAttribute>() != null);

            foreach (var t in outputs)
                OutputTypes.Add(new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t));

        }

    }
}
