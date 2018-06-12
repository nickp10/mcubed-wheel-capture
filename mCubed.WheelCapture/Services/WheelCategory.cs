using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mCubed.WheelCapture.Services
{
	public class WheelCategory
	{
		[BsonId]
		public ObjectId ID { get; set; }

		[BsonElement("name")]
		public string Name { get; set; }
	}
}
