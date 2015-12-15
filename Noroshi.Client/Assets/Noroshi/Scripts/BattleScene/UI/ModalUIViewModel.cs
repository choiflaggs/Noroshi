using UniRx;

namespace Noroshi.BattleScene.UI
{
	public class ModalUIViewModel : AbstractModalUIViewModel<IModalUIView>
	{
		readonly string _name;
		public ModalUIViewModel(string name)
		{
			_name = name;
		}
		protected override IObservable<IModalUIView> _loadView()
		{
			return SceneContainer.GetFactory().BuildModalUIView(_name);
		}
	}
}