using LedMusic2.Outputs.OutputModels;
using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace LedMusic2.Outputs
{
    public class OutputManager : ReactiveObject
    {
        
        public ReactiveCollection<OutputBase> Outputs { get; }
            = new ReactiveCollection<OutputBase>();

        [IgnoreOnLoad]
        public ReactiveCollection<OutputType> OutputTypes { get; }
            = new ReactiveCollection<OutputType>();

        public OutputManager()
        {
            initialize();
            Outputs.Add(new DummyOutput());
        }

        public OutputManager(JToken j)
        {
            LoadState(j);
            initialize();
        }

        private void initialize()
        {
            fillOutputTypes();
            RegisterCommand("addOutput", (p) => addOutput(p));                
        }

        private void fillOutputTypes()
        {

            var outputClasses = Assembly.GetCallingAssembly().GetTypes().Where(t => t.Namespace == "LedMusic2.Outputs.OutputModels" && !t.IsAbstract);
            OutputTypes.Clear();
            OutputTypes.AddRange(
                outputClasses
                    .Where(t => t.GetCustomAttribute<OutputAttribute>() != null)
                    .Select(t => new OutputType(t.GetCustomAttribute<OutputAttribute>().Name, t))
            );

        }

        private void addOutput(JToken p)
        {
            var type = OutputTypes.FindById((string)p);
            if (type != null)
            {
                var instance = Activator.CreateInstance(type.T);
                Outputs.Add((OutputBase)instance);
            }
        }

    }
}
