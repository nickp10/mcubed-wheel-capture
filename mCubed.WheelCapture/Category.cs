using System.Collections.Generic;

namespace mCubed.WheelCapture
{
	public class Category
	{
		public Category(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public List<Word> Words { get; set; }
	}
}
