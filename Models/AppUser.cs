using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using Common.JsonSerialization;

namespace Models
{

    public class AppUser
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "subjectId")]
        public string SubjectId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "passwordSalt")]
        public string PasswordSalt { get; set; }

        [JsonProperty(PropertyName = "passwordHash")]
        public string PasswordHash { get; set; }

        [JsonProperty(PropertyName = "providerName")]
        public string ProviderName { get; set; }

        [JsonProperty(PropertyName = "providerSubjectId")]
        public string ProviderSubjectId { get; set; }

        [JsonProperty(ItemConverterType = typeof(ClaimConverter))]
        public List<Claim> Claims  { get; set; } 
    }
}