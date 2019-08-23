using System;
using System.Linq;
using Ex = Lib.Exceptions;
using Lib.Config;
using Tweetinvi;
using Tweetinvi.Streams.Model;

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
            var handler = new TweetHandler();

            var stream = Stream.CreateFilteredStream(credentials);
            //var stream = Stream.CreateSampleStream(credentials);

            config.Track.Split(",").ToList().ForEach(t => stream.AddTrack(t));
            stream.MatchingTweetReceived += (o, args) => Ex.Log(() => handler.Add(args.Tweet));
            //stream.TweetReceived += (o, args) => Ex.Log(() => handler.Add(args.Tweet));
            stream.DisconnectMessageReceived += (o, args) => Console.WriteLine($"DisconnectMessageReceived code:{args.DisconnectMessage.Code} reason:{args.DisconnectMessage.Reason} name:{args.DisconnectMessage.StreamName}");
            stream.LimitReached += (o, args) => Console.WriteLine($"LimitReached skipped:{args.NumberOfTweetsNotReceived}");
            stream.WarningFallingBehindDetected += (o, args) => Console.WriteLine($"WarningFallingBehindDetected code:{args.WarningMessage.Code} percentFull:{args.WarningMessage.PercentFull} message:{args.WarningMessage.Message}");
            stream.TweetDeleted += (o, args) => Ex.Log(() => handler.Delete(args));
            stream.KeepAliveReceived += (o, args) => Console.WriteLine("KeepAlive");

            //stream.StartStream();
            stream.StartStreamMatchingAllConditionsAsync().GetAwaiter().GetResult();

            Console.WriteLine("stream was stopped");
        }
    }
}
