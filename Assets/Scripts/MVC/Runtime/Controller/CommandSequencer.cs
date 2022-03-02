﻿using System;
using System.Collections.Generic;
using MVC.Runtime.Controller.Binder;
using MVC.Runtime.Injectable.Utils;

namespace MVC.Runtime.Controller
{
    internal class CommandSequencer
    {
        public Action<CommandSequencer> SequenceFinished;
        
        private CommandBinder _commandBinder;
        
        private ICommandBinding _commandBinding;
        private List<Type> _commands;

        private int _sequenceId;

        public void Initialize(ICommandBinding commandBinding, CommandBinder commandBinder)
        {
            _commandBinder = commandBinder;
            
            _sequenceId = 0;
            _commandBinding = commandBinding;
            _commands = _commandBinding.GetBindedCommands();
        }

        public void Clear()
        {
            _sequenceId = default;
            _commands = default;
            _commandBinding = default;
        }
        
        public void RunCommands()
        {
            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            var commandType = GetCurrentCommandType();
            var command = _commandBinder.GetCommand(commandType);
            
            InjectionExtensions.InjectCommand(command);
            
            ExecuteCommand(command);
            ReleaseCommand(command);
        }
        
        public void ReleaseCommand(ICommandBody command)
        {
            if(command.Retain)
                return;
            
            _commandBinder.ReturnCommandToPool(command);
            NextCommand(command);
        }
        
        private void NextCommand(ICommandBody command)
        {
            if(!command.Retain)
            {
                if(!IsSequenceCompleted())
                    ExecuteCommand();
                else
                    SequenceCompleted();
            }
        }

        private void ExecuteCommand(ICommandBody commandBody, params object[] parameters)
        {
            var commandType = commandBody.GetType();
            var executeMethodInfo = commandType.GetMethod("Execute");
            executeMethodInfo.Invoke(commandBody, parameters);
            _sequenceId++;
        }
        
        private void SequenceCompleted()
        {
            SequenceFinished?.Invoke(this);
        }

        private bool IsSequenceCompleted()
        {
            return _sequenceId >= _commands.Count;
        }
        
        private Type GetCurrentCommandType()
        {
            return _commands[_sequenceId];
        }
    }
}