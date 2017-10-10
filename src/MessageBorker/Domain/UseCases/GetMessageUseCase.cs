using System;
using Domain.GateWays;
using Domain.Messages;

namespace Domain.UseCases
{
    public class GetMessageUseCase : IUseCase
    {
        private readonly MessageRequest _messageRequest;
        private readonly IPersistenceGateWay _persistenceGateWay;
        private readonly ITransportGateWay _transportGateWay;

        public GetMessageUseCase(MessageRequest messageRequest, ITransportGateWay transportGateWay, IPersistenceGateWay persistenceGateWay)
        {
            _messageRequest = messageRequest;
            _transportGateWay = transportGateWay;
            _persistenceGateWay = persistenceGateWay;
            
            Validate();
        }
        
        public void Execute()
        {
            var message = _persistenceGateWay.GetMessageFromQueueWithName(_messageRequest.QueueName);
            var messageResponse = new MessageResponse
            {
                Payload = message.Payload, 
                ReceiverName = _messageRequest.ReceiverName
            };
            _transportGateWay.Send(messageResponse);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_messageRequest?.QueueName))
            {
                throw new NullReferenceException();
            }
        }
    }
}