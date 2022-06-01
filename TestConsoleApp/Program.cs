using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using IbukiBooruLibrary;
using IbukiBooruLibrary.Classes;
using IbukiMobile.Classes;

namespace TestConsoleApp {
    /// <summary>
    /// Test application. Downloads approximately 5000 posts and tries to lookup a post with provided ID.
    /// The output is the lookup time in milliseconds.
    /// </summary>
    public static class Program {
        public static async Task Main(string[] args) {
            string script = File.ReadAllText("Extensions/Danbooru.js");
            Booru Danbooru = new Booru(script);

            List<BooruPost> posts = new List<BooruPost>();

            for (int i = 1; i < 50; i++) {
                Console.WriteLine($"Getting posts from page {i}; Total posts: {posts.Count}");
                posts.AddRange(await Danbooru.GetPosts(page: i, limit: 100));
            }
            
            Console.WriteLine("Dictionary Conversion lookup speed test");

            int search_id = Int32.Parse(Console.ReadLine() ?? "");
            
            DateTime start = DateTime.Now;
            posts.GetByID(search_id);
            Console.WriteLine($"Lookup time: {(DateTime.Now - start).TotalMilliseconds} ms");
            
            Console.WriteLine("Binary search lookup speed test");
            
            start = DateTime.Now;
            posts.BinaryGetByID(search_id);
            Console.WriteLine($"Lookup time: {(DateTime.Now - start).TotalMilliseconds} ms");
        }
    }
}