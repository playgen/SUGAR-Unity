using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

public class GroupMemberItemInterface : MonoBehaviour
{
	[SerializeField]
	private Text _actorName;
	[SerializeField]
	private Button _addButton;
	[SerializeField]
	private Button _removeButton;

	internal void SetText(ActorResponseAllowableActions actor)
	{
		gameObject.SetActive(true);
		_actorName.text = actor.Actor.Name;
		_addButton.onClick.RemoveAllListeners();
		_removeButton.onClick.RemoveAllListeners();
		_addButton.gameObject.SetActive(actor.CanAdd);
		if (actor.CanAdd)
		{
			_addButton.onClick.AddListener(delegate { SUGARManager.GroupMember.AddFriend(actor.Actor.Id); });
		}
		_removeButton.gameObject.SetActive(false);
	}

	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}