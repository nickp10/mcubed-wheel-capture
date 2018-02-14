using System;
using System.Collections.Generic;
using System.IO;

namespace mCubed.WheelCapture.ViewModel
{
	public static class Settings
	{
		#region Data Members

		private static readonly IDictionary<string, string> _settings = new Dictionary<string, string>();

		#endregion

		#region Helpers

		private static void ReadSettings()
		{
			if (_settings.Count == 0)
			{
				var lines = File.ReadAllLines(AppDomain.CurrentDomain.FriendlyName + ".settings");
				foreach (var line in lines)
				{
					var index = line.IndexOf('=');
					if (index > -1)
					{
						string key = line.Substring(0, index);
						string value = line.Substring(index + 1);
						_settings[key] = value;
					}
				}
			}
		}

		#endregion

		#region Properties

		public static string PersistenceAppName
		{
			get
			{
				ReadSettings();
				return _settings["PersistenceAppName"];
			}
		}

		public static string PersistenceAppKey
		{
			get
			{
				ReadSettings();
				return _settings["PersistenceAppKey"];
			}
		}

		public static string PersistenceServer
		{
			get
			{
				ReadSettings();
				return _settings["PersistenceServer"];
			}
		}

		public static string PersistencePort
		{
			get
			{
				ReadSettings();
				return _settings["PersistencePort"];
			}
		}

		public static string WebSocketURLFilter
		{
			get
			{
				ReadSettings();
				return _settings["WebSocketURLFilter"];
			}
		}

		#endregion
	}
}
