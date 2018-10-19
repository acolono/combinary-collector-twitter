using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TwitterDb
{
    public partial class TwitterDbContext : DbContext
    {
        public virtual DbSet<Metric> Metric { get; set; }
        public virtual DbSet<Stream> Stream { get; set; }
        public virtual DbSet<Tweet> Tweet { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? @"Host=localhost;Database=postgres;Username=postgres;Search Path=db,public");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metric>(entity =>
            {
                entity.ToTable("metric", "db");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FavouritesCount).HasColumnName("favourites_count");

                entity.Property(e => e.RetweetCount).HasColumnName("retweet_count");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.TweetId).HasColumnName("tweet_id");

                entity.HasOne(d => d.Tweet)
                    .WithMany(p => p.Metric)
                    .HasForeignKey(d => d.TweetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("metric_tweet_id_fkey");
            });

            modelBuilder.Entity<Stream>(entity =>
            {
                entity.ToTable("stream", "db");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MetricId).HasColumnName("metric_id");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.TweetId).HasColumnName("tweet_id");

                entity.HasOne(d => d.Metric)
                    .WithMany(p => p.Stream)
                    .HasForeignKey(d => d.MetricId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stream_metric_id_fkey");

                entity.HasOne(d => d.Tweet)
                    .WithMany(p => p.Stream)
                    .HasForeignKey(d => d.TweetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stream_tweet_id_fkey");
            });

            modelBuilder.Entity<Tweet>(entity =>
            {
                entity.ToTable("tweet", "db");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Country).HasColumnName("country");

                entity.Property(e => e.CountryCode).HasColumnName("country_code");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Hashtags)
                    .IsRequired()
                    .HasColumnName("hashtags");

                entity.Property(e => e.InReplyToTweetId).HasColumnName("in_reply_to_tweet_id");

                entity.Property(e => e.IsRetweet).HasColumnName("is_retweet");

                entity.Property(e => e.Lat)
                    .HasColumnName("lat")
                    .HasColumnType("numeric(9, 6)");

                entity.Property(e => e.Lon)
                    .HasColumnName("lon")
                    .HasColumnType("numeric(9, 6)");

                entity.Property(e => e.Place).HasColumnName("place");

                entity.Property(e => e.QuotedTweetId).HasColumnName("quoted_tweet_id");

                entity.Property(e => e.RetweetedTweetId).HasColumnName("retweeted_tweet_id");

                entity.Property(e => e.Source).HasColumnName("source");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.Url).HasColumnName("url");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tweet)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tweet_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "db");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.ProfileImageUrl)
                    .IsRequired()
                    .HasColumnName("profile_image_url");

                entity.Property(e => e.ScreenName)
                    .IsRequired()
                    .HasColumnName("screen_name");
            });
        }
    }
}
