using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LedMusic2.Reactive;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using LedMusic2Test.Reactive;

namespace LedMusic2Test.Reactive
{
    [TestClass]
    public class ReactiveTests
    {
        [TestMethod]
        public void AllTypesImplementJsonConstructor()
        {

            var assembly = Assembly.Load("LedMusic2");
            var types = assembly.GetTypes()
                .Where(t => typeof(ReactiveObject).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract);

            var failed = false;

            foreach (var t in types)
            {
                if (t.GetConstructor(new Type[] { typeof(JToken) }) == null)
                {
                    Console.WriteLine(t);
                    failed = true;
                }
            }

            Assert.IsFalse(failed);

        }

        [TestMethod]
        public void PrimitiveBinding()
        {
            var reactive = new ReactivePrimitive<int>(1);
            var bound = reactive.Bind();
            Assert.IsNotNull(reactive.GetFullState());
            Assert.IsNotNull(bound.GetFullState());
            reactive.Set(2);
            Assert.IsNotNull(reactive.GetStateUpdates());
            Assert.IsNotNull(bound.GetStateUpdates());
        }

        [TestMethod]
        public void ObjectBinding()
        {
            var reactive = new ReactiveTestObject();
            var bound = reactive.Bind();
            Assert.IsNotNull(reactive.GetFullState());
            Assert.IsNotNull(bound.GetFullState());
            reactive.TestValue.Set(2);
            Assert.IsNotNull(reactive.GetStateUpdates());
            Assert.IsNotNull(bound.GetStateUpdates());
            var bound2 = reactive.Bind();
            reactive.TestValue.Set(3);
            Assert.IsNotNull(reactive.GetStateUpdates());
            Assert.IsNotNull(bound.GetStateUpdates());
            Assert.IsNotNull(bound2.GetStateUpdates());
        }

        [TestMethod]
        public void CollectionBinding()
        {
            var reactive = new ReactiveCollection<ReactiveListItem<int>>();
            var bound = reactive.Bind();
            Assert.IsNotNull(reactive.GetFullState());
            Assert.IsNotNull(bound.GetFullState());
            reactive.Add(new ReactiveListItem<int>(5));
            reactive.Add(new ReactiveListItem<int>(6));
            Assert.IsNotNull(reactive.GetStateUpdates());
            Assert.IsNotNull(bound.GetStateUpdates());
            var bound2 = reactive.Bind();
            reactive.Add(new ReactiveListItem<int>(2));
            reactive.Add(new ReactiveListItem<int>(1));
            reactive.Remove(reactive[0]);
            var stu = bound2.GetStateUpdates();
            Assert.IsNotNull(stu);
            stu.Print(0);
            Assert.IsNotNull(reactive.GetStateUpdates());
            Assert.IsNotNull(bound.GetStateUpdates());
        }

    }
}
