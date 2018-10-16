using System;
using System.Collections.Generic;

namespace TwitterDb
{
    public partial class User
    {
        public User()
        {
            Tweet = new HashSet<Tweet>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string ProfileImageUrl { get; set; }

        public ICollection<Tweet> Tweet { get; set; }
    }
}
