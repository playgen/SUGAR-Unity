using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Unity;

public class LeaderboardPositionInterface : BaseLeaderboardPositionInterface
{
	internal override void Enable()
	{
		base.Enable();
	}

	internal void SetText(LeaderboardStandingsResponse res)
	{
		base.Enable();
		_position.text = res.Ranking.ToString();
		_playerName.text = res.ActorName;
		_score.text = res.Value;
	}

	internal override void Disable()
	{
		base.Disable();
	}
}