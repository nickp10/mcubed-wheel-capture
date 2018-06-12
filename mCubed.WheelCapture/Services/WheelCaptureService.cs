using System.Collections.Generic;
using mCubed.WheelCapture.ViewModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mCubed.WheelCapture.Services
{
	public class WheelCaptureService
	{
		#region Constants

		private const string CATEGORY_TABLE = "wheelcategories";
		private const string WORD_TABLE = "wheelwords";

		#endregion

		#region Data Members

		private IMongoDatabase _database;

		#endregion

		#region Methods

		public List<WheelCategory> CategoryList
		{
			get { return Get<WheelCategory>(CATEGORY_TABLE); }
		}

		public List<WheelWord> WordList
		{
			get { return Get<WheelWord>(WORD_TABLE); }
		}

		public WheelCategory AddCategory(string categoryName)
		{
			var category = new WheelCategory
			{
				Name = categoryName
			};
			return Post(CATEGORY_TABLE, category);
		}

		public WheelWord AddWord(string word, ObjectId categoryID)
		{
			var wheelWord = new WheelWord
			{
				Word = word,
				CategoryID = categoryID
			};
			return Post(WORD_TABLE, wheelWord);
		}

		#endregion

		#region Helpers

		private IMongoDatabase GetDatabase()
		{
			if (_database == null)
			{
				var client = new MongoClient(Settings.MongoConnectionUrl);
				_database = client.GetDatabase(Settings.MongoDBName);
			}
			return _database;
		}

		private List<T> Get<T>(string tableName)
		{
			var db = GetDatabase();
			return db.GetCollection<T>(tableName).AsQueryable().ToList();
		}

		private T Post<T>(string tableName, T obj)
		{
			var db = GetDatabase();
			db.GetCollection<T>(tableName).InsertOne(obj);
			return obj;
		}

		#endregion
	}
}
