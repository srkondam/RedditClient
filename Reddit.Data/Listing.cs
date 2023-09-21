using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reddit.Data
{
    [Serializable]
    public class Listing
    {
        [JsonPropertyName("data")]
        public ListingData Data { get; set; }
    }

    [Serializable]
    public class ListingData
    {
        [JsonPropertyName("children")]
        public IList<Post> Posts { get; set; } = new List<Post>();
    }
}
