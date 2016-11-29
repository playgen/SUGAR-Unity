using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	public static class SUGARManager
	{
		internal static SUGARClient Client { get; set; }

		internal static int GameId { get; set; }

		internal static ActorResponse CurrentUser { get; set; }

		internal static AccountUnityClient Account { get; set; }

		private static GameDataUnityClient _gameData = new GameDataUnityClient();

		public static GameDataUnityClient GameData
		{
			get { return _gameData; }
		}

		internal static AchievementUnityClient Achievement { get; set; }

		internal static LeaderboardUnityClient Leaderboard { get; set; }

		internal static LeaderboardListUnityClient GameLeaderboard { get; set; }

		internal static bool Register(SUGARUnityManager unityManager)
		{
			return Client == null;
		}
	}
}
