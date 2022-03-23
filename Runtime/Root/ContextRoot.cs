﻿using MVC.Runtime.Contexts;
using UnityEngine;

namespace MVC.Runtime.Root
{
    public class ContextRoot<TContextType> : RootBase, IContextRoot
        where TContextType : IContext, new()
    {
        public int initializeOrder;
        protected RootsManager _rootsManager;

        protected TContextType _context
        {
            get
            {
                return (TContextType) Context;
            }
            set
            {
                Context = value;
            }
        }

        #region UnityMethods

        private void Awake()
        {
            if(_context != null)
                return;
            
            _rootsManager = RootsManager.Instance;
            CreateContext();
            _rootsManager.RegisterContext(this);
        }

        private void Start()
        {
            _rootsManager.StartContexts();
        }

        private void OnDestroy()
        {
            DestroyContext();
        }

        #endregion

        private void CreateContext()
        {
            BeforeCreateContext();
            
            _context = new TContextType();
            _context.Initialize(gameObject, initializeOrder, _rootsManager.injectionBinderCrossContext);
        }
        
        public virtual void StartContext()
        {
            AfterCreateBeforeStartContext();

            _context.Start();
            _context.InjectAllInstances();
            _context.ExecutePostConstructMethods();
                
            AfterStarBeforeLaunchContext();
        }
        
        public virtual void DestroyContext()
        {
            _rootsManager.UnRegisterContext(this);
            _context.DestroyContext();
        } 
            
        public IContext GetContext()
        {
            return _context;
        }
        
        private void BeforeCreateContext(){}

        private void AfterCreateBeforeStartContext(){}

        private void AfterStarBeforeLaunchContext(){}
    }
}