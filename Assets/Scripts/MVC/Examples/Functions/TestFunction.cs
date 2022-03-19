﻿using MVC.Runtime.Function;
using MVC.Runtime.Function.ReturnableFunctions;
using MVC.Runtime.Function.VoidFunctions;
using UnityEngine;

namespace MVC.Examples.Functions
{
    public class TestFunction : FunctionVoid
    {
        public override void Execute()
        {
            Debug.Log("Test Function Executed");
        }
    }
    
    public class MathFunction : FunctionReturn<float, int, int>
    {
        public override float Execute(int param1, int param2)
        {
            var result =  param1 + param2;
            Debug.Log("Math Function Executed! - Result: " + result);
            return result;
        }
    }
}