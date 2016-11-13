using System.ComponentModel;

namespace mCubed.WheelCapture.Model
{
	public class Letter : INotifyPropertyChanged
	{
		#region Properties

		private string _display;
		public string Display
		{
			get { return _display; }
			set
			{
				if (_display != value)
				{
					_display = value;
					OnPropertyChanged("Display");
				}
			}
		}

		private bool _isUsed;
		public bool IsUsed
		{
			get { return _isUsed; }
			set
			{
				if (_isUsed != value)
				{
					_isUsed = value;
					OnPropertyChanged("IsUsed");
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
