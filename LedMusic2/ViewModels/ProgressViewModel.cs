using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.ViewModels
{
    public class ProgressViewModel : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public ReactivePrimitive<string> Description = new ReactivePrimitive<string>();
        public ReactivePrimitive<int> Progress = new ReactivePrimitive<int>(0);

        public ProgressViewModel(string description)
        {
            Description.Set(description);
        }

        public ProgressViewModel(JToken j)
        {
            LoadState(j);
        }

    }
}
