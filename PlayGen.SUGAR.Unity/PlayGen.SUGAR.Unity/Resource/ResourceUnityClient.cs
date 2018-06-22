using PlayGen.SUGAR.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Authorization;

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

		[Tooltip("Should the current status of user resources not tied to any game also be gathered?")]
		[SerializeField]
		private bool _checkGlobalUserResources;

		/// <summary>
		/// Resources for the currently signed in user for this game.
		/// </summary>
		public Dictionary<string, long> UserGameResources { get; } = new Dictionary<string, long>();

		/// <summary>
		/// Resources for the user not tied to any game.
		/// </summary>
		public Dictionary<string, long> GlobalUserResources { get; } = new Dictionary<string, long>();

		internal void StartCheck()
		{
			InvokeRepeating(nameof(UpdateResources), 0, _resourceCheckRate);
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
		/// Get the resources with the keys provided for the signed in user.
		/// If globalResource is true, resources will be global rather than for the game.
		/// </summary>
		public void Get(Action<List<ResourceResponse>> result, string[] keys = null, bool globalResource = false)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var gameId = SUGARManager.GameId;
				if (globalResource)
				{
					gameId = Platform.GlobalId;
				}
				SUGARManager.client.Resource.GetAsync(gameId, SUGARManager.CurrentUser.Id, keys,
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
					GameId = globalResource ? Platform.GlobalId : SUGARManager.GameId,
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
				var request = new ResourceAddRequest {
					ActorId = SUGARManager.CurrentUser.Id,
					GameId = globalResource ? Platform.GlobalId : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.AddOrUpdateAsync(request,
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
	}
}
