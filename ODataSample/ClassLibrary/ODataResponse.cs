using System.Text.Json.Serialization;

namespace ClassLibrary
{
	public class ODataResponse<T>
	{
        [JsonPropertyName("@odata.count")]
		public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<T> Data { get; set; } = new();
    }
}
