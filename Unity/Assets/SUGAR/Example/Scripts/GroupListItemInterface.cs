using PlayGen.SUGAR.Unity;
using System;
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
	internal void SetText(GroupResponseRelationshipStatus actor, Action reload)
	{
		gameObject.SetActive(true);
		_actorName.text = actor.Actor.Name;
		_addButton.onClick.RemoveAllListeners();
		_removeButton.onClick.RemoveAllListeners();
		_addButton.gameObject.SetActive(actor.RelationshipStatus == RelationshipStatus.NoRelationship || actor.RelationshipStatus == RelationshipStatus.PendingReceivedRequest);
		if (actor.RelationshipStatus == RelationshipStatus.NoRelationship)
		{
			_addButton.onClick.AddListener(() => actor.Add(onComplete =>
			{
				if (onComplete)
				{
					reload?.Invoke();
				}
			}));
		}
		else if (actor.RelationshipStatus == RelationshipStatus.PendingReceivedRequest)
		{
			_addButton.onClick.AddListener(() => actor.UpdateRequest(true, onComplete =>
			{
				if (onComplete)
				{
					reload?.Invoke();
				}
			}));
		}
		_removeButton.gameObject.SetActive(actor.RelationshipStatus != RelationshipStatus.NoRelationship);
		if (actor.RelationshipStatus == RelationshipStatus.ExistingRelationship)
		{
			_removeButton.onClick.AddListener(() => actor.Remove(onComplete =>
			{
				if (onComplete)
				{
					reload?.Invoke();
				}
			}));
		}
		else if (actor.RelationshipStatus == RelationshipStatus.PendingSentRequest)
		{
			_removeButton.onClick.AddListener(() => actor.CancelSentRequest(onComplete =>
			{
				if (onComplete)
				{
					reload?.Invoke();
				}
			}));
		}
		else if (actor.RelationshipStatus == RelationshipStatus.PendingReceivedRequest)
		{
			_removeButton.onClick.AddListener(() => actor.UpdateRequest(false, onComplete =>
			{
				if (onComplete)
				{
					reload?.Invoke();
				}
			}));
		}
		GetComponent<Button>().onClick.RemoveAllListeners();
		GetComponent<Button>().onClick.AddListener(() => SUGARManager.GroupMember.Display(actor.Actor));
	}

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}