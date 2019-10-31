using System.Collections.Generic;

namespace Instagram.Scraper.Models
{
    public class InstagramPost
    {
        /// <summary>
        /// List of pictures connected to instagram post
        /// </summary>
        public List<byte[]> Pictures { get; set; }
        /// <summary>
        /// Message attached to instagram post
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Likes attached to instagram post
        /// </summary>
        public int Likes { get; set; }
    }
}
