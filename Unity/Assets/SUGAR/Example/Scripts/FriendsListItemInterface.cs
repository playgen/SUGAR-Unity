using PlayGen.SUGAR.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FriendsListItemInterface : MonoBehaviour {

	/// <summary>
	/// Text for displaying user name.
	/// </summary>
	[Tooltip("Text for displaying user name")]
	[SerializeField]
	private Text _actorName;

	/// <summary>
	/// Button for adding another user or accepting their request.
	/// </summary>
	[Tooltip("Button for adding another user or accepting their request")]
	[SerializeField]
	private Button _addButton;

	/// <summary>
	/// Button for removing another user, rejecting their request or cancelling a user's request.
	/// </summary>
	[Tooltip("Button for removing another user or rejecting their request")]
	[SerializeField]
	private Button _removeButton;

	/// <summary>
	/// Enable the GameObject, set the text and set up the buttons to work in the context they are in.
	/// </summary>
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

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}
