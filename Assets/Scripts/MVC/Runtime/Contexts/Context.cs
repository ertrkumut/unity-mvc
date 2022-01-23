﻿using MVC.Runtime.Injectable;
using MVC.Runtime.Injectable.Binders;
using UnityEngine;

namespace MVC.Runtime.Contexts
{
    public class Context : IContext
    {
        protected GameObject _gameObject;

        public MediatorBinder MediatorBinder { get; set; }
        public InjectionBinder InjectionBinder { get; set; }

        public int InitializeOrder { get; set; }

        public void Initialize(GameObject contextGameObject, int initializeOrder)
        {
            _gameObject = contextGameObject;
            InitializeOrder = initializeOrder;
        }

        public void Start()
        {
            CoreBindings();
            MapBindings();
            PostBindings();
        }

        private void CoreBindings()
        {
            MediatorBinder = new MediatorBinder();
            InjectionBinder = new InjectionBinder();
        }

        public virtual void MapBindings()
        {
            
        }

        public virtual void PostBindings()
        {
            
        }
        
        public void Launch()
        {
        }
    }
}