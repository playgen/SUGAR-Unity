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

		internal static ActorUnityClient actor = new ActorUnityClient();

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
		/// Currently signed in user.
		/// </summary>
		public static ActorResponse CurrentUser { get; private set; }

		/// <summary>
		/// Is there a user currently signed in.
		/// </summary>
		public static bool UserSignedIn => CurrentUser != null;

		/// <summary>
		/// Currently signed in user's primary group.
		/// </summary>
		public static ActorResponse CurrentGroup { get; private set; }

		/// <summary>
		/// Group name gathered from auto sign in.
		/// </summary>
		public static string ClassId { get; private set; }

	    /// <summary>
	    /// Unity client for calls related to accounts
	    /// </summary>
	    public static AccountUnityClient Account => account;

		/// <summary>
		/// Unity client for calls related to groups and users
		/// </summary>
		public static ActorUnityClient Actor => actor;

		/// <summary>
		/// Unity client for calls related to evaluations
		/// </summary>
		public static EvaluationUnityClient Evaluation => evaluation;

		/// <summary>
		/// Unity client for calls related to friend lists
		/// </summary>
		public static UserFriendUnityClient UserFriend => userFriend;

		/// <summary>
		/// Unity client for calls related to gamedata
		/// </summary>
		public static GameDataUnityClient GameData => gameData;

		/// <summary>
		/// Unity client for calls related to group members
		/// </summary>
		public static GroupMemberUnityClient GroupMember => groupMember;

		/// <summary>
		/// Unity client for calls related to leaderboard lists
		/// </summary>
		public static LeaderboardListUnityClient GameLeaderboard => gameLeaderboard;

		/// <summary>
		/// Unity client for calls related to leaderboard standings
		/// </summary>
		public static LeaderboardUnityClient Leaderboard => leaderboard;

		/// <summary>
		/// Unity client for calls related to resources
		/// </summary>
		public static ResourceUnityClient Resource => resource;

		/// <summary>
		/// Unity client for calls related to user groups
		/// </summary>
		public static UserGroupUnityClient UserGroup => userGroup;

		/// <summary>
		/// Class for managing Unity elements of the asset
		/// </summary>
		public static SUGARUnityManager Unity => unity;

		/// <summary>
		/// Class for contacting SUGAR client functionality
		/// </summary>
		public static SUGARClient Client => client;

		internal static bool Register(SUGARUnityManager unityManager)
		{
			return client == null;
		}

		internal static void SetCurrentUser(ActorResponse user)
		{
			CurrentUser = user;
		}

		/// <summary>
		/// Set the 'primary' group for the currently signed in user
		/// </summary>
		public static void SetCurrentGroup(ActorResponse group)
		{
			CurrentGroup = group;
		}

		/// <summary>
		/// Set the ClassId for the currently signed in user
		/// </summary>
		public static void SetClassId(string classid)
		{
			ClassId = classid;
		}
	}
}
