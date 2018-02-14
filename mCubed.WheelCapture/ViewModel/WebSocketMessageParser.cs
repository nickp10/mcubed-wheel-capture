using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fiddler;
using mCubed.WheelCapture.Capture;
using mCubed.WheelCapture.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mCubed.WheelCapture.ViewModel
{
	public class WebSocketMessageParser
	{
		#region Data Members

		private readonly IHandleWOFEvent _handler;

		#endregion

		#region Constructors

		public WebSocketMessageParser(IHandleWOFEvent handler)
		{
			_handler = handler;
			Capturer.WebSocketCaptured += OnWebSocketCaptured;
		}

		#endregion

		#region Methods

		private void OnWebSocketCaptured(Session session, WebSocketMessage message)
		{
			var url = session.fullUrl;
			if (url != null && url.Contains("worldwinner.com"))
			{
				var payload = message.PayloadAsString();
				if (payload != null)
				{
					OnPayloadCaptured(payload);
				}
			}
		}

		private void OnPayloadCaptured(string payload)
		{
			var jsonIndex = payload.IndexOf('{');
			if (jsonIndex >= 0)
			{
				payload = payload.Substring(jsonIndex);
				if (!string.IsNullOrEmpty(payload))
				{
					var json = JsonConvert.DeserializeObject(payload) as JObject;
					if (json != null)
					{
						OnJsonPayloadCaptured(json);
					}
				}
			}
		}

		private void OnJsonPayloadCaptured(JObject json)
		{
			JToken name;
			if (json.TryGetValue("name", out name))
			{
				var nameStr = (string)name;
				if (nameStr == "data" || nameStr == "input")
				{
					OnJsonArgsCaptured(json);
				}
			}
		}

		private void OnJsonArgsCaptured(JObject json)
		{
			JToken args;
			if (json.TryGetValue("args", out args))
			{
				var argsArray = args as JArray;
				if (argsArray != null)
				{
					foreach (var arg in argsArray)
					{
						var argObject = arg as JObject;
						if (argObject != null)
						{
							OnJsonArgCaptured(argObject);
						}
					}
				}
			}
		}

		private void OnJsonArgCaptured(JObject arg)
		{
			JToken clue;
			if (arg.TryGetValue("clue", out clue))
			{
				_handler.CategoryChanged((string)clue);
			}

			JToken board;
			if (arg.TryGetValue("board", out board))
			{
				OnJsonBoardCaptured(board);
			}

			JToken solution;
			if (arg.TryGetValue("solution", out solution))
			{
				if (OnJsonBoardCaptured(solution))
				{
					_handler.PuzzleFinished();
				}
			}

			JToken guess;
			if (arg.TryGetValue("guess", out guess))
			{
				var guessStr = CleanUpBoard((string)guess);
				_handler.PuzzleChanged(guessStr);
			}

			JToken wedge;
			if (arg.TryGetValue("wedge", out wedge))
			{
				_handler.WheelSpun((int)wedge);
			}

			JToken letter;
			if (arg.TryGetValue("letter", out letter))
			{
				_handler.LetterGuessed((string)letter);
			}

			JToken messageCommand;
			if (arg.TryGetValue("messageCommand", out messageCommand))
			{
				var messageCommandStr = (string)messageCommand;
				if (messageCommandStr == "startBonusRound")
				{
					_handler.LetterGuessed("R");
					_handler.LetterGuessed("S");
					_handler.LetterGuessed("T");
					_handler.LetterGuessed("L");
					_handler.LetterGuessed("N");
					_handler.LetterGuessed("E");
				}
				else if (messageCommandStr == "endOfGame" || messageCommandStr == "voluntaryEndGame")
				{
					_handler.PuzzleFinished();
				}
			}

			JToken correct;
			if (arg.TryGetValue("correct", out correct))
			{
				if ((bool)correct)
				{
					_handler.PuzzleFinished();
				}
			}
		}

		private bool OnJsonBoardCaptured(JToken board)
		{
			var boardArrArr = board as JArray;
			if (boardArrArr != null)
			{
				var boardBuilder = new StringBuilder();
				foreach (var boardArr in boardArrArr.OfType<JArray>())
				{
					foreach (var boardItem in boardArr)
					{
						boardBuilder.Append((string)boardItem);
					}
				}
				var boardStr = CleanUpBoard(boardBuilder.ToString());
				_handler.PuzzleChanged(boardStr);
				return true;
			}
			return false;
		}

		private string CleanUpBoard(string board)
		{
			board = board.Replace('%', ' ');
			board = board.Trim();
			board = Regex.Replace(board, @"\s+", " ");
			return board;
		}

		#endregion
	}
}
