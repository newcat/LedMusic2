﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LedMusic2.Reactive;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LedMusic2Test
{
    [TestClass]
    public class Reactive
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
    }
}
