using System;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Unity
{
	public static class SUGARManager
	{
		internal static SUGARUnityManager unity { get; set; }

		internal static SUGARClient client { get; set; }

		internal static AccountUnityClient account { get; set; }

		internal static AchievementUnityClient achievement { get; set; }

		internal static FriendUnityClient friend { get; set; }

		internal static GameDataUnityClient gameData = new GameDataUnityClient();

		internal static LeaderboardListUnityClient gameLeaderboard { get; set; }

		internal static GroupUnityClient group { get; set; }

		internal static LeaderboardUnityClient leaderboard { get; set; }

		internal static ResourceUnityClient resource = new ResourceUnityClient();

		internal static Config config { get; set; }

		public static int GameId { get; internal set; }

		public static ActorResponse CurrentUser { get; internal set; }

		public static string GroupId { get; internal set; }

		public static AccountUnityClient Account
		{
			get
			{
				if (account != null)
				{
					return account;
				} 
				throw new Exception("SUGAR GameObject needs to be active to access Account");
			}
		}

		public static AchievementUnityClient Achievement
		{
			get
			{
				if (achievement != null)
				{
					return achievement;
				}
				throw new Exception("Achievements are currently disabled in the SUGAR Unity Manager");
			}
		}

		public static FriendUnityClient Friend
		{
			get
			{
				if (friend != null)
				{
					return friend;
				}
				throw new Exception("Friends are currently disabled in the SUGAR Unity Manager");
			}
		}

		public static GameDataUnityClient GameData
		{
			get
			{
				if (gameData != null)
				{
					return gameData;
				}
				throw new Exception("SUGAR GameObject needs to be active to access GameData");
			}
		}

		public static GroupUnityClient Group
		{
			get
			{
				if (group != null)
				{
					return group;
				}
				throw new Exception("Groups are currently disabled in the SUGAR Unity Manager");
			}
		}

		public static LeaderboardListUnityClient GameLeaderboard
		{
			get
			{
				if (gameLeaderboard != null)
				{
					return gameLeaderboard;
				}
				throw new Exception("Leaderboards are currently disabled in the SUGAR Unity Manager");
			}
		}

		public static LeaderboardUnityClient Leaderboard
		{
			get
			{
				if (leaderboard != null)
				{
					return leaderboard;
				}
				throw new Exception("Leaderboards are currently disabled in the SUGAR Unity Manager");
			}
		}

		public static ResourceUnityClient Resource
		{
			get
			{
				if (resource != null)
				{
					return resource;
				}
				throw new Exception("SUGAR GameObject needs to be active to access Resources");
			}
		}

		public static SUGARUnityManager Unity
		{
			get
			{
				if (unity != null)
				{
					return unity;
				}
				throw new Exception("No SUGARUnityManager found.");
			}
		}

		internal static bool Register(SUGARUnityManager unityManager)
		{
			return client == null;
		}
	}
}
