using System;
using System.Collections.Generic;
using System.Linq;
using CoreTweet;
using CoreTweet.Core;
using Lib;
using Lib.Config;
using TwitterDb;

namespace TwitterUpdate
{
    internal static class TwitterUpdateMain
    {
        internal static void Main(string[] args)
        {
            Exceptions.Log(Start);
        }

        private static void Start()
        {
            var config = new TwitterStreamConfig();
            var tokens = Tokens.Create(config.ConsumerKey, config.ConsumerSecret, config.AccessToken, config.AccessTokenSecret);
            var maxTrackingHours = double.Parse(Environment.GetEnvironmentVariable("TWITTER_UPDATE_MAX_TRACKING_HOURS") ?? "96"); // Default: 4 Days

            using (var db = new TwitterDbContext())
            {
                var changes = 0L;
                var list = new List<List<Metric>>();
                {
                    var block = new List<Metric>();
                    list.Add(block);
                    var tweetIds = new HashSet<long>();

                    foreach (var m in db.Metric.OrderByDescending(m=>m.Id))
                    {
                        if(!tweetIds.Add(m.TweetId)) continue;
                        var age = (DateTime.Now - m.Ts).TotalHours;
                        if(age > maxTrackingHours) continue;
                        if (block.Count >= 100)
                        {
                            block = new List<Metric>();
                            list.Add(block);
                        }
                        block.Add(m);
                    }
                }
                var blockCount = 0;
                foreach (var block in list)
                {
                    blockCount++;
                    var ids = block.Select(b => b.TweetId);
                    ListedResponse<Status> statuses;
                    DateTime now;
                    try
                    {
                        var rl = tokens.Application.RateLimitStatusAsync().AwaitAndGetResult();
                        Console.WriteLine($"RateLimit Remaining:{rl.RateLimit.Remaining}, block:{blockCount}/{list.Count}");
                        if (rl.RateLimit.Remaining < 5)
                        {
                            Console.WriteLine($"sleeping until: {rl.RateLimit.Reset}");
                            Sleep.Until(rl.RateLimit.Reset);
                        }
                        statuses = tokens.Statuses.LookupAsync(ids, trim_user: true, include_entities: false).AwaitAndGetResult();
                        now = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    var save = false;
                    foreach (var status in statuses)
                    {
                        var metric = block.Single(b => b.TweetId == status.Id);
                        var change = metric.FavouritesCount != status.FavoriteCount ||
                                     metric.RetweetCount != status.RetweetCount;
                        if (!change) continue;
                        save = true;
                        changes++;
                        db.Metric.Add(new Metric()
                        {
                            FavouritesCount = status.FavoriteCount,
                            RetweetCount = status.RetweetCount,
                            TweetId = metric.TweetId,
                            Ts = now,
                        });
                        //Console.WriteLine($"{(change?"change":"nochange")}: {metric.Id}");
                    }
                    if (save) db.SaveChanges();
                }
                Console.WriteLine("changes: " + changes);
            }
        }
    }
}
