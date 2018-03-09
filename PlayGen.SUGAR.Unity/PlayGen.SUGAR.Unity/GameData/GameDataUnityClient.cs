using System.Globalization;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to gamedata.
	/// </summary>
	public class GameDataUnityClient
	{
		/// <summary>
		/// Send a piece of gamedata with key and value provided.
		/// </summary>
		public void Send(string key, string value)
		{
			Send(key, value, EvaluationDataType.String);
		}

		/// <summary>
		/// Send a piece of gamedata with key and value provided.
		/// </summary>
		public void Send(string key, long value)
		{
			Send(key, value.ToString(), EvaluationDataType.Long);
		}

		/// <summary>
		/// Send a piece of gamedata with key and value provided.
		/// </summary>
		public void Send(string key, float value)
		{
			Send(key, value.ToString(CultureInfo.InvariantCulture), EvaluationDataType.Float);
		}

		/// <summary>
		/// Send a piece of gamedata with key and value provided.
		/// </summary>
		public void Send(string key, bool value)
		{
			Send(key, value.ToString(), EvaluationDataType.Boolean);
		}

		private void Send(string key, string value, EvaluationDataType dataType)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var data = new EvaluationDataRequest
				{
					CreatingActorId = SUGARManager.CurrentUser.Id,
					GameId = SUGARManager.GameId,
					Key = key,
					Value = value,
					EvaluationDataType = dataType
				};

				SUGARManager.client.GameData.AddAsync(data,
				response =>
				{
					Debug.Log("GameData Sending Success: True");
				},
				exception =>
				{
					Debug.LogError("GameData Sending Success: False. Exception: " + exception);
				});
			}
		}
	}
}
