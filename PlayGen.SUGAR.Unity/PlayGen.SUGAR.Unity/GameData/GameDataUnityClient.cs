using System;
using System.Collections.Generic;
using System.Globalization;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this to GET and POST data related to the game.
	/// </summary>
	public class GameDataUnityClient
	{
		/// <summary>
		/// Get GameData for the currently signed in user for this game.
		/// </summary>
		/// <param name="onComplete">Callback with a list of gathered EvaluationDataResponse results.</param>
		/// <param name="keys">**Optional** Keys to search and return values for. (default: null)</param>
		public void Get(Action<IEnumerable<EvaluationDataResponse>> onComplete, string[] keys = null)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.GameData.GetAsync(SUGARManager.CurrentUser.Id, SUGARManager.GameId, keys,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
		}

		/// <summary>
		/// Get the data related to the highest value recorded for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetHighest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Highest, onComplete);
		}

		/// <summary>
		/// Get the data related to the lowest value recorded for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetLowest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Lowest, onComplete);
		}

		/// <summary>
		/// Get the cumulative value for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <remarks>
		/// - EvaluationDataType should be a type that can be added together, eg. Long
		/// </remarks>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetCumulative(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Cumulative, onComplete);
		}

		/// <summary>
		/// Get the count of recorded values for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetCount(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Count, onComplete);
		}

		/// <summary>
		/// Get the earliest recorded data for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetEarliest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Earliest, onComplete);
		}

		/// <summary>
		/// Get the latest recorded data for the currently signed in user for the key and dataType provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="dataType">EvaluationDataType of the GameData.</param>
		/// <param name="onComplete">Callback which contains the gathered result.</param>
		public void GetLatest(string key, EvaluationDataType dataType, Action<EvaluationDataResponse> onComplete)
		{
			GetByLeaderboardType(key, dataType, LeaderboardType.Latest, onComplete);
		}

		private void GetByLeaderboardType(string key, EvaluationDataType dataType, LeaderboardType type, Action<EvaluationDataResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.GameData.GetByLeaderboardTypeAsync(SUGARManager.CurrentUser.Id, SUGARManager.GameId, key, dataType, type,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
		}

		/// <summary>
		/// Record GameData with EvaluationDataType String with the key and value provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="value">The String value that'll be recorded.</param>
		/// <param name="onComplete">**Optional** Callback returns whther the data was sent successfully (default: null)</param>
		public void Send(string key, string value, Action<bool> onComplete = null)
		{
			Send(key, value, EvaluationDataType.String, onComplete);
		}

		/// <summary>
		/// Record GameData with EvaluationDataType Long with the key and value provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="value">The Long value that'll be recorded.</param>
		/// <param name="onComplete">**Optional** Callback returns whther the data was sent successfully (default: null)</param>
		public void Send(string key, long value, Action<bool> onComplete = null)
		{
			Send(key, value.ToString(), EvaluationDataType.Long, onComplete);
		}

		/// <summary>
		/// Record GameData with EvaluationDataType Float with the key and value provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="value">The Float value that'll be recorded.</param>
		/// <param name="onComplete">**Optional** Callback returns whther the data was sent successfully (default: null)</param>
		public void Send(string key, float value, Action<bool> onComplete = null)
		{
			Send(key, value.ToString(CultureInfo.InvariantCulture), EvaluationDataType.Float, onComplete);
		}

		/// <summary>
		/// Record GameData with EvaluationDataType Bool with the key and value provided.
		/// </summary>
		/// <param name="key">Name of the GameData key.</param>
		/// <param name="value">The Bool value that'll be recorded.</param>
		/// <param name="onComplete">**Optional** Callback returns whther the data was sent successfully (default: null)</param>
		public void Send(string key, bool value, Action<bool> onComplete = null)
		{
			Send(key, value.ToString(), EvaluationDataType.Boolean, onComplete);
		}

		private void Send(string key, string value, EvaluationDataType dataType, Action<bool> onComplete = null)
		{
			if (SUGARManager.UserSignedIn)
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
					onComplete?.Invoke(true);
				},
				exception =>
				{
					Debug.LogError($"GameData Sending Success: False. Exception: {exception}");
					onComplete?.Invoke(false);
				});
			}
		}
	}
}
