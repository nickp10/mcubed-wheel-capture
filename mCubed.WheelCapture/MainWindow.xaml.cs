using System.Windows;
using mCubed.WheelCapture.ViewModel;

namespace mCubed.WheelCapture
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			DataContext = new WheelCaptureViewModel();
			InitializeComponent();
		}
	}
}
