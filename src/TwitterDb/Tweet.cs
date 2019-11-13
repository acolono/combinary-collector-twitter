using System;
using System.Collections.Generic;

namespace TwitterDb
{
    public partial class Tweet
    {
        public Tweet()
        {
            Metric = new HashSet<Metric>();
            Stream = new HashSet<Stream>();
        }

        public long Id { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public string MediaPhoto { get; set; }
        public string[] Hashtags { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Place { get; set; }
        public long? InReplyToTweetId { get; set; }
        public long? QuotedTweetId { get; set; }
        public long? RetweetedTweetId { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public bool Deleted { get; set; }
        public bool IsRetweet { get; set; }
        public DateTimeOffset? Ts { get; set; }

        public User User { get; set; }
        public ICollection<Metric> Metric { get; set; }
        public ICollection<Stream> Stream { get; set; }
    }
}
