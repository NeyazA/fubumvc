﻿using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.TestSupport;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class ChainExecutionFinished : MessageLogRecord
    {
        // TODO -- add a decent ToString()

        public Guid ChainId { get; set; }
        public EnvelopeToken Envelope { get; set; }
        public long ElapsedMilliseconds { get; set; }

        public override string ToString()
        {
            return "Chain finished for {0} at {1}".ToFormat(Envelope.Message, Envelope.ReceivedAt);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Finished Chain Execution"
            };
        }

        public override MessageTrack ToMessageTrack()
        {
            var track = MessageTrack.ForReceived(this, Envelope.CorrelationId);
            track.Type = track.FullName = MessageTrackType;

            return track;
        }
    }
}