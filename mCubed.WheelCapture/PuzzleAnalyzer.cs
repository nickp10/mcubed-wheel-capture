using System.Collections.Generic;
using System.Linq;

namespace mCubed.WheelCapture
{
	public class PuzzleAnalyzer
	{
		private readonly IEnumerable<Category> _categories;
		private IEnumerable<KeyValuePair<char, int>> _characterCounts;
		private IEnumerable<string> _duplicatePuzzles;

		public PuzzleAnalyzer(IEnumerable<Category> categories)
		{
			_categories = categories;
		}

		public IEnumerable<KeyValuePair<char, int>> CharacterCounts
		{
			get
			{
				if (_characterCounts == null)
				{
					_characterCounts = _categories.
						SelectMany(c => c.Words).
						SelectMany(w => w.Value).
						GroupBy(c => c).
						Select(c => new KeyValuePair<char, int>(c.Key, c.Count())).
						OrderByDescending(c => c.Value).
						ToArray();
				}
				return _characterCounts;
			}
		}

		public IEnumerable<string> DuplicatePuzzles
		{
			get
			{
				if (_duplicatePuzzles == null)
				{
					_duplicatePuzzles = _categories.
						SelectMany(c => c.Words).
						GroupBy(w => w.Value).
						Select(w => new { Count = w.Count(), Word = w.Key }).
						Where(w => w.Count > 1).
						OrderBy(w => w.Word).
						Select(w => w.Word).
						ToArray();
				}
				return _duplicatePuzzles;
			}
		}
	}
}
