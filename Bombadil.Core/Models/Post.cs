using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;

namespace Bombadil.Core.Models
{
    public class Post : IContent
    {

        [JsonIgnore]
        public string CompleteFilePath { get; set; }

        [JsonIgnore]
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.CompleteFilePath);
            }
        }

        //
        // Properties present in the JSON file.
        //

        [JsonProperty("processed")]
        public bool IsProcessed { get; set; }

        [JsonProperty("date")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }


        //
        // Properties used to generate the Views etc.
        //

        [JsonIgnore]
        public bool Exists { get { return !string.IsNullOrWhiteSpace(this.Content); } }

        public Post()
        {
            this.Tags = new List<string>();
            this.Categories = new List<string>();
        }

    }
}