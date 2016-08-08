using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Hawk.Etw
{
    [EventSource(Name = "Thinktecture-IdentityModel-Hawk-Etw-HawkEventSource")]
    public class HawkEventSource : EventSource
    {
        private static readonly Lazy<HawkEventSource> source = new Lazy<HawkEventSource>(() => new HawkEventSource());

        private HawkEventSource() { }

        public static HawkEventSource Log
        {
            get
            {
                return source.Value;
            }
        }

        [Event(1, Keywords = Keywords.Common, Level = EventLevel.Verbose)]
        public void Debug(string Message)
        {
            if (this.IsEnabled()) this.WriteEvent(1, Message);
        }

        [Event(2, Keywords = Keywords.Common, Level = EventLevel.Informational)]
        public void NormalizedTimestamp(string NormalizedFormat)
        {
            if (this.IsEnabled()) this.WriteEvent(2, NormalizedFormat);
        }

        [Event(3, Keywords = Keywords.Common, Level = EventLevel.Informational)]
        public void UnparsedArtifact(string Unparsed)
        {
            if (this.IsEnabled()) this.WriteEvent(3, Unparsed);
        }

        [Event(4, Keywords = Keywords.Common, Level = EventLevel.Informational)]
        public void NormalizedBody(string NormalizedFormat)
        {
            if (this.IsEnabled()) this.WriteEvent(4, NormalizedFormat);
        }

        [Event(5, Keywords = Keywords.Common, Level = EventLevel.Informational)]
        public void StaleTimestamp(string Age, string ShelfLife)
        {
            if (this.IsEnabled()) this.WriteEvent(5, Age, ShelfLife);
        }

        [Event(6, Keywords = Keywords.Common, Level = EventLevel.Informational)]
        public void NormalizedRequest(string NormalizedFormat)
        {
            if (this.IsEnabled()) this.WriteEvent(6, NormalizedFormat);
        }

        [Event(7, Keywords = Keywords.Common, Level = EventLevel.Error)]
        public void Exception(string Message)
        {
            if (this.IsEnabled()) this.WriteEvent(7, Message);
        }

        [Event(8, Keywords = Keywords.Client, Level = EventLevel.Informational)]
        public void TimestampMismatch(int CompensatorySeconds)
        {
            if (this.IsEnabled()) this.WriteEvent(8, CompensatorySeconds);
        }

        [Event(9, Keywords = Keywords.Client, Level = EventLevel.Informational)]
        public void ServerResponse(int StatusCode, string Body, string AuthorizationHeader)
        {
            if (this.IsEnabled()) this.WriteEvent(9, StatusCode, Body, AuthorizationHeader);
        }

        [Event(10, Keywords = Keywords.Common, Level = EventLevel.Warning)]
        public void NonceReplay(string Nonce, string LastUsedBy)
        {
            if (this.IsEnabled()) this.WriteEvent(10, Nonce, LastUsedBy);
        }

        public class Keywords
        {
            public const EventKeywords Common = (EventKeywords)1;
            public const EventKeywords Middleware = (EventKeywords)2;
            public const EventKeywords Client = (EventKeywords)4;
        }
    }
}
