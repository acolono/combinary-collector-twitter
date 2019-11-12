using System;
using System.Linq;
using Ex = Lib.Exceptions;
using Lib.Config;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace TwitterStream
{
    internal class TwitterStreamMain
    {
        private static void Main(string[] args)
        {
            Ex.Log(Start);
        }

        private static void Start()
        {
            Console.WriteLine("Start");
            var config = new TwitterStreamConfig();
            var credentials = Auth.SetUserCredentials(config.ConsumerKey, config.ConsumerSecret, config.AccessToken, config.AccessTokenSecret);
            var handler = new TweetHandler(config.Verbose);
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            var stream = Stream.CreateFilteredStream(credentials);

            config.Track?.Split(",").ToList().ForEach(t => {
                stream.AddTrack(t);
                Console.WriteLine($"Tracking: {t}");
            });
            config.Follow?.Split(",").ToList().ForEach(f => {
                if (long.TryParse(f, out var userId)) {
                    stream.AddFollow(userId);
                    Console.WriteLine($"Following: {userId}");
                    GetUserTimeline(userId, handler.Add);
                }
            });

            stream.MatchingTweetReceived += (o, args) => Ex.Log(() => {
                handler.Add(args.Tweet);
            });

            stream.DisconnectMessageReceived += (o, args) => Console.WriteLine($"DisconnectMessageReceived code:{args.DisconnectMessage.Code} reason:{args.DisconnectMessage.Reason} name:{args.DisconnectMessage.StreamName}");
            stream.LimitReached += (o, args) => Console.WriteLine($"LimitReached skipped:{args.NumberOfTweetsNotReceived}");
            stream.WarningFallingBehindDetected += (o, args) => Console.WriteLine($"WarningFallingBehindDetected code:{args.WarningMessage.Code} percentFull:{args.WarningMessage.PercentFull} message:{args.WarningMessage.Message}");
            stream.TweetDeleted += (o, args) => Ex.Log(() => handler.Delete(args));
            stream.KeepAliveReceived += (o, args) => Console.WriteLine("KeepAlive");

            stream.StartStreamMatchingAllConditionsAsync().GetAwaiter().GetResult();

            Console.WriteLine("stream was stopped");
        }

        private static void GetUserTimeline(long userId, Action<ITweet> tweetHandler) {
            var pars = new UserTimelineParameters();
            while (true) {
                var tweets = Timeline.GetUserTimeline(new UserIdentifier(userId), pars)?.ToList();
                if(tweets == null || !tweets.Any()) break;
                foreach (var tweet in tweets) {
                    if (tweet.CreatedBy.Id == userId) {
                        tweetHandler(tweet);
                    }
                }
                // https://developer.twitter.com/en/docs/tweets/timelines/guides/working-with-timelines
                // Subtract 1 from the lowest Tweet ID returned from the previous request and use this for the value of max_id
                pars.MaxId = tweets.Min(t => t.Id) - 1;
            }
        }
    }
}
