
namespace mCubed.WheelCapture.Model
{
	public interface IHandleWheelEvent
	{
		void PuzzleChanged(string puzzle);
		void CategoryChanged(string category);
		void LetterGuessed(string letter);
		void WheelSpun(int wedge);
		void PuzzleFinished();
	}
}
