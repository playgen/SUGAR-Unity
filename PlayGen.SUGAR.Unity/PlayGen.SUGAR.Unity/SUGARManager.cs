using System;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Access point for SUGAR related classes.
	/// </summary>
	public static class SUGARManager
	{
		internal static SUGARUnityManager unity { get; set; }

		internal static SUGARClient client { get; set; }

		internal static AccountUnityClient account { get; set; }

		internal static AchievementUnityClient achievement { get; set; }

		internal static GroupMemberUnityClient groupMember { get; set; }

		internal static GameDataUnityClient gameData = new GameDataUnityClient();

		internal static LeaderboardListUnityClient gameLeaderboard { get; set; }

		internal static UserGroupUnityClient userGroup { get; set; }

		internal static LeaderboardUnityClient leaderboard { get; set; }

		internal static ResourceUnityClient resource { get; set; }

		internal static UserFriendUnityClient userFriend { get; set; }

		internal static Config config { get; set; }

		/// <summary>
		/// GameId for this application.
		/// </summary>
		public static int GameId { get; internal set; }

		/// <summary>
		/// Currently signed in user.
		/// </summary>
		public static ActorResponse CurrentUser { get; internal set; }

		/// <summary>
		/// Group name gathered from auto sign in.
		/// </summary>
		public static string GroupId { get; internal set; }

		/// <summary>
		/// Unity client for calls related to accounts
		/// </summary>
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

		/// <summary>
		/// Unity client for calls related to achievements
		/// </summary>
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

		/// <summary>
		/// Unity client for calls related to friend lists
		/// </summary>
		public static UserFriendUnityClient UserFriend
		{
			get
			{
				if (userFriend != null)
				{
					return userFriend;
				}
				throw new Exception("Friends are currently disabled in the SUGAR Unity Manager");
			}
		}

		/// <summary>
		/// Unity client for calls related to gamedata
		/// </summary>
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

		/// <summary>
		/// Unity client for calls related to group members
		/// </summary>
		public static GroupMemberUnityClient GroupMember
		{
			get
			{
				if (groupMember != null)
				{
					return groupMember;
				}
				throw new Exception("Groups are currently disabled in the SUGAR Unity Manager");
			}
		}

		/// <summary>
		/// Unity client for calls related to leaderboard lists
		/// </summary>
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

		/// <summary>
		/// Unity client for calls related to leaderboard standings
		/// </summary>
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

		/// <summary>
		/// Unity client for calls related to resources
		/// </summary>
		public static ResourceUnityClient Resource
		{
			get
			{
				if (resource != null)
				{
					return resource;
				}
				throw new Exception("Resources are currently disabled in the SUGAR Unity Manager");
			}
		}

		/// <summary>
		/// Unity client for calls related to user groups
		/// </summary>
		public static UserGroupUnityClient UserGroup
		{
			get
			{
				if (userGroup != null)
				{
					return userGroup;
				}
				throw new Exception("Groups are currently disabled in the SUGAR Unity Manager");
			}
		}

		/// <summary>
		/// Class for managing Unity elements of the asset
		/// </summary>
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
