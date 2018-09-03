using PlayGen.SUGAR.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Authorization;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this to get current resources, add resources and send resources to other users
	/// </summary>
	public class ResourceUnityClient : MonoBehaviour
	{
		[Tooltip("How often (in seconds) the local resource cache is updated.")]
		[SerializeField]
		[Range(0.5f, 30f)]
		private float _resourceCheckRate = 2.5f;

		/// <value>
		/// Resources for the currently signed in user for this game.
		/// </value>
		private Dictionary<string, long> UserGameResources { get; } = new Dictionary<string, long>();

		/// <value>
		/// Resources for the user not tied to any game.
		/// </value>
		private Dictionary<string, long> GlobalUserResources { get; } = new Dictionary<string, long>();

		internal void StartCheck(Action callback)
		{
			InvokeRepeating(nameof(UpdateResources), _resourceCheckRate, _resourceCheckRate);
			UpdateResources(callback);
		}

		private void UpdateResources()
		{
			UpdateResources(null);
		}

		private void UpdateResources(Action callback = null)
		{
			var didGetGlobal = false;
			var didGetGame = false;

			// Game
			GetFromServer((result, values) =>
			{
				didGetGame = true;
				if (didGetGame && didGetGlobal)
				{
					callback?.Invoke();
				}
			});

			// Global
			GetFromServer((result, values) =>
			{
				didGetGlobal = true;
				if (didGetGame && didGetGlobal)
				{
					callback?.Invoke();
				}
			}, null, true);
		}

		/// <summary>
		/// Get the current resource amount for the current user from the local cache. Cache is updated at the rate set in the Inspector.
		/// </summary>
		/// <param name="key">Resource key value is being gathered for</param>
		/// <param name="globalResource">**Optional** Get value for a global resource rather than one for this game. (default: false)</param>
		/// <remarks>
		/// - If globalResource is true, resource will be global rather than for the game.
		/// </remarks>
		public long GetFromCache(string key, bool globalResource = false)
		{
			if (globalResource)
			{
				GlobalUserResources.TryGetValue(key, out var value);
				return value;
			}
			else
			{
				UserGameResources.TryGetValue(key, out var value);
				return value;
			}
		}

		/// <summary>
		/// Get the resources with the keys provided for the current user directly from the server.
		/// </summary>
		/// <param name="result">Callback which will return whether the call to the server was successful and a dictionary of all the keys and their current values</param>
		/// <param name="keys">Resource keys values are being gathered for</param>
		/// <param name="globalResource">**Optional** Get resource values for global resources rather than one for this game. (default: false)</param>
		/// /// <remarks>
		/// - If globalResource is true, resource will be global rather than for the game.
		/// </remarks>
		public void GetFromServer(Action<bool, Dictionary<string, long>> result, string[] keys = null, bool globalResource = false)
		{
			if (SUGARManager.UserSignedIn)
			{
				var gameId = SUGARManager.GameId;
				if (globalResource)
				{
					gameId = Platform.GlobalGameId;
				}
				SUGARManager.client.Resource.GetAsync(gameId, SUGARManager.CurrentUser.Id, keys,
				response =>
				{
					foreach (var r in response)
					{
						UpdateResource(r);
					}
					result(true, response.ToDictionary(r => r.Key, r => GetFromCache(r.Key, globalResource)));
				},
				exception =>
				{
					Debug.LogError($"Failed to gather resources. {exception}");
					result(false, keys.ToDictionary(k => k, k => (long)0));
				});
			}
		}

		/// <summary>
		/// Transfer the resource with the key provided from the currently signed in user
		/// </summary>
		/// <param name="recipientId">Id of the actor who will receive the resource</param>
		/// <param name="key">Name of the resource being transferred</param>
		/// <param name="amount">The amount being transferred</param>
		/// <param name="onComplete">Callback which returns whether the transfer was a success and the current value of the resource that was transferred</param>
		/// <param name="globalResource">**Optional** Setting for if the resource is global rather than for this game. (default: false)</param>
		/// <remarks>
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </remarks>
		public void Transfer(int recipientId, string key, long amount, Action<bool, long> onComplete, bool globalResource = false)
		{
			if (SUGARManager.UserSignedIn)
			{
				var request = new ResourceTransferRequest
				{
					SenderActorId = SUGARManager.CurrentUser.Id,
					RecipientActorId = recipientId,
					GameId = globalResource ? Platform.GlobalGameId : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.TransferAsync(request,
				response =>
				{
					UpdateResource(response.FromResource);
					UpdateResource(response.ToResource);
					onComplete(true, GetFromCache(key, globalResource));
				},
				exception =>
				{
					Debug.LogError($"Failed to transfer resources. {exception}");
					GetFromServer(
					(getSuccess, getValues) =>
					{
						onComplete(false, GetFromCache(key, globalResource));
					}, new[] { key }, globalResource);
				});
			}
		}

		/// <summary>
		/// Transfer the resource with the key provided to the currently signed in user
		/// </summary>
		/// <param name="senderId">Id of the actor who will send the resource</param>
		/// <param name="key">Name of the resource being transferred</param>
		/// <param name="amount">The amount being transferred</param>
		/// <param name="onComplete">Callback which returns whether the transfer was a success and the current value of the resource that was transferred</param>
		/// <param name="globalResource">**Optional** Setting for if the resource is global rather than for this game. (default: false)</param>
		/// <remarks>
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </remarks>
		public void TryTake(int senderId, string key, long amount, Action<bool, long> onComplete, bool globalResource = false)
		{
			if (SUGARManager.UserSignedIn)
			{
				var request = new ResourceTransferRequest
				{
					RecipientActorId = SUGARManager.CurrentUser.Id,
					SenderActorId = senderId,
					GameId = globalResource ? Platform.GlobalGameId : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.TransferAsync(request,
				response =>
				{
					UpdateResource(response.FromResource);
					UpdateResource(response.ToResource);
					onComplete(true, GetFromCache(key, globalResource));
				},
				exception =>
				{
					Debug.LogError($"Failed to transfer resources. {exception}");
					GetFromServer(
					(getSuccess, getValues) =>
					{
						onComplete(false, GetFromCache(key, globalResource));
					}, new[] { key }, globalResource);
				});
			}
		}

		/// <summary>
		/// Add the resource with the key provided to the currently signed in user
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </summary>
		/// <param name="key">Name of the resource being transferred</param>
		/// <param name="amount">The amount being transferred</param>
		/// <param name="onComplete">Callback which returns whether the transfer was a success and the current value of the resource that was transferred</param>
		/// <param name="globalResource">**Optional** Setting for if the resource is global rather than for this game. (default: false)</param>
		/// <remarks>
		/// If globalResource is true, resource transferred will be global rather than for the game.
		/// </remarks>
		public void Add(string key, long amount, Action<bool, long> onComplete, bool globalResource = false)
		{
			if (SUGARManager.UserSignedIn)
			{
				var request = new ResourceAddRequest
				{
					ActorId = SUGARManager.CurrentUser.Id,
					GameId = globalResource ? Platform.GlobalGameId : SUGARManager.GameId,
					Key = key,
					Quantity = amount
				};
				SUGARManager.client.Resource.AddOrUpdateAsync(request,
				response =>
				{
					UpdateResource(response);
					onComplete(true, GetFromCache(key, globalResource));
				},
				exception =>
				{
					Debug.LogError($"Failed to add resources. {exception}");
					GetFromServer(
					(getSuccess, getValues) =>
					{
						onComplete(false, GetFromCache(key, globalResource));
					}, new[] { key }, globalResource);
				});
			}
		}

		private void UpdateResource(ResourceResponse response)
		{
			if (response.ActorId == SUGARManager.CurrentUser?.Id)
			{
				if (response.GameId == Platform.GlobalGameId)
				{
					if (GlobalUserResources.ContainsKey(response.Key))
					{
						GlobalUserResources[response.Key] = response.Quantity;
					}
					else
					{
						GlobalUserResources.Add(response.Key, response.Quantity);
					}
				}
				else
				{
					if (UserGameResources.ContainsKey(response.Key))
					{
						UserGameResources[response.Key] = response.Quantity;
					}
					else
					{
						UserGameResources.Add(response.Key, response.Quantity);
					}
				}
			}
		}

		internal void ResetClient()
		{
			UserGameResources.Clear();
			GlobalUserResources.Clear();
		}
	}
}
