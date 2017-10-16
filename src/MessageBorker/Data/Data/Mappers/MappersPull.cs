using System;
using System.Collections.Generic;
using Data.Mappers.Messages;
using Data.Mappers.Persistence;
using Data.Models;
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
        private readonly Dictionary<Type, object> _mappersPull;
        private readonly Dictionary<Type, Type> _registeredMappers;

        private static MappersPull _instance;
        public static MappersPull Instance => _instance ?? (_instance = new MappersPull());

        private MappersPull()
        {
            _registeredMappers = new Dictionary<Type, Type>();
            _mappersPull = new Dictionary<Type, object>();

            RegisterMapper<Message, PayloadMessage>(typeof(MessageToPayloadMessageMapper));
            RegisterMapper<PayloadRouteMessage, RouteMessage>(typeof(PayloadRouteMessageToRouteMessageMapper));
            RegisterMapper<RouteMessage, PayloadRouteMessage>(typeof(RouteMessageToPayloadRouteMessageMapper));
            RegisterMapper<ServerGeneralInfo, ServerGeneralInfoResponse>(typeof(ServerGeneralInfoResponseMapper));
            RegisterMapper<PersistenceExchange, Exchange>(typeof(PersistenceExchangeToExchangeMapper));
            RegisterMapper<PersistenceMessage, Message>(typeof(PersistenceMessageToMessageMapper));
            RegisterMapper<PersistenceQueue<RouteMessage>, Queue<RouteMessage>>(typeof(PersistenceQueueToQueueMapper));
            RegisterMapper<PersistenceServerGeneralInfo, ServerGeneralInfo>(typeof(PersitenceServerGeneralInfoToServerGeneralInfoMapper));
            RegisterMapper<RouteMessage, PersistenceMessage>(typeof(RouteMessageToPersistenceMessageMapper));
            RegisterMapper<SubscribeMessage, PersistenceSubscription>(typeof(SubscribeMessageToPersistenceSubscription));
        }

        public TR Map<TM, TR>(TM objecToMapp)
        {
            var type = typeof(IMapper<TM, TR>);
            if (!_registeredMappers.ContainsKey(type))
            {
                throw new Exception($"There was not registered any mapper for such object {type}");
            }
            return GetMapperByType<TM, TR>().Map(objecToMapp);
        }

        private void RegisterMapper<TM,TR>(Type mapper)
        {
            _registeredMappers.Add(typeof(IMapper<TM, TR>), mapper);
        }
        
        private IMapper<TM, TR> GetMapperByType<TM, TR>()
        {
            if (!_mappersPull.ContainsKey(typeof(IMapper<TM, TR>)))
            {
                _mappersPull.Add(typeof(IMapper<TM, TR>), Activator.CreateInstance(_registeredMappers[typeof(IMapper<TM, TR>)]));
            }
            return _mappersPull[typeof(IMapper<TM, TR>)] as IMapper<TM, TR>;
        }
    }
}