﻿using System;

namespace MVC.Runtime.Injectable.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string Name { get; set; }
    }
}