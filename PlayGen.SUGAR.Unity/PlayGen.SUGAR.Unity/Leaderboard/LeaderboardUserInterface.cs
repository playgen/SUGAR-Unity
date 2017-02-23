using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using PlayGen.Unity.Utilities.BestFit;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public class LeaderboardUserInterface : MonoBehaviour
	{
		[SerializeField]
		private Text _leaderboardName;
		[SerializeField]
		private Text _leaderboardType;
		[SerializeField]
		private LeaderboardPositionInterface[] _leaderboardPositions;
		[SerializeField]
		private Button _previousButton;
		[SerializeField]
		private Button _nextButton;
		[SerializeField]
		private Text _pageNumberText;
		private int _pageNumber;
		private LeaderboardFilterType _filter;
		[SerializeField]
		private Button _topButton;
		[SerializeField]
		private Button _nearButton;
		[SerializeField]
		private Button _friendsButton;
		[SerializeField]
		private Text _errorText;
		[SerializeField]
		private Button _closeButton;
		[SerializeField]
		private Button _signinButton;

		private void Awake()
		{
			_previousButton.onClick.AddListener(delegate { UpdatePageNumber(-1); });
			_nextButton.onClick.AddListener(delegate { UpdatePageNumber(1); });
			_topButton.onClick.AddListener(delegate { UpdateFilter(0); });
			_nearButton.onClick.AddListener(delegate { UpdateFilter(1); });
			_friendsButton.onClick.AddListener(delegate { UpdateFilter(2); });
			_closeButton.onClick.AddListener(delegate { SUGARManager.Unity.DisableObject(gameObject); });
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		private void OnEnable()
		{
			DoBestFit();
			BestFit.ResolutionChange += DoBestFit;
			Localization.LanguageChange += OnLanguageChange;
		}

		private void OnDisable()
		{
			BestFit.ResolutionChange -= DoBestFit;
			Localization.LanguageChange -= OnLanguageChange;
		}

		internal void Display(LeaderboardFilterType filter, IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess = true)
		{
			_pageNumber = 0;
			_filter = filter;
			ShowLeaderboard(standings, loadingSuccess);
		}

		private void ShowLeaderboard(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess)
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.Unity.EnableObject(gameObject);
			_errorText.text = string.Empty;
			if (_signinButton)
			{
				_signinButton.gameObject.SetActive(false);
			}
			_topButton.interactable = true;
			_nearButton.interactable = true;
			_friendsButton.interactable = true;
			var standingsList = standings.ToList();
			if (!standingsList.Any() && _pageNumber > 0)
			{
				UpdatePageNumber(-1);
				return;
			}
			if (_pageNumber < 0)
			{
				UpdatePageNumber(1);
				return;
			}
			for (int i = 0; i < _leaderboardPositions.Length; i++)
			{
				if (i >= standingsList.Count)
				{
					_leaderboardPositions[i].Disable();
				} else
				{
					_leaderboardPositions[i].SetText(standingsList[i]);
				}
			}
			_leaderboardName.text = SUGARManager.Leaderboard.CurrentLeaderboard != null ? SUGARManager.Leaderboard.CurrentLeaderboard.Name : string.Empty;
			_leaderboardType.text = Localization.Get(_filter.ToString());
			_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
			_previousButton.interactable = _pageNumber > 0;
			_nextButton.interactable = SUGARManager.Leaderboard.NextPage;
			if (SUGARManager.Leaderboard.CurrentLeaderboard == null)
			{
				loadingSuccess = false;
			}
			else
			{
				_nearButton.interactable = SUGARManager.CurrentUser != null && SUGARManager.Leaderboard.CurrentLeaderboard.ActorType == ActorType.User;
				_friendsButton.interactable = SUGARManager.CurrentUser != null && SUGARManager.Leaderboard.CurrentLeaderboard.ActorType == ActorType.User;
			}
			if (!loadingSuccess)
			{
				if (SUGARManager.CurrentUser == null)
				{
					_errorText.text = Localization.Get("NO_USER_ERROR");
					if (SUGARManager.Account.HasInterface && _signinButton)
					{
						_signinButton.gameObject.SetActive(true);
					}
				}
				else
				{
					_errorText.text = Localization.Get("LEADERBOARD_LOAD_ERROR");
				}
				_topButton.interactable = false;
				_nearButton.interactable = false;
				_friendsButton.interactable = false;
			}
			else if (standingsList.Count == 0)
			{
				_errorText.text = Localization.Get("NO_LEADERBOARD_ERROR");
			}
			_leaderboardPositions.Select(t => t.gameObject).BestFit();
		}

		private void AttemptSignIn()
		{
			SUGARManager.Account.DisplayPanel(success =>
			{
				if (success)
				{
					UpdatePageNumber(0);
				}
			});
		}

		private void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			SUGARManager.Leaderboard.GetLeaderboardStandings(_filter, _pageNumber, result =>
			{
				var standings = result.ToList();
				ShowLeaderboard(standings, true);
			});
		}

		private void UpdateFilter(int filter)
		{
			_pageNumber = 0;
			_filter = (LeaderboardFilterType)filter;
			SUGARManager.Leaderboard.GetLeaderboardStandings(_filter, _pageNumber, result =>
			{
				var standings = result.ToList();
				ShowLeaderboard(standings, true);
			});
		}

		internal int GetPossiblePositionCount()
		{
			return _leaderboardPositions.Length;
		}

		private void DoBestFit()
		{
			_leaderboardPositions.Select(t => t.gameObject).BestFit();
			GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
		}

		private void OnLanguageChange()
		{
			UpdatePageNumber(0);
		}
	}
}
