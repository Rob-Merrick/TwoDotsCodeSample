using System.Collections;
using UnityEngine;

public class TutorialTwoDot : MonoBehaviour
{
	[SerializeField] private TwoDot _twoDotPrefab;

	private TwoDot _twoDot;
	private bool _isAnimating = false;

	private void Update()
	{
		if(_twoDot != null && !TutorialManager.Instance.IsGivingTutorial)
		{
			StartCoroutine(Animate(startScale: Vector3.one, endScale: Vector3.zero));

			this.DoAfter(() => !_isAnimating, () =>
			{
				Destroy(_twoDot.gameObject);
				gameObject.SetActive(false);
				TutorialManager.Instance.ShowOffContainer.SetActive(false);
			});
		}
	}

	public void ShowOffTwoDot()
	{
		_twoDot = Instantiate(_twoDotPrefab);
		_twoDot.InitializeTwoDot(ColorType.Turquoise, ColorType.Purple);
		_twoDot.transform.SetParent(TutorialManager.Instance.ShowOffContainer.transform, worldPositionStays: false);
		TutorialManager.Instance.ShowOffContainer.SetActive(true);
		StartCoroutine(Animate(startScale: Vector3.zero, endScale: Vector3.one));

		this.DoAfter(() => !_isAnimating, () =>
		{
			TutorialManager.Instance.AdvanceTutorial();
		});
	}

	public void AnimateTwoDot()
	{
		_twoDot.OnSpecialPowerPostUserTurn();

		this.DoAfter(() => !_twoDot.IsChangingColors, () =>
		{
			this.DoAfter(seconds: 1.0F, () => TutorialManager.Instance.AdvanceTutorial());
		});
	}

	private IEnumerator Animate(Vector3 startScale, Vector3 endScale)
	{
		_isAnimating = true;
		float totalTimeTaken = 0.0F;
		float totalTimeToTake = 0.2F;

		while(totalTimeTaken < totalTimeToTake)
		{
			_twoDot.gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, totalTimeTaken / totalTimeToTake);
			yield return null;
			totalTimeTaken += Time.deltaTime;
		}

		_twoDot.gameObject.transform.localScale = Vector3.one;
		_isAnimating = false;
	}
}
