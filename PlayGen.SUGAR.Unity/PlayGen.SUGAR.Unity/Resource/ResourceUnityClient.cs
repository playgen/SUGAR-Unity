﻿using PlayGen.SUGAR.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to resources.
	/// </summary>
	public class ResourceUnityClient : MonoBehaviour
	{
		[Tooltip("How often (in seconds) the current status of resources is updated.")]
		[SerializeField]
		[Range(0.5f, 30f)]
		private float _resourceCheckRate = 2.5f;

		[Tooltip("Should the current status of game resources not tied to any user also be gathered?")]
		[SerializeField]
		private bool _checkGlobalGameResources;

		[Tooltip("Should the current status of user resources not tied to any game also be gathered?")]
		[SerializeField]
		private bool _checkGlobalUserResources;

		/// <summary>
		/// Resources for the currently signed in user for this game.
		/// </summary>
		public Dictionary<string, long> UserGameResources { get; } = new Dictionary<string, long>();

		/// <summary>
		/// Resources for the game not tied to any user.
		/// </summary>
		public Dictionary<string, long> GlobalGameResources { get; } = new Dictionary<string, long>();

		/// <summary>
		/// Resources for the user not tied to any game.
		/// </summary>
		public Dictionary<string, long> GlobalUserResources { get; } = new Dictionary<string, long>();

		internal void StartCheck()
		{
			InvokeRepeating("UpdateResources", 0, _resourceCheckRate);
		}

		private void UpdateResources()
		{
			Get(result =>
			{
				foreach (var r in result)
				{
					if (UserGameResources.ContainsKey(r.Key))
					{
						UserGameResources[r.Key] = r.Quantity;
					}
					else
					{
						UserGameResources.Add(r.Key, r.Quantity);
					}
				}
			});
			if (_checkGlobalGameResources)
			{
				Get(result =>
				{
					foreach (var r in result)
					{
						if (GlobalGameResources.ContainsKey(r.Key))
						{
							GlobalGameResources[r.Key] = r.Quantity;
						}
						else
						{
							GlobalGameResources.Add(r.Key, r.Quantity);
						}
					}
				}, null, false, null);
			}
			if (_checkGlobalUserResources)
			{
				Get(result =>
				{
					foreach (var r in result)
					{
						if (GlobalUserResources.ContainsKey(r.Key))
						{
							GlobalUserResources[r.Key] = r.Quantity;
						}
						else
						{
							GlobalUserResources.Add(r.Key, r.Quantity);
						}
					}
				}, null, true);
			}
		}

		/// <summary>
		/// Get the resources with the keys provided for the actorId provided (or currently user if left default).
		/// If globalResource is true, resources will be global rather than for the game.
		/// </summary>
		public void Get(Action<List<ResourceResponse>> result, string[] keys = null, bool globalResource = false, int? actorId = -1)
		{
			if (SUGARManager.CurrentUser != null)
			{
				if (actorId == -1)
				{
					actorId = SUGARManager.CurrentUser.Id;
				}
				int? gameId = SUGARManager.GameId;
				if (globalResource)
				{
					gameId = null;
				}
				SUGARManager.client.Resource.GetAsync(gameId, actorId, keys,
				response =>
				{
					result(response.ToList());
				},
				exception =>
				{
					var error = "Failed to gather resources. " + exception.Message;
					Debug.LogError(error);
					result(new List<ResourceResponse>());
				});
			}
		}

		/// <summary>
		/// Transfer the resource with the key provided from the currently signed in user to the user with the provided id.
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </summary>
		public void Transfer(int recipientId, string key, long amount, Action<bool> success, bool globalResource = false)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var request = new ResourceTransferRequest
				{
					SenderActorId = SUGARManager.CurrentUser.Id,
					RecipientActorId = recipientId,
					GameId = globalResource ? 0 : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.TransferAsync(request,
				response =>
				{
					UpdateResources();
					success(true);
				},
				exception =>
				{
					var error = "Failed to transfer resources. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
		}

		/// <summary>
		/// Add the resource with the key provided from the currently signed in user.
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </summary>
		public void Add(string key, long amount, Action<bool> success, bool globalResource = false)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var request = new ResourceChangeRequest {
					ActorId = SUGARManager.CurrentUser.Id,
					GameId = globalResource ? 0 : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.AddAsync(request,
				response =>
				{
					UpdateResources();
					success(true);
				},
				exception =>
				{
					var error = "Failed to add resources. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
		}


		/// <summary>
		/// Set the resource with the key provided from the currently signed in user.
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </summary>
		public void Set(string key, long amount, Action<bool> success, bool globalResource = false)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var request = new ResourceChangeRequest {
					ActorId = SUGARManager.CurrentUser.Id,
					GameId = globalResource ? 0 : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.SetAsync(request,
				response =>
				{
					UpdateResources();
					success(true);
				},
				exception =>
				{
					var error = "Failed to set resources. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
		}
	}
}
