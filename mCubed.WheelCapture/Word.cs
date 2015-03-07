using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mCubed.WheelCapture
{
	public class Word
	{
		public Word(Category category, string word)
		{
			Category = category;
			Value = word;
		}

		public bool MatchesPuzzle(string format)
		{
			return MatchesPuzzle(format, Value);
		}

		public static bool MatchesPuzzle(string format, string puzzle)
		{
			var regex = "^" + Regex.Escape(format).Replace("_", "[A-Z_]") + "$";
			return Regex.IsMatch(puzzle, regex);
		}

		public Category Category { get; private set; }
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
	}
}
