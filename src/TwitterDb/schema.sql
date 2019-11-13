DROP SCHEMA IF EXISTS db CASCADE;
CREATE SCHEMA db;
SET SEARCH_PATH TO db, public;

-- dotnet ef dbcontext scaffold -f -c TwitterDbContext "Host=192.168.93.3;Database=postgres;Username=postgres;Search Path=db,public" Npgsql.EntityFrameworkCore.PostgreSQL

CREATE TABLE "user" (
  id BIGINT PRIMARY KEY NOT NULL,
  name TEXT not NULL,
  screen_name TEXT NOT NULL,
  profile_image_url TEXT NOT NULL
);

CREATE TABLE tweet (
  id BIGINT PRIMARY KEY NOT NULL,
  user_id BIGINT NOT NULL REFERENCES "user"(id),
  text TEXT not NULL,
  media_photo TEXT,
  hashtags TEXT[] not null,
  country TEXT,
  country_code TEXT,
  place TEXT,
  in_reply_to_tweet_id BIGINT,
  quoted_tweet_id BIGINT,
  retweeted_tweet_id BIGINT,
  url TEXT,
  source TEXT,
  lat NUMERIC(9,6),
  lon NUMERIC(9,6),
  deleted BOOLEAN NOT NULL,
  is_retweet BOOLEAN NOT NULL,
  ts TIMESTAMPTZ
);

CREATE TABLE metric (
  id BIGSERIAL PRIMARY KEY NOT NULL,
  tweet_id BIGINT NOT NULL REFERENCES tweet(id),
  favourites_count INT,
  retweet_count INT,
  ts TIMESTAMPTZ NOT NULL
);

CREATE TABLE stream (
  id BIGSERIAL PRIMARY KEY NOT NULL ,
  tweet_id BIGINT NOT NULL REFERENCES tweet(id),
  metric_id BIGINT NOT NULL REFERENCES metric(id),
  ts TIMESTAMPTZ NOT NULL
);

CREATE VIEW export AS
SELECT
  m.id,
  t.id as tweet_id,
  t.is_retweet,
  u.screen_name,
  u.profile_image_url,
  t.text,
  t.hashtags,
  concat_ws($$,$$, t.lat, t.lon) as location,
  t.country,
  t.country_code,
  t.place,
  t.deleted,
  m.favourites_count,
  m.retweet_count,
  m.ts

FROM metric m
LEFT JOIN tweet t on m.tweet_id = t.id
LEFT JOIN "user" u on t.user_id = u.id
ORDER BY m.id DESC;
