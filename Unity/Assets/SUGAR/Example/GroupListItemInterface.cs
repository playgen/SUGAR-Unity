using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

public class GroupListItemInterface : MonoBehaviour
{
	[SerializeField]
	private Text _actorName;
	[SerializeField]
	private Button _addButton;
	[SerializeField]
	private Button _removeButton;

	internal void SetText(ActorResponseAllowableActions actor, bool pending, bool member)
	{
		gameObject.SetActive(true);
		_actorName.text = actor.Actor.Name;
		_addButton.onClick.RemoveAllListeners();
		_removeButton.onClick.RemoveAllListeners();
		_addButton.gameObject.SetActive(actor.CanAdd);
		if (actor.CanAdd)
		{
			_addButton.onClick.AddListener(delegate { SUGARManager.UserGroup.AddGroup(actor.Actor.Id); });
		}
		_removeButton.gameObject.SetActive(actor.CanRemove);
		if (actor.CanRemove)
		{
			if (pending)
			{
				_removeButton.onClick.AddListener(delegate { SUGARManager.UserGroup.ManageGroupRequest(actor.Actor.Id, false); });
			}
			else
			{
				_removeButton.onClick.AddListener(delegate { SUGARManager.UserGroup.RemoveGroup(actor.Actor.Id); });
			}
		}
		GetComponent<Button>().onClick.RemoveAllListeners();
		GetComponent<Button>().onClick.AddListener(delegate { SUGARManager.GroupMember.Display(actor.Actor); });
	}

	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}