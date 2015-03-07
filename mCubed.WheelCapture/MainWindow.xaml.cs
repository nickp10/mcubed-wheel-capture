using System.ComponentModel;
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

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			((WOFCaptureViewModel)DataContext).SavePuzzles();
		}
	}
}
