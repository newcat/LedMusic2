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
            Initialize();
            RegisterCommand("addOutput", (p) => addOutput(p));
        }

        protected override void Initialize()
        {
            base.Initialize();
            fillOutputTypes();
            if (Outputs.Count == 0)
                Outputs.Add(new DummyOutput());
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
