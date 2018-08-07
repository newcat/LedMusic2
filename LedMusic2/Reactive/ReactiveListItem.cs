﻿using System;

namespace LedMusic2.Reactive
{
    public class ReactiveListItem<T> : ReactivePrimitive<T>, ICombinedReactive
    {

        public Guid Id { get; } = Guid.NewGuid();

        public ReactiveListItem(T value) : base(value) { }

    }
}