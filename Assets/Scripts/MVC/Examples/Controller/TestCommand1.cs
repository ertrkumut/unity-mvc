﻿using MVC.Examples.Entity;
using MVC.Runtime.Controller;
using MVC.Runtime.Injectable.Attributes;
using UnityEngine;

namespace MVC.Examples.Controller
{
    public class TestCommand1 : Command
    {
        [Inject] private TestClass _testClass;

        [SignalParam] private int _intParam;
        [SignalParam] private int _stringParam;
        public override void Execute()
        {
            RetainCommand();
            Debug.Log("Command 1");
            
            ReleaseCommand();
        }
    }
    
    public class TestCommand2 : Command
    {
        [SignalParam] private int _intParam;
        [SignalParam] private int _stringParam;
        
        public override void Execute()
        {
            Debug.Log("Command 2");
        }
    }
}