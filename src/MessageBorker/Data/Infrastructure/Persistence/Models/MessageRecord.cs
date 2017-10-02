using System;
using System.ComponentModel;
using log4net;

namespace Persistence.Models
{
    public class MessageRecord
    {
        private readonly ILog _logger;
        public int Id { get; set; }
        public string MessageId { get; set; }
        public DateTime RecordDate { get; set; }

        public MessageRecord(string messageId)
        {
            _logger = LogManager.GetLogger(GetType());
            MessageId = messageId;
            RecordDate = DateTime.Now;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(MessageId))
            {
                _logger.Error("Message id should be specified");
                throw new InvalidEnumArgumentException("Message id should be specified");
            }
        }
    }
}