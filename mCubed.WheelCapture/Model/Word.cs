using System.Linq;
using System.Text;

namespace mCubed.WheelCapture.Model
{
	public class Word
	{
		#region Constructors

		public Word(string category, string word)
		{
			Category = category;
			Value = word;
		}

		#endregion

		#region Properties

		public string Category { get; private set; }
		public string Value { get; private set; }

		public string SolveLetters
		{
			get
			{
				var builder = new StringBuilder();
				foreach (var c in Value.Where(char.IsLetter).Where(c => c != 'A' && c != 'E' && c != 'I' && c != 'O' && c != 'U').GroupBy(c => c).Select(c => new { Count = c.Count(), Character = c.Key }).OrderByDescending(c => c.Count))
				{
					if (builder.Length > 0)
					{
						builder.Append(" ");
					}
					builder.Append(c.Count);
					builder.Append(c.Character);
				}
				return builder.ToString();
			}
		}

		#endregion
	}
}
