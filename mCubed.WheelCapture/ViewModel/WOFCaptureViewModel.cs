using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using mCubed.WheelCapture.Model;
using mCubed.WheelCapture.Services;

namespace mCubed.WheelCapture.ViewModel
{
	public class WOFCaptureViewModel : IHandleWOFEvent, INotifyPropertyChanged
	{
		#region Data Members

		private readonly PuzzleAnalyzer _analyzer;
		private readonly WheelCaptureService _service;
		private readonly ObservableCollection<Word> _words;
		private readonly WebSocketMessageParser _parser;
		private readonly IList<WheelCategory> _categories;
		private readonly WheelCategory _unknownCategory;

		#endregion

		#region Constructors

		public WOFCaptureViewModel()
		{
			_service = new WheelCaptureService();
			_categories = _service.CategoryList;
			_unknownCategory = GetOrAddCategory("Unknown");
			_words = new ObservableCollection<Word>(_service.WordList.Select(w => new Word(GetCategoryName(w.CategoryID), w.Word)));
			_analyzer = new PuzzleAnalyzer(_words);
			_parser = new WebSocketMessageParser(this);
		}

		#endregion

		#region Properties

		public PuzzleAnalyzer Analyzer
		{
			get { return _analyzer; }
		}

		private Puzzle _currentPuzzle;
		public Puzzle CurrentPuzzle
		{
			get { return _currentPuzzle; }
			private set
			{
				if (_currentPuzzle != value)
				{
					_currentPuzzle = value;
					OnPropertyChanged("CurrentPuzzle");
				}
			}
		}

		private string _currentWedge = "$0";
		public string CurrentWedge
		{
			get { return _currentWedge; }
			private set
			{
				if (_currentWedge != value)
				{
					_currentWedge = value;
					OnPropertyChanged("CurrentWedge");
				}
			}
		}

		public ObservableCollection<Word> Words
		{
			get { return _words; }
		}

		#endregion

		#region Methods

		private string GetCategoryName(string categoryID)
		{
			var category = _categories.FirstOrDefault(c => c.ID == categoryID);
			if (category != null)
			{
				return category.Name;
			}
			return _unknownCategory.Name;
		}

		private WheelCategory GetOrAddCategory(string categoryName)
		{
			var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
			if (category != null)
			{
				return category;
			}
			category = _service.AddCategory(categoryName);
			_categories.Add(category);
			return category;
		}

		#endregion

		#region IHandleWOFEvent Members

		public void PuzzleChanged(string puzzle)
		{
			if (!string.IsNullOrEmpty(puzzle))
			{
				lock (this)
				{
					if (CurrentPuzzle == null)
					{
						CurrentPuzzle = new Puzzle(Words);
					}
					CurrentPuzzle.CurrentPuzzle = puzzle;
				}
			}
		}

		public void CategoryChanged(string category)
		{
			if (!string.IsNullOrEmpty(category))
			{
				lock (this)
				{
					if (CurrentPuzzle == null)
					{
						CurrentPuzzle = new Puzzle(Words);
					}
					CurrentPuzzle.Category = category;
				}
			}
		}

		public void LetterGuessed(string letter)
		{
			if (!string.IsNullOrEmpty(letter))
			{
				lock (this)
				{
					if (CurrentPuzzle == null)
					{
						CurrentPuzzle = new Puzzle(Words);
					}
					CurrentPuzzle.LetterGuessed(letter);
				}
			}
		}

		public void WheelSpun(int wedge)
		{
			CurrentWedge = wedge == -1 ? "LOSE A TURN" : string.Format("{0:c}", wedge);
		}

		public void PuzzleFinished()
		{
			lock (this)
			{
				var puzzle = CurrentPuzzle;
				if (puzzle != null)
				{
					if (!puzzle.CurrentPuzzle.Contains('_') && !Words.Any(w => string.Equals(w.Category, puzzle.Category, StringComparison.OrdinalIgnoreCase) && string.Equals(w.Value, puzzle.CurrentPuzzle, StringComparison.OrdinalIgnoreCase)))
					{
						var category = GetOrAddCategory(puzzle.Category);
						var wheelWord = _service.AddWord(puzzle.CurrentPuzzle, category.ID);
						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
						{
							Words.Add(new Word(category.Name, wheelWord.Word));
						}));
					}
					CurrentPuzzle = null;
					CurrentWedge = "$0";
				}
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
