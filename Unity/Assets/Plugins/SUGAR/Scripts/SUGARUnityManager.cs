using UnityEngine;
using PlayGen.SUGAR.Client;

namespace SUGAR.Unity
{
	[RequireComponent(typeof(AccountUnityClient))]
	[RequireComponent(typeof(AchievementUnityClient))]
	[RequireComponent(typeof(LeaderboardUnityClient))]
	[RequireComponent(typeof(LeaderboardListUnityClient))]
	public class SUGARUnityManager : MonoBehaviour
	{
		[SerializeField]
		private string _baseAddress;
		[SerializeField]
		private int _gameId;
		[SerializeField]
		private bool _useAchievements = true;
		[SerializeField]
		private bool _useLeaderboards = true;

		void Awake()
		{
			if (SUGARManager.Register(this))
			{
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
			SUGARManager.Client = new SUGARClient(_baseAddress); // hTTPhANDLER ?>?!
			SUGARManager.GameId = _gameId;
			SUGARManager.Account = GetComponent<AccountUnityClient>();
			SUGARManager.Achievement = _useAchievements ? GetComponent<AchievementUnityClient>() : null;
			SUGARManager.Leaderboard = _useLeaderboards ? GetComponent<LeaderboardUnityClient>() : null;
			SUGARManager.GameLeaderboard = _useLeaderboards ? GetComponent<LeaderboardListUnityClient>() : null;
		}
	}
}