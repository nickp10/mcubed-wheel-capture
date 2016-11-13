using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using mCubed.Services.Core;
using mCubed.WheelCapture.Model;

namespace mCubed.WheelCapture.ViewModel
{
	public class WOFCaptureViewModel : IHandleWOFEvent, INotifyPropertyChanged
	{
		#region Data Members

		private readonly PuzzleAnalyzer _analyzer;
		private readonly WheelWordService _service;
		private readonly ObservableCollection<Word> _words;
		private readonly WebSocketMessageParser _parser;

		#endregion

		#region Constructors

		public WOFCaptureViewModel()
		{
			_service = new WheelWordService();
			_words = new ObservableCollection<Word>(_service.WordList.Select(w => new Word(w.Category, w.Word)));
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

		private string _currentWedge;
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
					// TODO: ADD GUESSED LETTER
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
						_service.AddWord(puzzle.CurrentPuzzle, puzzle.Category);
						Application.Current.Dispatcher.BeginInvoke(new Action(() =>
						{
							Words.Add(new Word(puzzle.Category, puzzle.CurrentPuzzle));
						}));
					}
					CurrentPuzzle = null;
					CurrentWedge = null;
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
