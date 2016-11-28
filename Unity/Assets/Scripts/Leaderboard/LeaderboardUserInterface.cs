using System;
using System.Collections.Generic;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class LeaderboardUserInterface : MonoBehaviour
	{
		[SerializeField]
		private Text _leaderboardName;
		[SerializeField]
		private Text _leaderboardType;
		[SerializeField]
		private LeaderboardPositionInterface _leaderboardHeader;
		[SerializeField]
		private LeaderboardPositionInterface[] _leaderboardPositions;
		[SerializeField]
		private Button _previousButton;
		[SerializeField]
		private Button _nextButton;
		[SerializeField]
		private Button _topButton;
		[SerializeField]
		private Button _nearButton;
		[SerializeField]
		private Button _friendsButton;
		[SerializeField]
		private Button _closeButton;

		public void Awake()
		{
			//set button on click to fire SUGARManager.Leaderboard. methods where needed
		}

		public void ShowLeaderboard(string token, IEnumerable<LeaderboardStandingsResponse> standings)
		{
			//set-up leaderboard standing display
		}
	}
}
