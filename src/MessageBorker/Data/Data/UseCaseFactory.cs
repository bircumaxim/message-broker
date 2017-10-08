using System;
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
        private readonly ILog _logger;
        private readonly Persistence _persistence;

        public UseCaseFactory(Persistence persistence)
        {
            _defaultMessageMapper = new DefaultMessageMapper();
            _logger = LogManager.GetLogger(GetType());
            _persistence = persistence;
        }

        public IUseCase GetUseCaseFor(Serialization.Message message)
        {
            switch (message.MessageTypeName)
            {
                case "DefaultMessage":
                    var domainMessage = _defaultMessageMapper.Map(message as DefaultMessage);
                    return CreateRouteUseCase(domainMessage);
                case "PingMessage":
                    return null;
                default:
                    return null;
            }
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