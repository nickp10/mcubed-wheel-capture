using System.Collections.Generic;

namespace mCubed.WheelCapture.Services
{
	public class WheelCaptureService
	{
		public List<WheelCategory> CategoryList
		{
			get { return new List<WheelCategory>(); }
		}

		public List<WheelWord> WordList
		{
			get { return new List<WheelWord>(); }
		}

		public WheelCategory AddCategory(string categoryName)
		{
			var category = new WheelCategory
			{
				Name = categoryName
			};
			// Add it
			return category;
		}

		public WheelWord AddWord(string word, string categoryID)
		{
			var wheelWord = new WheelWord
			{
				Word = word,
				CategoryID = categoryID
			};
			// Add it
			return wheelWord;
		}
	}
}
