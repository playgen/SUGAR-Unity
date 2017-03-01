using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public class ResourceUnityClient : MonoBehaviour
	{
		[SerializeField]
		[Range(0.5f, 30f)]
		private float _resourceCheckRate = 2.5f;

		[SerializeField]
		private bool _checkGlobalGameResources;

		[SerializeField]
		private bool _checkGlobalUserResources;

		private readonly Dictionary<string, long> _userGameResources = new Dictionary<string, long>();
		private readonly Dictionary<string, long> _globalGameResources = new Dictionary<string, long>();
		private readonly Dictionary<string, long> _globalUserResources = new Dictionary<string, long>();

		public Dictionary<string, long> UserGameResources => _userGameResources;
		public Dictionary<string, long> GlobalGameResources => _globalGameResources;
		public Dictionary<string, long> GlobalUserResources => _globalUserResources;

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
					if (_userGameResources.ContainsKey(r.Key))
					{
						_userGameResources[r.Key] = r.Quantity;
					}
					else
					{
						_userGameResources.Add(r.Key, r.Quantity);
					}
				}
			});
			if (_checkGlobalGameResources)
			{
				Get(result =>
				{
					foreach (var r in result)
					{
						if (_globalGameResources.ContainsKey(r.Key))
						{
							_globalGameResources[r.Key] = r.Quantity;
						}
						else
						{
							_globalGameResources.Add(r.Key, r.Quantity);
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
						if (_globalUserResources.ContainsKey(r.Key))
						{
							_globalUserResources[r.Key] = r.Quantity;
						}
						else
						{
							_globalUserResources.Add(r.Key, r.Quantity);
						}
					}
				}, null, true);
			}
		}

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
					string error = "Failed to gather resources. " + exception.Message;
					Debug.LogError(error);
					result(new List<ResourceResponse>());
				});
			}
		}

		public void Transfer(int recipientId, string key, long amount, Action<bool> success, bool globalResource = false)
		{
			if (SUGARManager.CurrentUser != null)
			{
				ResourceTransferRequest request = new ResourceTransferRequest
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
					string error = "Failed to transfer resources. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
		}
	}
}
