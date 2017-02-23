using PlayGen.SUGAR.Unity;

public class AchievementItemInterface : BaseAchievementItemInterface
{
	internal override void Enable()
	{
		base.Enable();
	}

	internal void SetText(string achieveName, bool completed)
	{
		base.Enable();
		_achieveName.text = achieveName;
		_achieveImage.enabled = completed;
	}

	internal override void Disable()
	{
		base.Disable();
	}
}