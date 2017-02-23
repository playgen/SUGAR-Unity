using System.Linq;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

public class AccountInterface : BaseAccountInterface
{
	protected override void Awake()
	{
		base.Awake();
	}

	private void OnEnable()
	{
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BestFit.ResolutionChange -= DoBestFit;
	}

	protected override void PostShow()
	{
		
	}

	protected override void PostHide()
	{
		
	}

	private void DoBestFit()
	{
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).Where(t => t.activeSelf).BestFit();
	}
}
