﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MVC.Runtime.Contexts;
using MVC.Runtime.Injectable.Attributes;
using MVC.Runtime.ViewMediators.Mediator;
using MVC.Runtime.ViewMediators.View;
using UnityEngine;

namespace MVC.Runtime.Injectable.Utils
{
    public static class InjectionExtensions
    {
        public static bool TryToInjectObject(object injectedObject)
        {
            return true;
        }

        #region InjectMediator

        public static bool TryToInjectMediator(this IContext context, IMediator mediator, IView view)
        {
            InjectMediatorFields(mediator, view, context);
            InjectMediatorProperties(mediator, view, context);
            
            mediator.OnRegister();
            return true;
        }

        private static void InjectMediatorFields(object mediator, object view, IContext context)
        {
            var viewType = view.GetType();

            var injectableFields = GetFieldInfoList(mediator);
            
            foreach (var injectableFieldInfo in injectableFields)
            {
                var fieldType = injectableFieldInfo.FieldType;

                if (fieldType == viewType)
                {
                    injectableFieldInfo.SetValue(mediator, view);
                }
                else
                {
                    SetInjectedValue(mediator, context, injectableFieldInfo);
                }
            }
        }
        
        private static void InjectMediatorProperties(object mediator, object view, IContext context)
        {
            var viewType = view.GetType();

            var injectablePropertyList = GetPropertyInfoList(mediator);
            
            foreach (var injectableProperty in injectablePropertyList)
            {
                var fieldType = injectableProperty.PropertyType;

                if (fieldType == viewType)
                {
                    injectableProperty.SetValue(mediator, view);
                }
                else
                {
                    SetInjectedValue(mediator, context, injectableProperty);
                }
            }
        }

        #endregion

        #region GetInjectable Fields-Properties

        private static List<FieldInfo> GetFieldInfoList(object instance)
        {
            var injectableTypes = instance.GetType().GetAllChildClasses();

            var injectableFields = new List<FieldInfo>();
            foreach (var injectableType in injectableTypes)
            {
                var fields = injectableType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => 
                        x.GetCustomAttributes(typeof(InjectAttribute)).ToList().Count != 0)
                    .ToList();

                injectableFields = injectableFields.Concat(fields).ToList();
            }

            return injectableFields;
        }

        private static List<PropertyInfo> GetPropertyInfoList(object instance)
        {
            var injectableTypes = instance.GetType().GetAllChildClasses();

            var injectableProperties = new List<PropertyInfo>();
            foreach (var injectableType in injectableTypes)
            {
                var properties = injectableType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => 
                        x.GetCustomAttributes(typeof(InjectAttribute)).ToList().Count != 0)
                    .ToList();

                injectableProperties = injectableProperties.Concat(properties).ToList();
            }

            return injectableProperties;
        }

        #endregion

        private static void SetInjectedValue(object objectInstance, IContext context, MemberInfo injectedMemberInfo)
        {
            Type injectionType = null;
            if (injectedMemberInfo.MemberType == MemberTypes.Field)
                injectionType = (injectedMemberInfo as FieldInfo).FieldType;
            else if(injectedMemberInfo.MemberType == MemberTypes.Property)
                injectionType = (injectedMemberInfo as PropertyInfo).PropertyType;
            
            var injectionValue = context.GetInjectedObject(injectedMemberInfo);
            if (injectionValue == null)
            {
                Debug.LogError("Injection Failed! There is no injected property in container! " +
                               "\n Instance Type: " + objectInstance.GetType().Name + 
                               "\n Injection Type: " + injectionType.Name);
            }
            
            if (injectedMemberInfo.MemberType == MemberTypes.Field)
                (injectedMemberInfo as FieldInfo).SetValue(objectInstance, injectionValue);
            else if(injectedMemberInfo.MemberType == MemberTypes.Property)
                (injectedMemberInfo as PropertyInfo).SetValue(objectInstance, injectionValue);
        }
        
        private static object GetInjectedObject(this IContext context, MemberInfo memberInfo)
        {
            Type injectionType = null;
            if (memberInfo.MemberType == MemberTypes.Field)
                injectionType = (memberInfo as FieldInfo).FieldType;
            else if(memberInfo.MemberType == MemberTypes.Property)
                injectionType = (memberInfo as PropertyInfo).PropertyType;
            
            var injectAttribute = memberInfo.GetCustomAttributes(typeof(InjectAttribute)).ToList()[0] as InjectAttribute;
            
            var injectionBinder = context.InjectionBinder;
            var crossContextInjectionBinder = context.CrossContextInjectionBinder;
            
            var injectionValue = injectionBinder.GetInstance(injectionType, injectAttribute.Name);
            if (injectionValue == null)
                injectionValue = crossContextInjectionBinder.GetInstance(injectionType, injectAttribute.Name);

            return injectionValue;
        }

        private static List<Type> GetAllChildClasses(this Type type)
        {
            var childTypes = Assembly
                .GetAssembly(type)
                .GetTypes()
                .Where(x => x.IsAssignableFrom(type) && !x.IsInterface)
                .ToList();

            return childTypes;
        }
    }
}