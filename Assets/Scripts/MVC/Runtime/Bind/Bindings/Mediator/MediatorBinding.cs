﻿using MVC.Runtime.ViewMediators.Mediator;

namespace MVC.Runtime.Bind.Bindings.Mediator
{
    public class MediatorBinding : Binding
    {
        public new virtual void To<TValueType>()
            where TValueType : IMVCMediator
        {
            Value = typeof(TValueType);
        }
    }
}