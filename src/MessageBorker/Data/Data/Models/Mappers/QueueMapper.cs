﻿using Domain;
using Domain.Infrastructure.Mapping;
using Domain.Messages;

namespace Data.Models.Mappers
{
    public class QueueMapper : IMapper<QueueData<MessageData>, Queue<Message>>
    {
        public Queue<Message> Map(QueueData<MessageData> model)
        {
            return new Queue<Message> {Name = model.Name};
        }
    }
}