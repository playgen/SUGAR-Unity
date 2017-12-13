using System;
using System.Collections.Generic;
using System.Globalization;
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
		public void Get(Action<IEnumerable<EvaluationDataResponse>> success, string[] keys = null)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GameData.GetAsync(SUGARManager.CurrentUser.Id, SUGARManager.GameId, keys,
				success,
				exception =>
				{
					Debug.LogError(exception.Message);
					success(null);
				});
			}
		}

		/// <summary>
		/// Get the highest value for this user for the key provided.
		/// </summary>
		public void GetHighest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Highest, success);
		}

		/// <summary>
		/// Get the lowest value for this user for the key provided.
		/// </summary>
		public void GetLowest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Lowest, success);
		}

		/// <summary>
		/// Get the cumulative value for this user for the key provided.
		/// </summary>
		public void GetCumulative(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Cumulative, success);
		}

		/// <summary>
		/// Get the count for this user for the key provided.
		/// </summary>
		public void GetCount(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Count, success);
		}

		/// <summary>
		/// Get the earliest value for this user for the key provided.
		/// </summary>
		public void GetEarliest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Earliest, success);
		}

		/// <summary>
		/// Get the latest value for this user for the key provided.
		/// </summary>
		public void GetLatest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> success)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Latest, success);
		}

		private void GetByLeaderboardType(string key, EvaluationDataType dataType, LeaderboardType type, Action<EvaluationDataResponse> success)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GameData.GetByLeaderboardTypeAsync(SUGARManager.CurrentUser.Id, SUGARManager.GameId, key, dataType, type,
				success,
				exception =>
				{
					Debug.LogError(exception.Message);
					success(null);
				});
			}
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
