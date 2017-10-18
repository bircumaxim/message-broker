using Data.Mappers.Messages;
using Data.Mappers.Persistence;
using Data.Models;
using Domain;
using Domain.Exhcanges;
using Domain.Infrastructure.Mapping;
using Domain.Models;
using Messages.Payload;
using Messages.ServerInfo;
using Messages.Subscribe;
using Persistence.Models;

namespace Data.Mappers
{
    public class MappersPull
    {
        private static MappersPull _instance;
        public static MappersPull Instance => _instance ?? (_instance = new MappersPull());
     
        private readonly Pull<object> _pull;

        private MappersPull()
        {
            _pull = new ObjectPullBuilder<object>()
                .For(typeof(IMapper<Message, PayloadMessage>)).Use(typeof(MessageToPayloadMessageMapper))
                .For(typeof(IMapper<PayloadRouteMessage, RouteMessage>)).Use(typeof(PayloadRouteMessageToRouteMessageMapper))
                .For(typeof(IMapper<RouteMessage, PayloadRouteMessage>)).Use(typeof(RouteMessageToPayloadRouteMessageMapper))
                .For(typeof(IMapper<ServerGeneralInfo, ServerGeneralInfoResponse>)).Use(typeof(ServerGeneralInfoResponseMapper))
                .For(typeof(IMapper<PersistenceExchange, Exchange>)).Use(typeof(PersistenceExchangeToExchangeMapper))
                .For(typeof(IMapper<PersistenceQueue<RouteMessage>, Queue<RouteMessage>>)).Use(typeof(PersistenceQueueToQueueMapper))
                .For(typeof(IMapper<PersistenceServerGeneralInfo, ServerGeneralInfo>)).Use(typeof(PersitenceServerGeneralInfoToServerGeneralInfoMapper))
                .For(typeof(IMapper<RouteMessage, PersistenceMessage>)).Use(typeof(RouteMessageToPersistenceMessageMapper))
                .For(typeof(IMapper<SubscribeMessage, PersistenceSubscription>)).Use(typeof(SubscribeMessageToPersistenceSubscription))
                .Build();
        }

        public TR Map<TM, TR>(TM objecToMapp)
        {
            var mapper = (IMapper<TM, TR>) _pull.GetObject(typeof(IMapper<TM, TR>));
            return mapper.Map(objecToMapp);
        }
    }
}