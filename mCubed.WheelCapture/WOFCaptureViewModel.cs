using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using mCubed.Services.Core;

namespace mCubed.WheelCapture
{
	public class WOFCaptureViewModel : IHandleWOFEvent, INotifyPropertyChanged
	{
		#region Data Members

		private readonly PuzzleAnalyzer _analyzer;
		private readonly Action<Word> _puzzleAddWordHandler;
		private readonly WheelWordService _service;
		private readonly ObservableCollection<Word> _words;
		private Puzzle _currentPuzzle;
		private readonly WebSocketMessageParser _parser;

		#endregion

		#region Constructors

		public WOFCaptureViewModel()
		{
			_service = new WheelWordService();
			_words = new ObservableCollection<Word>(_service.WordList.Select(w => new Word(w.Category, w.Word)));
			_analyzer = new PuzzleAnalyzer(_words);
			_puzzleAddWordHandler = new Action<Word>(OnAddWord);
			_parser = new WebSocketMessageParser(this);
		}

		#endregion

		#region Properties

		public PuzzleAnalyzer Analyzer
		{
			get { return _analyzer; }
		}

		public Puzzle CurrentPuzzle
		{
			get { return _currentPuzzle; }
			set
			{
				if (_currentPuzzle != value)
				{
					var oldPuzzle = _currentPuzzle;
					_currentPuzzle = value;
					OnPropertyChanged("CurrentPuzzle");
					OnCurrentPuzzleChanged(oldPuzzle, _currentPuzzle);
				}
			}
		}

		public ObservableCollection<Word> Words
		{
			get { return _words; }
		}

		#endregion

		#region Methods

		private void OnPuzzleChanged(string puzzle)
		{
			if (!string.IsNullOrEmpty(puzzle))
			{
				lock (this)
				{
					if (CurrentPuzzle == null)
					{
						CurrentPuzzle = new Puzzle(puzzle, Words);
					}
					else
					{
						if (puzzle.All(c => !char.IsLetter(c)))
						{
							if (CurrentPuzzle.CurrentPuzzle != puzzle)
							{
								// It's a new puzzle.
								CurrentPuzzle = new Puzzle(puzzle, Words);
							}
							else
							{
								// It's the same blank puzzle.
							}
						}
						else
						{
							if (CurrentPuzzle.CurrentPuzzle.Length == puzzle.Length)
							{
								// Either the same or updated letters.
								CurrentPuzzle.CurrentPuzzle = puzzle;
							}
							else
							{
								// Misread the puzzle because of popup.
							}
						}
					}
				}
			}
		}

		private void OnCurrentPuzzleChanged(Puzzle oldPuzzle, Puzzle newPuzzle)
		{
			if (oldPuzzle != null)
			{
				oldPuzzle.AddWord -= _puzzleAddWordHandler;
			}
			if (newPuzzle != null)
			{
				newPuzzle.AddWord += _puzzleAddWordHandler;
			}
		}

		private void OnAddWord(Word word)
		{
			_service.AddWord(word.Value, word.Category);
		}

		#endregion

		#region IHandleWOFEvent Members

		public void PuzzleChanged(string puzzle)
		{
			OnPuzzleChanged(puzzle);
		}

		public void CategoryChanged(string category)
		{
			Console.WriteLine("New Category Is: " + category);
		}

		public void LetterGuessed(string letter)
		{
			Console.WriteLine("Letter Guessed: " + letter);
		}

		public void WheelSpun(int wedge)
		{
			if (wedge == -1)
			{
				Console.WriteLine("Lose a turn");
			}
			else
			{
				Console.WriteLine("Wheel Spun: " + wedge);
			}
		}

		public void PuzzleFinished()
		{
			Console.WriteLine("Puzzle Finished");
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
