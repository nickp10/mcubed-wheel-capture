using System.Windows;
using mCubed.WheelCapture.Capture;

namespace mCubed.WheelCapture
{
	public partial class App : Application
	{
		#region Overrides

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			Capturer.StartListening();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Capturer.StopListening();
		}

		#endregion
	}
}
