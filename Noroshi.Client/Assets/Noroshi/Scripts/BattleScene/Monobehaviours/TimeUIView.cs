namespace Noroshi.BattleScene.MonoBehaviours
{
	public class TimeUIView : UIView, UI.ITimerUIView
	{
		UnityEngine.UI.Text _textUI;
		
		new void Awake()
		{
			base.Awake();
			_textUI = GetComponent<UnityEngine.UI.Text>();
		}
		
		public void UpdateTime(int time)
		{
			_textUI.text = time.ToString();
			_textUI.text = string.Format("{0:00}:{1:00}", time / 60, time % 60);
		}
	}
}