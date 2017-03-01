using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FriendsListItemInterface : MonoBehaviour {

	[SerializeField]
	private Text _actorName;
	[SerializeField]
	private Button _addButton;
	[SerializeField]
	private Button _removeButton;

	internal void SetText(ActorResponseAllowableActions actor, bool pending, bool own = false)
	{
		gameObject.SetActive(true);
		_actorName.text = actor.Actor.Name;
		_addButton.onClick.RemoveAllListeners();
		_removeButton.onClick.RemoveAllListeners();
		_addButton.gameObject.SetActive(actor.CanAdd);
		if (actor.CanAdd)
		{
			if (pending)
			{
				_addButton.onClick.AddListener(delegate { SUGARManager.UserFriend.ManageFriendRequest(actor.Actor.Id, true); });
			}
			else
			{
				_addButton.onClick.AddListener(delegate { SUGARManager.UserFriend.AddFriend(actor.Actor.Id); });
			}
		} 
		_removeButton.gameObject.SetActive(actor.CanRemove);
		if (actor.CanRemove)
		{
			if (pending)
			{
				_removeButton.onClick.AddListener(delegate { SUGARManager.UserFriend.ManageFriendRequest(actor.Actor.Id, false, own); });
			}
			else
			{
				_removeButton.onClick.AddListener(delegate { SUGARManager.UserFriend.RemoveFriend(actor.Actor.Id); });
			}
		}
	}

	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}
