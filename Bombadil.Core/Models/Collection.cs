using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Bombadil.Core.Models
{
    public class Collection
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("references")]
        public List<string> References { get; set; }

        public Collection()
        {
            this.References = new List<string>();
        }
    }

    public enum CollectionType
    { 
        Archives,
        Tags,
        Categories
    }
}