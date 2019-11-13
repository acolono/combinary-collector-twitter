using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using Tweetinvi.Events;
using Tweetinvi.Streams.Model;
using TwitterDb;
using TwModels = Tweetinvi.Models;
using TwEvents = Tweetinvi.Streaming.Events;

namespace TwitterStream
{
    public interface IHandler<in TAdd, in TDelete>
    {
        void Add(TAdd data);
        void Delete(TweetDeletedEventArgs data);
    }

    public interface ITweetHandler : IHandler<TwModels.ITweet, TweetDeletedEventArgs> { }

    public class TweetHandler : ITweetHandler {
        private readonly bool _verbose;
        public TweetHandler(bool verbose = false) {
            _verbose = verbose;
        }


        public void Add(TwModels.ITweet inTweet)
        {
            var now = DateTime.Now;

            using (var db = new TwitterDbContext())
            {
                var users = inTweet.MapToUsers();
                foreach (var user in users)
                {
                    var exists = db.User.Any(u => u.Id == user.Id);
                    if (!exists) db.User.Add(user);
                }
                
                var tweetExists = db.Tweet.Any(t => t.Id == inTweet.Id);

                if (tweetExists)
                {
                    // twitter sometimes sends duplicates
                    Console.WriteLine($"duplicate tweet: {inTweet.Id} {inTweet.Url}");
                }

                if (!tweetExists)
                {
                    var tweets = inTweet.MapToTweets();
                    foreach (var tweet in tweets)
                    {
                        var exists = db.Tweet.Any(t => t.Id == tweet.Id);
                        if (!exists) {
                            if (_verbose) Console.WriteLine($"{tweet.UserId}> {tweet.Text}");
                            db.Tweet.Add(tweet);
                        }
                    }
                }

                var metrics = new Metric
                {
                    TweetId = inTweet.Id,
                    FavouritesCount = inTweet.FavoriteCount,
                    RetweetCount = inTweet.RetweetCount,
                    Ts = now,
                };

                db.Metric.Add(metrics);
                db.SaveChanges();

                var stream = new Stream
                {
                    TweetId = inTweet.Id,
                    MetricId = metrics.Id,
                    Ts = now,
                };

                db.Stream.Add(stream);
                db.SaveChanges();
            }
        }

        public void Delete(TweetDeletedEventArgs data)
        {
            using (var db = new TwitterDbContext())
            {
                var tweet = db.Tweet.SingleOrDefault(t => t.Id == data.TweetId);
                if (tweet?.UserId == data.UserId)
                {
                    tweet.Deleted = true;
                    db.Tweet.Update(tweet);
                    db.SaveChanges();
                }
            }
        }
    }

    public static class Extensions {
        public static string ToNullIfEmpty(this string str) => string.IsNullOrWhiteSpace(str) ? null : str;

        public static void MapITweetIntoUser(TwModels.ITweet inTweet, User to)
        {
            to.Id = inTweet.CreatedBy.Id;
            to.Name = inTweet.CreatedBy.Name;
            to.ScreenName = inTweet.CreatedBy.ScreenName;
            to.ProfileImageUrl = inTweet.CreatedBy.ProfileImageUrl;
        }

        public static IEnumerable<User> MapToUsers(this TwModels.ITweet inTweet)
        {
            var list = new List<User>();
            MapUsersRecursive(list, inTweet);
            return list.DistinctBy(l=>l.Id);
        }

        private static void MapUsersRecursive(List<User> list, TwModels.ITweet inTweet)
        {
            var quotedId = inTweet.QuotedTweet?.CreatedBy?.Id ?? 0;
            if (quotedId > 0) MapUsersRecursive(list, inTweet.QuotedTweet);

            var retweetedId = inTweet.RetweetedTweet?.CreatedBy?.Id ?? 0;
            if (retweetedId > 0) MapUsersRecursive(list, inTweet.RetweetedTweet);

            var user = new User();
            MapITweetIntoUser(inTweet, user);
            list.Add(user);
        }

        public static void MapITweetIntoTweet(TwModels.ITweet inTweet, Tweet to)
        {
            to.Id = inTweet.Id;
            to.Text = inTweet.FullText.ToNullIfEmpty() ?? inTweet.Text;
            to.MediaPhoto = inTweet.Media?.FirstOrDefault(m => m.MediaType == "photo")?.MediaURLHttps.ToNullIfEmpty();
            to.Hashtags = inTweet.Hashtags?.Select(t => t.Text).ToArray();
            to.Lat = (decimal?) inTweet.Coordinates?.Latitude;
            to.Lon = (decimal?) inTweet.Coordinates?.Longitude;
            to.UserId = inTweet.CreatedBy.Id;
            to.Place = inTweet.Place?.FullName;
            to.Country = inTweet.Place?.Country;
            to.CountryCode = inTweet.Place?.CountryCode;
            to.Ts = inTweet.CreatedAt;
            to.InReplyToTweetId = inTweet.InReplyToStatusId;
            to.IsRetweet = inTweet.IsRetweet;
            to.QuotedTweetId = inTweet.QuotedStatusId;
            to.RetweetedTweetId = inTweet.RetweetedTweet?.Id;
            to.Url = inTweet.Url;
            to.Source = inTweet.Source;
        }

        public static IEnumerable<Tweet> MapToTweets(this TwModels.ITweet inTweet)
        {
            var list = new List<Tweet>();
            MapTweetsRecursive(list, inTweet);
            return list.DistinctBy(l=>l.Id);
        }

        private static void MapTweetsRecursive(List<Tweet> list, TwModels.ITweet inTweet)
        {
            var quotedId = inTweet.QuotedTweet?.Id ?? 0;
            if (quotedId > 0) MapTweetsRecursive(list, inTweet.QuotedTweet);

            var retweetedId = inTweet.RetweetedTweet?.Id ?? 0;
            if(retweetedId > 0) MapTweetsRecursive(list, inTweet.RetweetedTweet);

            var tweet = new Tweet();
            MapITweetIntoTweet(inTweet, tweet);
            list.Add(tweet);
        }
    }
}
