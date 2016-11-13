using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace mCubed.WheelCapture.Model
{
	public class Puzzle : INotifyPropertyChanged
	{
		#region Data Members

		private readonly IEnumerable<Word> _allSolutions;

		#endregion

		#region Constructors

		public Puzzle(IEnumerable<Word> allSolutions)
		{
			_allSolutions = allSolutions;
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

		private void OnCurrentPuzzleChanged()
		{
			var puzzle = CurrentPuzzle;
			if (string.IsNullOrEmpty(puzzle))
			{
				PotentialSolutions = null;
			}
			else
			{
				var potentialSolutions = PotentialSolutions;
				if (potentialSolutions == null)
				{
					potentialSolutions = _allSolutions;
				}
				var regex = BuildFormatRegex(puzzle);
				PotentialSolutions = potentialSolutions.Where(c => regex.IsMatch(c.Value)).ToArray();
			}
		}

		private Regex BuildFormatRegex(string puzzle)
		{
			return new Regex("^" + Regex.Escape(puzzle).Replace("_", "[A-Z_]") + "$");
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
