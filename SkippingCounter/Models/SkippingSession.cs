using System;
using System.Collections.Generic;

namespace SkippingCounter.Models
{
    public record SkippingSession(DateTimeOffset Start, DateTimeOffset End, IList<TimeSpan> Jumps) : IIdentifiable
    {
        public string GetID() => Start.ToString("o");
    }
}
