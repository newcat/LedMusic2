using Newtonsoft.Json.Linq;

namespace LedMusic2.Reactive
{
    public interface IReactiveCollection
    {
        void LoadFromJson(JToken j);
    }
}
