﻿using System;
using Data.Events;
using Data.Mappers;
using Data.Mappers.Messages;
using Domain.Models;
using Domain.UseCases;
using log4net;
using Messages.Payload;

namespace Data
{
    public class UseCaseFactory
    {
        private readonly PayloadRouteMessageMapper _payloadRouteMessageMapper;
        private readonly PayloadRequestMessageMapper _payloadRequestMessageMapper;
        private readonly ILog _logger;
        private readonly Persistence _persistence;
        private readonly Transport _transport;

        public UseCaseFactory(Persistence persistence, Transport transport)
        {
            _payloadRequestMessageMapper = new PayloadRequestMessageMapper();
            _payloadRouteMessageMapper = new PayloadRouteMessageMapper();
            _logger = LogManager.GetLogger(GetType());
            _persistence = persistence;
            _transport = transport;
        }

        public IUseCase GetUseCaseFor(RemoteApplicationMessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == typeof(PayloadRouteMessage).Name)
            {
                var message = _payloadRouteMessageMapper.Map(args.Message as PayloadRouteMessage);
                return CreateRouteUseCase(message);
            }
            if (args.Message.MessageTypeName == typeof(PayloadRequestMessage).Name)
            {
                var domainRequestMessage = _payloadRequestMessageMapper.Map(args.Message as PayloadRequestMessage);
                domainRequestMessage.ReceiverName = args.Application.Name;
                return CreateGetMessageUseCase(domainRequestMessage);
            }
            return null;
        }

        private IUseCase CreateRouteUseCase(RouteMessage routeMessage)
        {
            RouteMessageUseCase routeMessageUseCase = null;
            try
            {
                routeMessageUseCase = new RouteMessageUseCase(routeMessage, _persistence);
            }
            catch (Exception ex)
            {
                _logger.Error("Smth went wrong during executing RouteMessageUseCase", ex);
            }
            return routeMessageUseCase;
        }
        
        private IUseCase CreateGetMessageUseCase(MessageRequest message)
        {
            GetMessageUseCase getMessageUseCase = null;
            try
            {
                getMessageUseCase = new GetMessageUseCase(message, _transport, _persistence);
            }
            catch (Exception ex)
            {
                _logger.Error("Smth went wrong during executing GetMessageUseCase", ex);
            }
            return getMessageUseCase;
        }
    }
}