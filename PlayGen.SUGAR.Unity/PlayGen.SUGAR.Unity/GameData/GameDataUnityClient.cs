using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to gamedata.
	/// </summary>
	public class GameDataUnityClient
	{
		/// <summary>
		/// Get the highest value for this user for the keys provided.
		/// </summary>
		public IEnumerable<EvaluationDataResponse> GetHighest(string[] keys, EvaluationDataType dataType)
        {
			if (SUGARManager.CurrentUser != null)
			{
				var response = SUGARManager.client.GameData.GetHighest(SUGARManager.CurrentUser.Id, SUGARManager.GameId, keys, dataType);
				return response;
			}
			return Enumerable.Empty<EvaluationDataResponse>();
		}

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
