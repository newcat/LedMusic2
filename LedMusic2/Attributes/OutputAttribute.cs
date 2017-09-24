﻿using System;

namespace LedMusic2.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OutputAttribute : Attribute
    {

        public string Name { get; private set; }

        public OutputAttribute(string name)
        {
            Name = name;
        }

    }
}
