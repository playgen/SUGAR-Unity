using System;

using UnityEngine;
using System.Collections.Generic;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

namespace SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : MonoBehaviour
	{
		private int _pageNumber;

		private ActorType _actorType;

		private readonly List<List<LeaderboardResponse>> _leaderboards = new List<List<LeaderboardResponse>>();

		[SerializeField]
		private LeaderboardListUserInterface _leaderboardListInterface;

		public void DisplayList(ActorType filter = ActorType.User)
		{
			GetList(filter);
		}

		private void GetList(ActorType filter)
		{
			_pageNumber = 0;
			_actorType = filter;
			GetLeaderboards();
			_leaderboardListInterface.ShowLeaderboards(_actorType, _leaderboards[(int)_actorType], _pageNumber);
		}

		internal void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			_leaderboardListInterface.ShowLeaderboards(_actorType, _leaderboards[(int)_actorType], _pageNumber);
		}

		internal void UpdateFilter(int filter)
		{
			_pageNumber = 0;
			_actorType = (ActorType)filter;
			_leaderboardListInterface.ShowLeaderboards(_actorType, _leaderboards[(int)_actorType], _pageNumber);
		}

		private void GetLeaderboards()
		{
			_leaderboards.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				var leaderboards = SUGARManager.Client.Leaderboard.Get(SUGARManager.GameId).ToList();
				foreach (var at in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					_leaderboards.Add(leaderboards.Where(lb => lb.ActorType == at).ToList());
				}
			}
			else
			{
				for (int i = 0; i < Enum.GetValues(typeof(ActorType)).Length; i++)
				{
					_leaderboards.Add(new List<LeaderboardResponse>());
				}
			}
		}
	}
}
