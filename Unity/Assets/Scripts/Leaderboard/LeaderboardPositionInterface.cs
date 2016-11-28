using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class LeaderboardPositionInterface : MonoBehaviour
	{
		public Text Position;

		public Text PlayerName;

		public Text Score;

		public void SetText(LeaderboardStandingsResponse res)
		{
			gameObject.SetActive(true);
			Position.text = res.Ranking.ToString();
			PlayerName.text = res.ActorName;
			Score.text = res.Value;
		}

		public void Disbale()
		{
			gameObject.SetActive(false);
		}
	}
}
