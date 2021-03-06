﻿using PlayGen.SUGAR.Unity;
using System;
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
	/// Button for removing another user, rejecting their request or cancelling a user's request.
	/// </summary>
	[Tooltip("Button for removing another user or rejecting their request")]
	[SerializeField]
	private Button _removeButton;

	/// <summary>
	/// Enable the GameObject, set the text and set up the event listener on the button.
	/// </summary>
	internal void SetText(UserResponseRelationshipStatus actor, Action reload)
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
	}

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}