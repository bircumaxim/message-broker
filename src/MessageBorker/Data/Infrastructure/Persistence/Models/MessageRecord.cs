using System;
using System.ComponentModel;

namespace Persistence.Models
{
    public class MessageRecord
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public DateTime RecordDate { get; set; }

        public MessageRecord(string messageId)
        {
            MessageId = messageId;
            RecordDate = DateTime.Now;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(MessageId))
            {
                //TODO log here exception.
                throw new InvalidEnumArgumentException("Message id should be specified");
            }
        }
    }
}