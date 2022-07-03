﻿using System;
using System.Collections.Generic;
using System.Linq;
using MVC.Runtime.Attributes;
using MVC.Runtime.Bind.Binders;
using MVC.Runtime.Contexts;
using MVC.Runtime.Controller.Sequencer;
using MVC.Runtime.Signals;
using UnityEngine;

namespace MVC.Runtime.Controller.Binder
{
    [HideInModelViewer]
    public class CommandBinder : Binder<CommandBinding>, ICommandBinder
    {
        private Dictionary<Type, List<ICommandBody>> _commandPool;
        private List<CommandSequencer> _sequencePool;
        private List<CommandSequencer> _activeSequenceList;

        internal IContext Context;
        
        public CommandBinder()
        {
            _commandPool = new Dictionary<Type, List<ICommandBody>>();
            
            _sequencePool = new List<CommandSequencer>();
            _activeSequenceList = new List<CommandSequencer>();
        }
        
        public virtual CommandBinding Bind<TSignal>(TSignal key)
            where TSignal : ISignalBody
        {
            key.InternalCallback = null;
            key.InternalCallback += SignalDispatcher;
            var binding = base.Bind(key);
            binding.SetContext(Context);
            return binding;
        }

        private void SignalDispatcher(ISignalBody signal, params object[] commandParameters)
        {
            var binding = GetBinding(signal) as ICommandBinding;
            if (binding == null)
                return;

            var sequence = GetAvailableSequence();
            sequence.Initialize(binding, this, commandParameters);
            sequence.SequenceFinished += sequencer =>
            {
                ReturnSequenceToPool(sequencer);
            };
            _activeSequenceList.Add(sequence);
            sequence.RunCommands();
        }

        public virtual void ReleaseCommand(ICommandBody commandBody, params object[] commandParameters)
        {
            var sequence = GetActiveSequence(commandBody);
            if (sequence == null)
            {
                Debug.LogError("RELEASE FAILED! - Command Sequence not found! \n CommandType: " + commandBody.GetType().Name);
                return;
            }
            
            sequence.ReleaseCommand(commandBody, commandParameters);
        }

        public virtual void Jump<TCommandType>(ICommandBody commandBody, params object[] commandParameters)
            where TCommandType : ICommandBody 
        {
            var sequence = GetActiveSequence(commandBody);
            if (sequence == null)
            {
                Debug.LogError("JUMP FAILED! - Command Sequence not found! \n CommandType: " + commandBody.GetType().Name);
                return;
            }
            
            sequence.JumpCommand<TCommandType>(commandBody, commandParameters);
        }
        
        public virtual void StopCommand(ICommandBody commandBody)
        {
            var sequence = GetActiveSequence(commandBody);
            if (sequence == null)
            {
                Debug.LogError("COMMAND STOP FAILED! - Command Sequence not found! \n CommandType: " + commandBody.GetType().Name);
                return;
            }

            sequence.Stop();
        }
        
        #region SequencePool

        private CommandSequencer GetAvailableSequence()
        {
            var availableSequencer = _sequencePool.Count != 0 ? _sequencePool[0] : null;

            if (availableSequencer == null)
                availableSequencer = new CommandSequencer();

            availableSequencer.SequenceFinished = null;
            
            return availableSequencer;
        }
        
        private void ReturnSequenceToPool(CommandSequencer commandSequencer)
        {
            commandSequencer.Dispose();
            
            _activeSequenceList.Remove(commandSequencer);
            _sequencePool.Add(commandSequencer);
        }

        private CommandSequencer GetActiveSequence(ICommandBody commandBody)
        {
            var sequence =
                _activeSequenceList.FirstOrDefault(x => x.currentCommand != null && x.currentCommand == commandBody);
            return sequence;
        }
        
        #endregion

        #region CommandPool

        internal ICommandBody GetCommand(Type commandType)
        {
            ICommandBody command = null;
            if (_commandPool.ContainsKey(commandType) && _commandPool[commandType].Count != 0)
            {
                command = _commandPool[commandType][0];
                _commandPool[commandType].Remove(command);
                return command;
            }
            
            command = (ICommandBody) Activator.CreateInstance(commandType);
            return command;
        }

        internal void ReturnCommandToPool(ICommandBody commandBody)
        {
            commandBody.Clean();
            var commandType = commandBody.GetType();
            
            if(!_commandPool.ContainsKey(commandType))
                _commandPool.Add(commandType, new List<ICommandBody>());

            _commandPool[commandType].Add(commandBody);
        }

        #endregion
    }
}