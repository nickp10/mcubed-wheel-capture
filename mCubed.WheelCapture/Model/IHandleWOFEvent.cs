
namespace mCubed.WheelCapture.Model
{
	public interface IHandleWOFEvent
	{
		void PuzzleChanged(string puzzle);
		void CategoryChanged(string category);
		void LetterGuessed(string letter);
		void WheelSpun(int wedge);
		void PuzzleFinished();
	}
}
