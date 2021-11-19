using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOverlayManager : Manager<ScreenOverlayManager>
{
	[SerializeField] private Image _screenOverlayObject;

	public bool IsFading { get; private set; } = false;
	public Color OverlayColor { get; set; } = Color.clear;

	private void Start()
	{
		FadeFromColor(Color.white);
		this.DoAfter(() => !IsFading, () => DisableOverlay());
	}

	private void Update()
	{
		if(!IsFading && _screenOverlayObject.gameObject.activeSelf)
		{
			_screenOverlayObject.color = OverlayColor;
		}
	}

	public void EnableOverlay()
	{
		_screenOverlayObject.gameObject.SetActive(true);
	}

	public void DisableOverlay()
	{
		_screenOverlayObject.gameObject.SetActive(false);
	}

	public void FadeToColor(Color color, float timeToFade = 1.0F, Action callback = null)
	{
		StartCoroutine(Fade(originalColor: OverlayColor, color, timeToFade, callback));
	}

	public void FadeFromColor(Color color, float timeToFade = 1.0F, Action callback = null)
	{
		StartCoroutine(Fade(originalColor: color, OverlayColor, timeToFade, callback));
	}

	private IEnumerator Fade(Color originalColor, Color targetColor, float timeToFade, Action callback)
	{
		IsFading = true;
		float totalTimeTaken = 0.0F;
		_screenOverlayObject.color = originalColor;
		EnableOverlay();

		while(totalTimeTaken < timeToFade)
		{
			_screenOverlayObject.color = Color.Lerp(originalColor, targetColor, totalTimeTaken / timeToFade);
			yield return null;
			totalTimeTaken += Time.deltaTime;
		}

		_screenOverlayObject.color = targetColor;
		IsFading = false;
		callback?.Invoke();
	}
}
