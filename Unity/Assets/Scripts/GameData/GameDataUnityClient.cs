using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace SUGAR.Unity
{
	public class GameDataUnityClient
	{
		public void SendGameData(string key, string value, SaveDataType dataType)
		{
			bool success = false;
			if (SUGARManager.CurrentUser != null)
			{
				SaveDataRequest data = new SaveDataRequest
				{
					ActorId = SUGARManager.CurrentUser.Id,
					GameId = SUGARManager.GameId,
					Key = key,
					Value = value,
					SaveDataType = dataType
				};
				success = SUGARManager.Client.GameData.Add(data) != null;
			}
			Debug.Log("GameData Sending Success: " + success);
		}
	}
}
