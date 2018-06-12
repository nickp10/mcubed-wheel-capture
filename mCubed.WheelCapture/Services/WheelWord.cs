using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mCubed.WheelCapture.Services
{
	public class WheelWord
	{
		[BsonElement("approved")]
		public bool Approved { get; set; }

		[BsonId]
		public ObjectId ID { get; set; }

		[BsonElement("categoryID")]
		public ObjectId CategoryID { get; set; }

		[BsonElement("word")]
		public string Word { get; set; }
	}
}
