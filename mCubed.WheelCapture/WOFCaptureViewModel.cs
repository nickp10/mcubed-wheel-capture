using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace mCubed.WheelCapture
{
	public class WOFCaptureViewModel : INotifyPropertyChanged
	{
		#region Data Members

		private readonly PuzzleAnalyzer _analyzer;
		private readonly PuzzleCalculator _calculator;
		private readonly ObservableCollection<Category> _categories;
		private readonly CategorySerializer _serializer;
		private Puzzle _currentPuzzle;
		private readonly ObservableCollection<Puzzle> _previousPuzzles = new ObservableCollection<Puzzle>();

		#endregion

		#region Constructors

		public WOFCaptureViewModel()
		{
			_calculator = new PuzzleCalculator();
			_serializer = new CategorySerializer();
			_categories = new ObservableCollection<Category>(_serializer.ReadCategories());
			_analyzer = new PuzzleAnalyzer(_categories);
			CalculatePuzzle();
		}

		#endregion

		#region Properties

		public PuzzleAnalyzer Analyzer
		{
			get { return _analyzer; }
		}

		public ObservableCollection<Category> Categories
		{
			get { return _categories; }
		}

		public Puzzle CurrentPuzzle
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

		public ObservableCollection<Puzzle> PreviousPuzzles
		{
			get { return _previousPuzzles; }
		}

		#endregion

		#region Methods

		private void CalculatePuzzle()
		{
			ThreadPool.QueueUserWorkItem(q =>
			{
				while (true)
				{
					using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
					{
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
							var puzzle = _calculator.GetPuzzle(bitmap);
							ThreadPool.QueueUserWorkItem(q2 =>
							{
								OnPuzzleChanged(puzzle);
							});
						}
					}
				}
			});
		}

		private void OnPuzzleChanged(string puzzle)
		{
			if (!string.IsNullOrEmpty(puzzle))
			{
				lock (this)
				{
					if (CurrentPuzzle == null)
					{
						CurrentPuzzle = new Puzzle(puzzle, Categories);
					}
					else
					{
						if (puzzle.All(c => !char.IsLetter(c)))
						{
							if (CurrentPuzzle.CurrentPuzzle != puzzle)
							{
								// It's a new puzzle.
								System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
								{
									if (!PreviousPuzzles.Contains(CurrentPuzzle))
									{
										PreviousPuzzles.Add(CurrentPuzzle);
									}
								}));
								CurrentPuzzle = new Puzzle(puzzle, Categories);
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

		private void OnCurrentPuzzleChanged()
		{
			PropertyChangedEventHandler handler = null;
			var puzzle = CurrentPuzzle;
			if (puzzle != null)
			{
				handler = (s, e) =>
				{
					if (e.PropertyName == "IsCompleted" && puzzle.IsCompleted)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
						{
							PreviousPuzzles.Add(puzzle);
						}));
						puzzle.PropertyChanged -= handler;
					}
				};
				puzzle.PropertyChanged += handler;
			}
		}

		public void SavePuzzles()
		{
			_serializer.WriteCategories(Categories);
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
