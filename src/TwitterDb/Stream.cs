using System;
using System.Collections.Generic;

namespace TwitterDb
{
    public partial class Stream
    {
        public long Id { get; set; }
        public long TweetId { get; set; }
        public long MetricId { get; set; }
        public DateTimeOffset Ts { get; set; }

        public Metric Metric { get; set; }
        public Tweet Tweet { get; set; }
    }
}
