using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace mCubed.WheelCapture.Model
{
	public class Puzzle : INotifyPropertyChanged
	{
		#region Data Members

		private readonly Queue<string> _guessedLettersQueue = new Queue<string>();
		private IEnumerable<Word> _persistedPotentialSolutions;

		#endregion

		#region Constructors

		public Puzzle(IEnumerable<Word> solutions)
		{
			_persistedPotentialSolutions = solutions;
			_letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Select(l => new Letter { Display = l.ToString() }).ToArray();
		}

		#endregion

		#region Properties

		private string _category = "Unknown";
		public string Category
		{
			get { return _category; }
			set
			{
				if (_category != value)
				{
					_category = value;
					OnPropertyChanged("Category");
				}
			}
		}

		private string _currentPuzzle;
		public string CurrentPuzzle
		{
			get { return _currentPuzzle; }
			set
			{
				if (_currentPuzzle != value)
				{
					_currentPuzzle = value;
					OnPropertyChanged("CurrentPuzzle");
					OnCurrentPuzzleChanged();
				}
			}
		}

		private readonly IEnumerable<Letter> _letters;
		public IEnumerable<Letter> Letters
		{
			get { return _letters; }
		}

		private IEnumerable<Word> _potentialSolutions;
		public IEnumerable<Word> PotentialSolutions
		{
			get { return _potentialSolutions; }
			private set
			{
				if (_potentialSolutions != value)
				{
					_potentialSolutions = value;
					OnPropertyChanged("PotentialSolutions");
				}
			}
		}

		#endregion

		#region Methods

		public void LetterGuessed(string letter)
		{
			if (string.IsNullOrEmpty(CurrentPuzzle))
			{
				_guessedLettersQueue.Enqueue(letter);
			}
			else
			{
				var puzzleLetter = Letters.FirstOrDefault(l => l.Display == letter);
				if (puzzleLetter != null)
				{
					puzzleLetter.IsUsed = true;
					OnCurrentPuzzleChanged(false);
				}
			}
		}

		private void OnCurrentPuzzleChanged(bool persistPotential = true)
		{
			var puzzle = CurrentPuzzle;
			if (!string.IsNullOrEmpty(puzzle))
			{
				var regex = BuildFormatRegex(puzzle);
				var potentialSolutions = _persistedPotentialSolutions
					.Where(c => regex.IsMatch(c.Value))
					.OrderBy(w =>
					{
						if (w.Category.Equals(Category, StringComparison.OrdinalIgnoreCase))
						{
							return 1;
						}
						else if (w.Category.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
						{
							return 2;
						}
						else if (w.Category.Equals("Bonus", StringComparison.OrdinalIgnoreCase))
						{
							return 3;
						}
						return 4;
					})
					.ThenBy(w => w.Value)
					.ToArray();
				if (persistPotential)
				{
					_persistedPotentialSolutions = potentialSolutions;
				}
				PotentialSolutions = potentialSolutions;
				while (_guessedLettersQueue.Count > 0)
				{
					LetterGuessed(_guessedLettersQueue.Dequeue());
				}
			}
		}

		private Regex BuildFormatRegex(string puzzle)
		{
			var availableLetters = Letters.Where(l => !l.IsUsed).Select(l => l.Display).Aggregate((s1, s2) => s1 += s2);
			var availableRegex = string.Format("[{0}_]", availableLetters);
			return new Regex("^" + Regex.Escape(puzzle).Replace("_", availableRegex) + "$");
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
