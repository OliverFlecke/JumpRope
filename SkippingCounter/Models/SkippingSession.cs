using System;
using System.Collections.Generic;
using System.Numerics;

namespace SkippingCounter.Models
{
    public record SkippingSession(DateTimeOffset Start, DateTimeOffset End, IList<(TimeSpan Time, Vector3 Force)> Jumps) : IIdentifiable
    {
        public string GetID() => Start.ToString("yyyy-MM-ddThh-mm-ss");
    }
}
