using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LedMusic2.Reactive
{
    public interface IReactiveCollection : IReactive
    {
        void LoadFromJson(JToken j);
    }

    public interface IReactiveCollection<T> : IReactiveCollection, IEnumerable<T>
    {
        T FindById(string id);
        T FindById(Guid id);
    }

}
