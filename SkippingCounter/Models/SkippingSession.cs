using System;
using System.Collections.Generic;
using System.Numerics;
using Humanizer;

namespace SkippingCounter.Models
{
    public record SkippingSession(DateTimeOffset Start, DateTimeOffset End, IList<(TimeSpan Time, Vector3 Force)> Jumps) : IIdentifiable
    {
        public string GetID() => Start.ToString("yyyy-MM-ddThh-mm-ss");

        public TimeSpan Duration => End.Subtract(Start);

        public string FormattedDuration => Duration.Humanize();
    }
}
