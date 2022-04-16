﻿using MVC.Runtime.Contexts;

namespace MVC.Runtime.Root
{
    public interface IContextRoot
    {
        void StartContext();
        IContext GetContext();

        void Launch(bool forceToLaunch = false);
    }
}