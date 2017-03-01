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

	protected void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
	}

	private void DoBestFit()
	{
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).Where(t => t.activeSelf).BestFit();
	}
}
