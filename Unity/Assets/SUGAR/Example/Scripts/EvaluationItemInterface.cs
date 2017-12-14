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
	/// Text for showing evaluation image. In this case, a tick showing if the evaluation has been completed or not.
	/// </summary>
	[Tooltip("Text for showing evaluation image. In this case, a tick showing if the evaluation has been completed or not")]
	[SerializeField]
	private Image _evaluationImage;

	/// <summary>
	/// Enable the GameObject, set the text and enable/disable the image.
	/// </summary>
	internal void SetText(EvaluationProgressResponse evaluation, bool completed)
	{
		gameObject.SetActive(true);
		_evaluationName.text = evaluation.Name;
		if (_evaluationDescription)
		{
			_evaluationDescription.text = evaluation.Description;
		}
		_evaluationImage.enabled = completed;
	}

	/// <summary>
	/// Disable the GameObject if it isn't being used.
	/// </summary>
	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}