using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reddit.Data
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = String.Empty;
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = "*";
    }

    public class RedditApiError
    {
        [JsonPropertyName("error")]
        public string ErrorCode { get; set; } = String.Empty;

        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; } = String.Empty;
    }
}
