using PlayGen.SUGAR.Contracts;

using UnityEngine;
using UnityEngine.UI;

public class EvaluationItemInterface : MonoBehaviour
{
	/// <summary>
	/// Text for displaying evaluation name.
	/// </summary>
	[Tooltip("Text for displaying evaluation name")]
	[SerializeField]
	private Text _evaluationName;

	/// <summary>
	/// Text for displaying evaluation description.
	/// </summary>
	[Tooltip("Text for displaying evaluation description")]
	[SerializeField]
	private Text _evaluationDescription;

	/// <summary>
	/// Image related to the evaluation. In this case, a tick showing if the evaluation has been completed or not.
	/// </summary>
	[Tooltip("Image related to the evaluation. In this case, a tick showing if the evaluation has been completed or not")]
	[SerializeField]
	private Image _evaluationImage;

	/// <summary>
	/// Text for showing evaluation progress if not completed.
	/// </summary>
	[Tooltip("Text for showing evaluation progress if not completed.")]
	[SerializeField]
	private Text _evaluationProgress;

	/// <summary>
	/// Enable the GameObject, set the text and enable/disable the image.
	/// </summary>
	internal void SetText(EvaluationProgressResponse evaluation, bool completed)
	{
		gameObject.SetActive(true);
		_evaluationName.text = evaluation.Name;
		_evaluationDescription.text = evaluation.Description;
		_evaluationImage.enabled = completed;
		_evaluationProgress.text = completed ? string.Empty : (Mathf.Round(evaluation.Progress * 100)) + "%";
	}

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}