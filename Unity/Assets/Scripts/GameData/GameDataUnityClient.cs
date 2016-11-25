using UnityEngine;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	public class GameDataUnityClient : MonoBehaviour
	{
		private GameDataClient _gameDataClient;

		void Start()
		{
			_gameDataClient = SUGAR.Client.GameData;
		}

		public void SendGameData(string key, string value, GameDataType dataType)
		{
			GameDataRequest data = new GameDataRequest
			{
				ActorId = SUGAR.CurrentUser.Id,
				GameId = SUGAR.GameId,
				Key = key,
				Value = value,
				GameDataType = dataType
			};
			_gameDataClient.Add(data);
		}
	}
}
