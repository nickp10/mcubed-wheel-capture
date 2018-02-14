using System.Collections.Generic;
using System.Net;
using mCubed.WheelCapture.ViewModel;
using Newtonsoft.Json;

namespace mCubed.WheelCapture.Services
{
	public class WheelCaptureService
	{
		#region Constants

		private const string CATEGORY_TABLE = "wheelcategories";
		private const string WORD_TABLE = "wheelwords";

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

		public WheelWord AddWord(string word, string categoryID)
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

		private List<T> Get<T>(string tableName)
		{
			var url = string.Format("http://{0}:{1}/{2}", Settings.PersistenceServer, Settings.PersistencePort, tableName);
			using (var client = new WebClient())
			{
				client.Headers.Add("mcubed-app-name", Settings.PersistenceAppName);
				client.Headers.Add("mcubed-app-key", Settings.PersistenceAppKey);
				var responseBody = client.DownloadString(url);
				return JsonConvert.DeserializeObject<List<T>>(responseBody);
			}
		}

		private T Post<T>(string tableName, T obj)
		{
			var url = string.Format("http://{0}:{1}/{2}", Settings.PersistenceServer, Settings.PersistencePort, tableName);
			using (var client = new WebClient())
			{
				client.Headers.Add("mcubed-app-name", Settings.PersistenceAppName);
				client.Headers.Add("mcubed-app-key", Settings.PersistenceAppKey);
				client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
				var requestBody = JsonConvert.SerializeObject(obj);
				var responseBody = client.UploadString(url, requestBody);
				return JsonConvert.DeserializeObject<T>(responseBody);
			}
		}

		#endregion
	}
}
