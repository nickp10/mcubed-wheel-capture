using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mCubed.WheelCapture
{
	public class PuzzleCalculator
	{
		private const int COLOR_THRESHOLD = 25;
		private Point? _lastBoardCorner;
		private static readonly int[] _boardXOffsets = new[] { 119, 153, 186, 220, 253, 287, 320, 354, 387, 420, 454, 488, 521, 554 };
		private static readonly int[] _boardYOffsets = new[] { 116, 160, 204, 247 };
		private static readonly IDictionary<string, Bitmap> _letterBitmaps = new Dictionary<string, Bitmap>();
		private static readonly IEnumerable<KeyValuePair<string, char>> _letters = new[]
		{
			new KeyValuePair<string, char>("Blank.bmp", '_'),
			new KeyValuePair<string, char>("R.bmp", 'R'),
			new KeyValuePair<string, char>("T.bmp", 'T'),
			new KeyValuePair<string, char>("N.bmp", 'N'),
			new KeyValuePair<string, char>("S.bmp", 'S'),
			new KeyValuePair<string, char>("L.bmp", 'L'),
			new KeyValuePair<string, char>("C.bmp", 'C'),
			new KeyValuePair<string, char>("D.bmp", 'D'),
			new KeyValuePair<string, char>("H.bmp", 'H'),
			new KeyValuePair<string, char>("G.bmp", 'G'),
			new KeyValuePair<string, char>("P.bmp", 'P'),
			new KeyValuePair<string, char>("M.bmp", 'M'),
			new KeyValuePair<string, char>("B.bmp", 'B'),
			new KeyValuePair<string, char>("F.bmp", 'F'),
			new KeyValuePair<string, char>("Y.bmp", 'Y'),
			new KeyValuePair<string, char>("K.bmp", 'K'),
			new KeyValuePair<string, char>("W.bmp", 'W'),
			new KeyValuePair<string, char>("V.bmp", 'V'),
			new KeyValuePair<string, char>("J.bmp", 'J'),
			new KeyValuePair<string, char>("Z.bmp", 'Z'),
			new KeyValuePair<string, char>("Q.bmp", 'Q'),
			new KeyValuePair<string, char>("X.bmp", 'X'),
			new KeyValuePair<string, char>("Hyphen.bmp", '-'),
			new KeyValuePair<string, char>("Ampersand.bmp", '&'),
			new KeyValuePair<string, char>("Apostrophe.bmp", '\''),
			new KeyValuePair<string, char>("Question.bmp", '?'),
			new KeyValuePair<string, char>("Exclamation.bmp", '!'),
			new KeyValuePair<string, char>("Period.bmp", '.'),
			new KeyValuePair<string, char>("BlankHighlighted.bmp", '_'),
			new KeyValuePair<string, char>("A.bmp", 'A'),
			new KeyValuePair<string, char>("E.bmp", 'E'),
			new KeyValuePair<string, char>("I.bmp", 'I'),
			new KeyValuePair<string, char>("O.bmp", 'O'),
			new KeyValuePair<string, char>("U.bmp", 'U')
		};

		public string GetPuzzle(Bitmap boardBitmap)
		{
			var puzzleBuilder = new StringBuilder();
			var corner = FindBoardCorner(boardBitmap);
			if (corner.X != 0 || corner.Y != 0)
			{
				foreach (var boardYOffset in _boardYOffsets)
				{
					foreach (var boardXOffset in _boardXOffsets)
					{
						puzzleBuilder.Append(GetLetter(boardBitmap, boardXOffset + corner.X, boardYOffset + corner.Y));
					}
					puzzleBuilder.Append(' ');
				}
				return Regex.Replace(puzzleBuilder.ToString().Trim(), @"\s+", " ");
			}
			return string.Empty;
		}

		private char GetLetter(Bitmap boardBitmap, int x, int y)
		{
			foreach (var letter in _letters)
			{
				if (LetterMatches(boardBitmap, x, y, letter.Key))
				{
					return letter.Value;
				}
			}
			return ' ';
		}

		private bool LetterMatches(Bitmap boardBitmap, int boardXOffset, int boardYOffset, string letter)
		{
			var letterBitmap = ReadLetterBitmap(letter);
			for (int i = 0; i < 2; i++)
			{
				var equals = true;
				for (int x = 0; x < letterBitmap.Width - i; x++)
				{
					for (int y = 0; y < letterBitmap.Height; y++)
					{
						var boardPixel = boardBitmap.GetPixel(boardXOffset + x + i, boardYOffset + y);
						var letterPixel = letterBitmap.GetPixel(x, y);
						if (!AreColorsEquivalent(boardPixel, letterPixel))
						{
							equals = false;
							goto EqualsCheck;
						}
					}
				}
			EqualsCheck:
				if (equals)
				{
					return true;
				}
			}
			return false;
		}

		private Point FindBoardCorner(Bitmap bitmap)
		{
			if (_lastBoardCorner != null)
			{
				var color = bitmap.GetPixel(_lastBoardCorner.Value.X, _lastBoardCorner.Value.Y);
				if (color.B == 207 && color.G == 0 && color.R == 0)
				{
					return _lastBoardCorner.Value;
				}
			}
			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					var color = bitmap.GetPixel(x, y);
					if (color.B == 207 && color.G == 0 && color.R == 0)
					{
						if (x + _boardXOffsets.Last() < bitmap.Width && y + _boardYOffsets.Last() < bitmap.Height)
						{
							_lastBoardCorner = new Point(x, y);
							return _lastBoardCorner.Value;
						}
					}
				}
			}
			_lastBoardCorner = null;
			return new Point();
		}

		private bool AreColorsEquivalent(Color color1, Color color2)
		{
			return Math.Abs(color1.R - color2.R) < COLOR_THRESHOLD && Math.Abs(color1.G - color2.G) < COLOR_THRESHOLD && Math.Abs(color1.B - color2.B) < COLOR_THRESHOLD;
		}

		private Bitmap ReadLetterBitmap(string letter)
		{
			Bitmap bitmap;
			if (_letterBitmaps.TryGetValue(letter, out bitmap))
			{
				return bitmap;
			}
			using (var letterStream = System.Windows.Application.GetResourceStream(new Uri("Letters\\" + letter, UriKind.Relative)).Stream)
			{
				var letterBitmap = new Bitmap(letterStream);
				_letterBitmaps[letter] = letterBitmap;
				return letterBitmap;
			}
		}
	}
}
