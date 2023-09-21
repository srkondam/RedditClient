using System.Text.Json.Serialization;

namespace Reddit.Data
{
    [Serializable]
    public class Post
    {
        [JsonPropertyName("data")]
        public PostDetails Data { get; set; }

        public override string ToString()
        {
            return Data.ToString();
        }
    }

    [Serializable]
    public class PostDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("author_fullname")]
        public string Author { get; set; } = string.Empty;
        [JsonPropertyName("ups")]
        public long Ups { get; set; }
        [JsonPropertyName("downs")]
        public long Downs { get; set; }

        public override string ToString()
        {
            return $@"
                    ### Post ###
                    Title: {Title}
                    Author: {Author}
                    Ups: {Ups}
                    Downs: {Downs}
                    ";
        }
    }
}