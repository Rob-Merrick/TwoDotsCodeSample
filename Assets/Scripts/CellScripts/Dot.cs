using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dot : ConnectableCellContent
{
	[SerializeField] protected Image _dotImage;

	public override bool TerminatesFalling => true;

	public override CellContent CreateWithRandomProperties()
	{
		InitializeDot(CellContentSpawnManager.Instance.ActiveDotColors.GetRandom());
		return this;
	}

	public void InitializeDot(ColorType colorType)
	{
		ColorType = colorType;
		_dotImage.color = CellContentSpawnManager.Instance.GetDotColor(colorType);
		name = $"{colorType}Dot";
	}

	public override IEnumerator ApplyConnectionCoroutine(ConnectableCellContent[] connectedCells, Action<ConnectableCellContent> successCallback, Action<ConnectableCellContent> failureCallback)
	{
		float timeToFade = 0.15F;
		float totalFadeTime = 0.0F;
		Color originalColor = new Color(_dotImage.color.r, _dotImage.color.g, _dotImage.color.b, _dotImage.color.a);

		while(totalFadeTime < timeToFade)
		{
			_dotImage.color = Color.Lerp(originalColor, Color.clear, totalFadeTime / timeToFade);
			totalFadeTime += Time.deltaTime;
			yield return null;
		}

		_dotImage.color = Color.clear;
		GridManager.Instance.QueueCellErase(Cell);
		successCallback.Invoke(this);
	}
}
