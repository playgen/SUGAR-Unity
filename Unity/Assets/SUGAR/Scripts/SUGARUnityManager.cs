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
		[SerializeField] private string _baseAddress;

		[SerializeField] private int _gameId;

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
			SUGARManager.Achievement = GetComponent<AchievementUnityClient>();
			SUGARManager.Leaderboard = GetComponent<LeaderboardUnityClient>();
			SUGARManager.GameLeaderboard = GetComponent<LeaderboardListUnityClient>();
		}
	}
}