using Newtonsoft.Json;

namespace mCubed.WheelCapture.Services
{
	public class WheelWord
	{
		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("categoryID")]
		public string CategoryID { get; set; }

		[JsonProperty("word")]
		public string Word { get; set; }
	}
}
