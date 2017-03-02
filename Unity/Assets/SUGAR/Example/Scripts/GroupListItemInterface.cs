using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

public class GroupListItemInterface : MonoBehaviour
{
	/// <summary>
	/// Text for displaying group name.
	/// </summary>
	[Tooltip("Text for displaying group name")]
	[SerializeField]
	private Text _actorName;

	/// <summary>
	/// Button for applying to join a group.
	/// </summary>
	[Tooltip("Button for applying to join a group")]
	[SerializeField]
	private Button _addButton;

	/// <summary>
	/// Button for leaving a group or cancelling a user's request to join.
	/// </summary>
	[Tooltip("Button for leaving a group or cancelling a user's request to join")]
	[SerializeField]
	private Button _removeButton;

	/// <summary>
	/// Enable the GameObject, set the text and set up the buttons to work in the context they are in.
	/// </summary>
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

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}