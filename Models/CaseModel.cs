using System.ComponentModel;
using Newtonsoft.Json;

namespace Models
{
    public class CaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        
        [JsonProperty(PropertyName = "caseNumber")]
        public string CaseNumber { get; set; }
        
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }
        
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }
}