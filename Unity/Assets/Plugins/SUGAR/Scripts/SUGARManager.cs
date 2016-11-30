using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	public static class SUGARManager
	{
		internal static SUGARClient Client { get; set; }

		public static int GameId { get; internal set; }

		public static ActorResponse CurrentUser { get; internal set; }

		public static AccountUnityClient Account { get; internal set; }

		public static AchievementUnityClient Achievement { get; internal set; }

		public static GameDataUnityClient GameData { get; internal set; }

		public static LeaderboardListUnityClient GameLeaderboard { get; internal set; }

		public static LeaderboardUnityClient Leaderboard { get; internal set; }

		internal static bool Register(SUGARUnityManager unityManager)
		{
			return Client == null;
		}
	}
}
