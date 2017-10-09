using System;
using Data.Events;
using Data.Mappers;
using Domain.Messages;
using Domain.UseCases;
using log4net;
using Messages;

namespace Data
{
    public class UseCaseFactory
    {
        private readonly DefaultMessageMapper _defaultMessageMapper;
        private readonly DefaultMessageRequestMapper _defaultMessageRequestMapper;
        private readonly ILog _logger;
        private readonly Persistence _persistence;
        private readonly Transport _transport;

        public UseCaseFactory(Persistence persistence, Transport transport)
        {
            _defaultMessageRequestMapper = new DefaultMessageRequestMapper();
            _defaultMessageMapper = new DefaultMessageMapper();
            _logger = LogManager.GetLogger(GetType());
            _persistence = persistence;
            _transport = transport;
        }

        public IUseCase GetUseCaseFor(RemoteApplicationMessageReceivedEventArgs args)
        {
            switch (args.Message.MessageTypeName)
            {
                case "DefaultMessage":
                    var domainMessage = _defaultMessageMapper.Map(args.Message as DefaultMessage);
                    return CreateRouteUseCase(domainMessage);
                case "DefaultMessageRequest":
                    var domainRequestMessage = _defaultMessageRequestMapper.Map(args.Message as DefaultMessageRequest);
                    domainRequestMessage.ReceiverName = args.Application.Name;
                    return CreateGetMessageUseCase(domainRequestMessage);
                default:
                    return null;
            }
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
                _logger.Error("Smth went wrong during executing RouteMessageUseCase");
            }
            return getMessageUseCase;
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
    }
}