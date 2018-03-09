using System;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;

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

		internal static EvaluationUnityClient evaluation { get; set; }

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
		/// Currently signed in user. WARNING: Only set this value if completely necessary.
		/// </summary>
		public static ActorResponse CurrentUser { get; set; }

		/// <summary>
		/// Currently signed in user's group. WARNING: Only set this value if completely necessary.
		/// </summary>
		public static ActorResponse CurrentGroup { get; set; }

		/// <summary>
		/// Group name gathered from auto sign in. WARNING: Only set this value if completely necessary.
		/// </summary>
		public static string ClassId { get; set; }

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
		/// Unity client for calls related to evaluations
		/// </summary>
		public static EvaluationUnityClient Evaluation
		{
			get
			{
				if (evaluation != null)
				{
					return evaluation;
				}
				throw new Exception("SUGAR GameObject needs to be active to access Evaluations");
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
				throw new Exception("SUGAR GameObject needs to be active to access Friends");
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
				throw new Exception("SUGAR GameObject needs to be active to access Groups");
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
				throw new Exception("SUGAR GameObject needs to be active to access Leaderboards");
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
				throw new Exception("SUGAR GameObject needs to be active to access Leaderboards");
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
				throw new Exception("SUGAR GameObject needs to be active to access Resources");
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
				throw new Exception("SUGAR GameObject needs to be active to access Groups");
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

		/// <summary>
		/// Class for contacting SUGAR client functionality
		/// </summary>
		public static SUGARClient Client
		{
			get
			{
				if (client != null)
				{
					return client;
				}
				throw new Exception("No SUGARClient found.");
			}
		}

		internal static bool Register(SUGARUnityManager unityManager)
		{
			return client == null;
		}
	}
}
