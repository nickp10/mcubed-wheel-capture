using System.Collections.Generic;
using System.Linq;
using mCubed.WheelCapture.Model;

namespace mCubed.WheelCapture.ViewModel
{
	public class PuzzleAnalyzer
	{
		private IEnumerable<KeyValuePair<char, int>> _characterCounts;
		private IEnumerable<string> _duplicatePuzzles;
		private readonly IEnumerable<Word> _words;

		public PuzzleAnalyzer(IEnumerable<Word> words)
		{
			_words = words;
		}

		public IEnumerable<KeyValuePair<char, int>> CharacterCounts
		{
			get
			{
				if (_characterCounts == null)
				{
					_characterCounts = _words.
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
					_duplicatePuzzles = _words.
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
