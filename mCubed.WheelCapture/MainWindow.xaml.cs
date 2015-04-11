using System.Windows;

namespace mCubed.WheelCapture
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			DataContext = new WOFCaptureViewModel();
			InitializeComponent();
		}
	}
}
