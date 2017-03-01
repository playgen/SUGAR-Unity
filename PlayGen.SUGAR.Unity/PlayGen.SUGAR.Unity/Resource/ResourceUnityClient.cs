using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public class ResourceUnityClient : MonoBehaviour
	{
		public Dictionary<string, long> UserGameResources = new Dictionary<string, long>();
		public Dictionary<string, long> GlobalGameResources = new Dictionary<string, long>();
		public Dictionary<string, long> GlobalUserResources = new Dictionary<string, long>();

		[SerializeField]
		[Range(1f, 30f)]
		private float _resourceCheckRate = 2.5f;

		[SerializeField]
		private bool _checkGlobalGameResources;

		[SerializeField]
		private bool _checkGlobalUserResources;

		internal void StartCheck()
		{
			InvokeRepeating("Update", 0, _resourceCheckRate);
		}

		private void Update()
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
				}, null, false, 0);
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

		public void Get(Action<List<ResourceResponse>> result, string[] keys = null, bool globalResource = false, int actorId = -1)
		{
			if (SUGARManager.CurrentUser != null)
			{
				if (actorId == -1)
				{
					actorId = SUGARManager.CurrentUser.Id;
				}
				SUGARManager.client.Resource.GetAsync(actorId, globalResource ? 0 : SUGARManager.GameId, keys,
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
					Update();
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
