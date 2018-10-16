using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Config
{
    public interface ITwitterConfig
    {
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string AccessToken { get; set; }
        string AccessTokenSecret { get; set; }
    }

    public interface ITwitterStreamConfig : ITwitterConfig
    {
        string Track { get; set; }
    }

    public class TwitterStreamConfig : ITwitterStreamConfig
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string Track { get; set; }

        private static string GetOrThrow(string prefix, string name) => GetOrThrow(prefix + name);
        private static string GetOrThrow(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if(value == null) throw new InvalidOperationException($"missing environment variable: {name}");
            return value;
        }

        public TwitterStreamConfig()
        {
            const string prefix = "TWITTER_";
            ConsumerKey = GetOrThrow(prefix, "CONSUMER_KEY");
            ConsumerSecret = GetOrThrow(prefix, "CONSUMER_SECRET");
            AccessToken = GetOrThrow(prefix, "ACCESS_TOKEN");
            AccessTokenSecret = GetOrThrow(prefix, "ACCESS_TOKEN_SECRET");
            Track = GetOrThrow(prefix, "TRACK");
        }
    }
}
