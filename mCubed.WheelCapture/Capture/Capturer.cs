using System;
using Fiddler;

namespace mCubed.WheelCapture.Capture
{
	/// <summary>
	/// Defines a class that will monitor HTTP/HTTPS requests and responses.
	/// </summary>
	public static class Capturer
	{
		#region Events

		/// <summary>
		/// Event that notifies when a web socket message has been captured.
		/// </summary>
		public static event Action<Session, WebSocketMessage> WebSocketCaptured;

		#endregion

		#region Methods

		/// <summary>
		/// Starts listening for HTTP/HTTPS requests/responses.
		/// </summary>
		public static void StartListening()
		{
			CertificateManager.CreateCertificate();
			FiddlerApplication.OnWebSocketMessage += new EventHandler<WebSocketMessageEventArgs>(OnWebSocketMessage);
			FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
			FiddlerApplication.Startup(0, FiddlerCoreStartupFlags.Default);
		}

		/// <summary>
		/// Stops listening for HTTP/HTTPS requests/responses.
		/// </summary>
		public static void StopListening()
		{
			FiddlerApplication.OnWebSocketMessage -= new EventHandler<WebSocketMessageEventArgs>(OnWebSocketMessage);
			FiddlerApplication.Shutdown();
			CertificateManager.DeleteAllCertificates();
		}

		/// <summary>
		/// Called when a web socket message as been captured. This will notify others
		/// via the statically available event.
		/// </summary>
		/// <param name="sender">The web socket session that is captured.</param>
		/// <param name="e">The event argument containing the web socket message that was captured.</param>
		private static void OnWebSocketMessage(object sender, WebSocketMessageEventArgs e)
		{
			var handler = WebSocketCaptured;
			var session = sender as Session;
			var message = e == null ? null : e.oWSM;
			if (handler != null && session != null && message != null)
			{
				handler(session, message);
			}
		}

		#endregion
	}
}
