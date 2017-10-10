using System;
using Data.Events;
using Data.Mappers;
using Domain.Messages;
using Domain.UseCases;
using log4net;
using Messages;
using Messages.Payload;

namespace Data
{
    public class UseCaseFactory
    {
        private readonly PayloadMessageMapper _payloadMessageMapper;
        private readonly PayloadRequestMapper _payloadRequestMapper;
        private readonly ILog _logger;
        private readonly Persistence _persistence;
        private readonly Transport _transport;

        public UseCaseFactory(Persistence persistence, Transport transport)
        {
            _payloadRequestMapper = new PayloadRequestMapper();
            _payloadMessageMapper = new PayloadMessageMapper();
            _logger = LogManager.GetLogger(GetType());
            _persistence = persistence;
            _transport = transport;
        }

        public IUseCase GetUseCaseFor(RemoteApplicationMessageReceivedEventArgs args)
        {
            if (args.Message.MessageTypeName == typeof(PayloadMessage).Name)
            {
                var domainMessage = _payloadMessageMapper.Map(args.Message as PayloadMessage);
                return CreateRouteUseCase(domainMessage);
            }
            if (args.Message.MessageTypeName == typeof(PayloadRequest).Name)
            {
                var domainRequestMessage = _payloadRequestMapper.Map(args.Message as PayloadRequest);
                domainRequestMessage.ReceiverName = args.Application.Name;
                return CreateGetMessageUseCase(domainRequestMessage);
            }
            return null;
        }

        private IUseCase CreateRouteUseCase(Message message)
        {
            RouteMessageUseCase routeMessageUseCase = null;
            try
            {
                routeMessageUseCase = new RouteMessageUseCase(message, _persistence);
            }
            catch (Exception ex)
            {
                _logger.Error("Smth went wrong during executing RouteMessageUseCase");
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
                _logger.Error("Smth went wrong during executing GetMessageUseCase");
            }
            return getMessageUseCase;
        }
    }
}