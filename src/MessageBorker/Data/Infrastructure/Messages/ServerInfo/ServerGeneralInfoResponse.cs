﻿using System;
using Serialization;
using Serialization.Deserializer;
using Serialization.Serializer;

namespace Messages.ServerInfo
{
    public class ServerGeneralInfoResponse : Message
    {
        public DateTime ServerStartTime { get; set; }
        public int ConnectionsCount { get; set; }
        public int MessagesInQueue { get; set; }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.WriteDateTime(ServerStartTime);
            serializer.WriteInt32(ConnectionsCount);
            serializer.WriteInt32(MessagesInQueue);
        }

        public override void Deserialize(IDeserializer deserializer)
        {
            base.Deserialize(deserializer);
            ServerStartTime = deserializer.ReadDateTime();
            ConnectionsCount = deserializer.ReadInt32();
            MessagesInQueue = deserializer.ReadInt32();
        }
    }
}