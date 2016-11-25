using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	public class GameDataUnityClient
	{
		public void SendGameData(string key, string value, GameDataType dataType)
		{
			GameDataRequest data = new GameDataRequest
			{
				ActorId = SUGARManager.CurrentUser.Id,
				GameId = SUGARManager.GameId,
				Key = key,
				Value = value,
				GameDataType = dataType
			};
			SUGARManager.Client.GameData.Add(data);
		}
	}
}
