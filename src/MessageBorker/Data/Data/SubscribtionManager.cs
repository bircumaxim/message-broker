﻿using System.Collections.Generic;
using Data.Mappers;
using Domain.Models;
using Messages.Payload;
using Messages.Subscribe;

namespace Data
{
    public class SubscribtionManager
    {
        private readonly RemoteApplicationManager _remoteApplicationManager;
        private readonly Dictionary<string, List<string>> _subscriptions;
        private readonly Persistence _persistence;

        public SubscribtionManager(RemoteApplicationManager remoteApplicationManager, Persistence persistence)
        {
            _remoteApplicationManager = remoteApplicationManager;
            _persistence = persistence;
            _subscriptions = new Dictionary<string, List<string>>();
        }

        public void RemoveSubscription(string applicationName)
        {
            lock (_subscriptions)
            {
                foreach (var subscription in _subscriptions)
                {
                    if (subscription.Value.Contains(applicationName))
                    {
                        subscription.Value.Remove(applicationName);
                    }
                }
            }
        }
        
        public void AddSubscrption(string queueName, string applicationName)
        {
            lock (_subscriptions)
            {
                if (_subscriptions.ContainsKey(queueName))
                {
                    _subscriptions[queueName].Add(applicationName);
                }
                else
                {
                    _subscriptions.Add(queueName, new List<string> {applicationName});
                }
                SendSubscribeSuccessMessage(applicationName);
            }
        }

        private void SendSubscribeSuccessMessage(string remoteApplicationName)
        {
            _remoteApplicationManager.SendMessage(remoteApplicationName, new SubscribeSuccessMessage());
        }

        public void NotifySubscribers(string queueName)
        {
            lock (_subscriptions)
            {
                List<string> applicationNames;
                if (_subscriptions.TryGetValue(queueName, out applicationNames))
                {
                    var message = _persistence.GetMessageFromQueueWithName(queueName);
                    while (message != null)
                    {
                        var payloadMessage = MappersPull.Instance.Map<Message, PayloadMessage>(message);
                        applicationNames.ForEach(app => _remoteApplicationManager.SendMessage(app, payloadMessage));
                        message = _persistence.GetMessageFromQueueWithName(queueName);
                    }
                }
            }
        }
    }
}