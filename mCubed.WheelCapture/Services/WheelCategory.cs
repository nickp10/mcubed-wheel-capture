using Newtonsoft.Json;

namespace mCubed.WheelCapture.Services
{
	public class WheelCategory
	{
		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
