using Instagram.Scraper.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instagram.Scraper.Services
{
    public static class InstagramService
    {
        public static async Task<InstagramPage> GetInstagramPage(Uri url, int maxPostCount)
        {
            InstagramPage instagramPage = new InstagramPage();
            instagramPage.Url = url;
            instagramPage.MaxPostCount = maxPostCount;

            List<string> shortcodes = new List<string>();
            //Retrieve instagram page
            HttpClient client = new HttpClient();
            using (var response = await client.GetAsync(url))
            {
                using (var content = response.Content)
                {
                    string json = await content.ReadAsStringAsync();
                    JObject jsonResult = JObject.Parse(json);
                    shortcodes = jsonResult.SelectTokens("$.graphql.user.edge_owner_to_timeline_media.edges[*].node.shortcode").Select(s => (string)s).Take(instagramPage.MaxPostCount).ToList();
                }
            }

            //Retrieve post with pictures
            instagramPage.InstagramPosts = await GetInstagramPosts(instagramPage, shortcodes);

            return instagramPage;
        }

        private static async Task<List<InstagramPost>> GetInstagramPosts(InstagramPage instagramPage, List<string> shortcodes)
        {
            List<InstagramPost> instagramPosts = new List<InstagramPost>();
            foreach (var shortCode in shortcodes)
            {
                //Retrieve instagram post with pictures
                HttpClient client = new HttpClient();
                Uri url = new Uri(@"https://www.instagram.com/p/" + shortCode + @"/?__a=1", UriKind.Absolute);
                InstagramPost instagramPost = new InstagramPost();
                instagramPost.Pictures = new List<byte[]>();
                List<Uri> pictureUrls = new List<Uri>();
                using (var response = await client.GetAsync(url))
                {

                    using (var content = response.Content)
                    {
                        string json = await content.ReadAsStringAsync();
                        JObject jsonResult = JObject.Parse(json);
                        try
                        {
                            instagramPost.Message = (string)jsonResult.SelectToken("$.graphql.shortcode_media.edge_media_to_caption.edges[0].node.text");
                            instagramPost.Likes = (int)jsonResult.SelectToken("$.graphql.shortcode_media.edge_media_preview_like.count");
                        }
                        catch (Exception)
                        {

                        }
                        pictureUrls = jsonResult.SelectTokens("$.graphql.shortcode_media.edge_sidecar_to_children.edges.[*].node.display_resources[0].src").Select(s => (Uri)s).ToList();

                        if(pictureUrls.Count == 0)
                        {
                            pictureUrls.Add((Uri)jsonResult.SelectToken("$.graphql.shortcode_media.display_resources[0].src"));
                        }
                    }
                }

                foreach (var pictureUrl in pictureUrls)
                {
                    byte[] picture = await client.GetByteArrayAsync(pictureUrl);
                    instagramPost.Pictures.Add(picture);
                }

                instagramPosts.Add(instagramPost);
            }
            return instagramPosts;
        }
    }
}
