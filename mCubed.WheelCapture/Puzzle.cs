using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace mCubed.WheelCapture
{
	public class Puzzle : INotifyPropertyChanged
	{
		#region Data Members

		private string _category = "Unknown";
		private string _currentPuzzle;
		private bool _isCompleted;
		private string _originalPuzzle;
		private IEnumerable<Word> _solutions;
		private readonly ObservableCollection<Word> _words;

		#endregion

		#region Constructors

		public Puzzle(string originalPuzzle, ObservableCollection<Word> words)
		{
			Solutions = _words = words;
			OriginalPuzzle = originalPuzzle;
			CurrentPuzzle = originalPuzzle;
		}

		#endregion

		#region Events

		public event Action<Word> AddWord;

		#endregion

		#region Properties

		public string Category
		{
			get { return _category; }
			set
			{
				if (_category != value)
				{
					_category = value;
					OnPropertyChanged("Category");
					OnCategoryChanged();
				}
			}
		}

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

		public bool IsCompleted
		{
			get { return _isCompleted; }
			private set
			{
				if (_isCompleted != value)
				{
					_isCompleted = value;
					OnPropertyChanged("IsCompleted");
					OnIsCompletedChanged();
				}
			}
		}

		public string OriginalPuzzle
		{
			get { return _originalPuzzle; }
			set
			{
				if (_originalPuzzle != value)
				{
					_originalPuzzle = value;
					OnPropertyChanged("OriginalPuzzle");
				}
			}
		}

		public IEnumerable<Word> Solutions
		{
			get { return _solutions; }
			private set
			{
				if (_solutions != value)
				{
					_solutions = value;
					OnPropertyChanged("Solutions");
				}
			}
		}

		#endregion

		#region Methods

		private void OnCurrentPuzzleChanged()
		{
			if (Word.MatchesPuzzle(OriginalPuzzle, CurrentPuzzle))
			{
				Solutions = Solutions.Where(c => c.MatchesPuzzle(CurrentPuzzle)).ToArray();
				IsCompleted = !CurrentPuzzle.Contains('_');
			}
		}

		private void OnCategoryChanged()
		{
			UpdateCategories();
		}

		private void OnIsCompletedChanged()
		{
			var solution = Solutions.FirstOrDefault(s => s.Value == CurrentPuzzle);
			if (solution != null)
			{
				Category = solution.Category;
			}
			UpdateCategories();
		}

		private void UpdateCategories()
		{
			if (IsCompleted && !_words.Any(w => string.Equals(w.Category, Category, StringComparison.OrdinalIgnoreCase) && string.Equals(w.Value, CurrentPuzzle, StringComparison.OrdinalIgnoreCase)))
			{
				OnAddWord(new Word(Category, CurrentPuzzle));
			}
		}

		private void OnAddWord(Word word)
		{
			var handler = AddWord;
			if (handler != null)
			{
				handler(word);
			}
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
