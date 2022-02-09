using Newtonsoft.Json;

namespace XPerts.TvShows.Models
{
    public class TvShowView : Identifiable
    {
        public string Name { get; set; }

        [JsonProperty("premiered")]
        public string PremieredOn { get; set; }

        public IEnumerable<string> Genres { get; set; }
    }
}