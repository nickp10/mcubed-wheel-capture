using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace mCubed.WheelCapture
{
	public class CategorySerializer
	{
		public IEnumerable<Category> ReadCategories()
		{
			var categories = new List<Category>();
			var document = XDocument.Load("WOF.xml");
			foreach (var categoryElement in document.Root.Elements("Category"))
			{
				var category = new Category((string)categoryElement.Attribute("Name"));
				category.Words = categoryElement.Elements("Word").Select(w => new Word(category, w.Value)).ToList();
				categories.Add(category);
			}
			return categories;
		}

		public void WriteCategories(IEnumerable<Category> categories)
		{
			var document = new XDocument(
				new XElement("Categories",
					categories.Select(c => new XElement("Category",
						new XAttribute("Name", c.Name),
						c.Words.Select(w => new XElement("Word", w.Value))
					))
				)
			);
			document.Save("WOF.xml");
		}
	}
}
