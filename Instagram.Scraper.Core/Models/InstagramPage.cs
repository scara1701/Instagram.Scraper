using System;
using System.Collections.Generic;

namespace Instagram.Scraper.Models
{
    public class InstagramPage
    {
        /// <summary>
        /// The Url of the instagram page https://www.instagram.com/accountname/
        /// </summary>
        public Uri Url { get; set; }
        /// <summary>
        /// Number of posts retrieved
        /// </summary>
        public int MaxPostCount { get; set; }
        /// <summary>
        /// List of retrieved instagram posts
        /// </summary>
        public List<InstagramPost> InstagramPosts { get; set; }
    }
}
