using System.Linq;

using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.SUGAR.Common;

public class EvaluationListInterface : BaseEvaluationListInterface
{
	/// <summary>
	/// Text which describes the current information being displayed.
	/// </summary>
	[Tooltip("Text which describes the current information being displayed.")]
	[SerializeField]
	private Text _titleText;

	/// <summary>
	/// An array of the EvaluationItemInterfaces on this GameObject, set in the Inspector.
	/// </summary>
	[Tooltip("An array of the EvaluationItemInterfaces on this GameObject, set in the Inspector.")]
	[SerializeField]
	private EvaluationItemInterface[] _evaluationItems;

	/// <summary>
	/// Button used to go to the previous page of results.
	/// </summary>
	[Tooltip("Button used to go to the previous page of results.")]
	[SerializeField]
	private Button _previousButton;

	/// <summary>
	/// Button used to go to the next page of results.
	/// </summary>
	[Tooltip("Button used to go to the next page of results.")]
	[SerializeField]
	private Button _nextButton;

	/// <summary>
	/// Text which displays the current page.
	/// </summary>
	[Tooltip("Text which displays the current page.")]
	[SerializeField]
	private Text _pageNumberText;

	/// <summary>
	/// The current page number.
	/// </summary>
	private int _pageNumber;

	/// <summary>
	/// In addition to base onclick adding, adds listeners for the previous and next buttons.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		_previousButton.onClick.AddListener(() => UpdatePageNumber(-1));
		_nextButton.onClick.AddListener(() => UpdatePageNumber(1));
	}

	/// <summary>
	/// Trigger DoBestFit method and add event listeners for when resolution and language changes.
	/// </summary>
	private void OnEnable()
	{
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
		Localization.LanguageChange += OnLanguageChange;
	}

	/// <summary>
	/// Remove event listeners.
	/// </summary>
	private void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
		Localization.LanguageChange -= OnLanguageChange;
	}

	/// <summary>
	/// Set the pageNumber to 0 before displaying the UI.
	/// </summary>
	protected override void PreDisplay()
	{
		_pageNumber = 0;
	}

	/// <summary>
	/// Adjust EvaluationItemInterface pool to display a page of evaluations.
	/// </summary>
	protected override void Draw()
	{
		var evaluationType = SUGARManager.Evaluation.Progress.FirstOrDefault()?.Type;
		_titleText.text = evaluationType == null ? string.Empty : Localization.Get(evaluationType == EvaluationType.Achievement ? "ACHIEVEMENTS" : "SKILLS");
		var evaluationList = SUGARManager.Evaluation.Progress.Skip(_pageNumber * _evaluationItems.Length).Take(_evaluationItems.Length).ToList();
		if (!evaluationList.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if (_pageNumber < 0)
		{
			UpdatePageNumber(1);
			return;
		}
		for (var i = 0; i < _evaluationItems.Length; i++)
		{
			if (i >= evaluationList.Count)
			{
				_evaluationItems[i].Disable();
			}
			else
			{
				_evaluationItems[i].SetText(evaluationList[i], Mathf.Approximately(evaluationList[i].Progress, 1.0f));
			}
		}
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		_nextButton.interactable = SUGARManager.Evaluation.Progress.Count > (_pageNumber + 1) * _evaluationItems.Length;
		DoBestFit();
	}

	/// <summary>
	/// If a user signs in via this panel, refresh the current page (which should be page 1).
	/// </summary>
	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	/// <summary>
	/// Adjust the current page number and redraw the UI
	/// </summary>
	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		Show(true);
	}

	/// <summary>
	/// Set the text of all buttons and all evaluations to be as big as possible and the same size within the same grouping.
	/// </summary>
	private void DoBestFit()
	{
		_evaluationItems.Select(t => t.transform.Find("Name")).BestFit();
		_evaluationItems.Select(t => t.transform.Find("Description")).BestFit();
		_evaluationItems.Select(t => t.transform.Find("Progress")).BestFit();
		GetComponentsInChildren<Button>(true).ToList().BestFit();
	}

	/// <summary>
	/// Refresh the current page to ensure any text set in code is also translated.
	/// </summary>
	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}
