using UnityEngine;
using UnityEngine.UI;

public class AchievementItemInterface : MonoBehaviour
{
	[SerializeField]
	private Text _achieveName;
	[SerializeField]
	private Image _achieveImage;

	internal void SetText(string achieveName, bool completed)
	{
		gameObject.SetActive(true);
		_achieveName.text = achieveName;
		_achieveImage.enabled = completed;
	}

	internal void Disable()
	{
		gameObject.SetActive(false);
	}
}