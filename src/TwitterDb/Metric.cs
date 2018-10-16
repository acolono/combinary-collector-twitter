using System;
using System.Collections.Generic;

namespace TwitterDb
{
    public partial class Metric
    {
        public Metric()
        {
            Stream = new HashSet<Stream>();
        }

        public long Id { get; set; }
        public long TweetId { get; set; }
        public int? FavouritesCount { get; set; }
        public int? RetweetCount { get; set; }
        public DateTimeOffset Ts { get; set; }

        public Tweet Tweet { get; set; }
        public ICollection<Stream> Stream { get; set; }
    }
}
