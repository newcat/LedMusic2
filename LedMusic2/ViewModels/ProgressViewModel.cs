using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.ViewModels
{
    public class ProgressViewModel : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; } = Guid.NewGuid();
        public override string ReactiveName => "ProgressViewModel";
        public ReactivePrimitive<string> Description = new ReactivePrimitive<string>("Name");
        public ReactivePrimitive<int> Progress = new ReactivePrimitive<int>("Progress", 0);

        public ProgressViewModel(string description)
        {
            Description.Set(description);
        }

    }
}
