using Domain.UseCases;
using Serialization;

namespace Data
{
    public static class MessageUseCaseFactory
    {
        public static IUseCase GetUseCaseForMessage(Message message)
        {
            switch (message.MessageTypeName)
            {
                
            }
            return null;
        }
    }
}