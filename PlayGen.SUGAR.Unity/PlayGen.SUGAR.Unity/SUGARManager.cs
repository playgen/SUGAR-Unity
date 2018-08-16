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

		/// <value>
		/// GameId for this application.
		/// </value>
		public static int GameId { get; internal set; }

		/// <value>
		/// Currently signed in user.
		/// </value>
		public static ActorResponse CurrentUser { get; private set; }

		/// <value>
		/// Is there a user currently signed in.
		/// </value>
		public static bool UserSignedIn => CurrentUser != null;

		/// <value>
		/// Currently signed in user's primary group.
		/// </value>
		public static ActorResponse CurrentGroup { get; private set; }

		/// <value>
		/// Group name gathered from auto sign in.
		/// </value>
		public static string ClassId { get; private set; }

	    /// <summary>
	    /// Unity client for calls related to accounts
	    /// </summary>
	    public static AccountUnityClient Account => account;

		/// <value>
		/// Unity client for calls related to groups and users
		/// </value>
		public static ActorUnityClient Actor => actor;

		/// <value>
		/// Unity client for calls related to evaluations
		/// </value>
		public static EvaluationUnityClient Evaluation => evaluation;

		/// <value>
		/// Unity client for calls related to friend lists
		/// </value>
		public static UserFriendUnityClient UserFriend => userFriend;

		/// <value>
		/// Unity client for calls related to gamedata
		/// </value>
		public static GameDataUnityClient GameData => gameData;

		/// <value>
		/// Unity client for calls related to group members
		/// </value>
		public static GroupMemberUnityClient GroupMember => groupMember;

		/// <value>
		/// Unity client for calls related to leaderboard lists
		/// </value>
		public static LeaderboardListUnityClient GameLeaderboard => gameLeaderboard;

		/// <value>
		/// Unity client for calls related to leaderboard standings
		/// </value>
		public static LeaderboardUnityClient Leaderboard => leaderboard;

		/// <value>
		/// Unity client for calls related to resources
		/// </value>
		public static ResourceUnityClient Resource => resource;

		/// <value>
		/// Unity client for calls related to user groups
		/// </value>
		public static UserGroupUnityClient UserGroup => userGroup;

		/// <value>
		/// Class for managing Unity elements of the asset
		/// </value>
		public static SUGARUnityManager Unity => unity;

		/// <value>
		/// Class for contacting SUGAR client functionality
		/// </value>
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
