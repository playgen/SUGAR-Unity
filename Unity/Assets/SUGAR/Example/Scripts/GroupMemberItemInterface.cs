using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

public class GroupMemberItemInterface : MonoBehaviour
{
	/// <summary>
	/// Text for displaying user name.
	/// </summary>
	[Tooltip("Text for displaying user name")]
	[SerializeField]
	private Text _actorName;

	/// <summary>
	/// Button for adding another user.
	/// </summary>
	[Tooltip("Button for adding another user")]
	[SerializeField]
	private Button _addButton;

	/// <summary>
	/// Enable the GameObject, set the text and set up the event listener on the button.
	/// </summary>
	internal void SetText(ActorResponseAllowableActions actor)
	{
		gameObject.SetActive(true);
		_actorName.text = actor.Actor.Name;
		_addButton.onClick.RemoveAllListeners();
		_addButton.gameObject.SetActive(actor.CanAdd);
		if (actor.CanAdd)
		{
			_addButton.onClick.AddListener(delegate { SUGARManager.GroupMember.AddFriend(actor.Actor.Id); });
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