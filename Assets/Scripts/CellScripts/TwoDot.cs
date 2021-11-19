using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoDot : Dot
{
	[SerializeField] private Image _nextColorImage;
	[SerializeField] private Animator _animator;

	private bool _isFirstColorTypeActive = true;
	private ColorType _firstColorType;
	private ColorType _secondColorType;

	public bool IsChangingColors { get; private set; }

	public override CellContent CreateWithRandomProperties()
	{
		List<ColorType> activeColors = new List<ColorType>(CellContentSpawnManager.Instance.ActiveDotColors);
		InitializeTwoDot(activeColors.RemoveRandom(), activeColors.GetRandom());
		return this;
	}

	public void InitializeTwoDot(ColorType firstColorType, ColorType secondColorType)
	{
		if(firstColorType == secondColorType)
		{
			throw new System.Exception("Cannot create a two dot with the same color types");
		}

		InitializeDot(firstColorType);
		_animator.enabled = false;
		_firstColorType = firstColorType;
		_secondColorType = secondColorType;
		name = $"{firstColorType}_{secondColorType}_TwoDot";
		SetColors();
	}

	public override void OnSpecialPowerPostUserTurn()
	{
		IsChangingColors = true;
		_animator.enabled = true;
		_animator.SetTrigger("Snap");
		AudioManager.Instance.PlaySound("TwoDotLevitate");
		AnimationManager.Instance.QueueAnimation(() => !IsChangingColors);
	}

	public void AnimationOnColorSnap()
	{
		_isFirstColorTypeActive = !_isFirstColorTypeActive;
		AudioManager.Instance.PlaySound("TwoDotSwitchColors");
		SetColors();
	}

	public void AnimationOnEnd()
	{
		_animator.enabled = false;
		IsChangingColors = false;
	}

	private void SetColors()
	{
		ColorType = _isFirstColorTypeActive ? _firstColorType : _secondColorType;
		_dotImage.color = CellContentSpawnManager.Instance.GetDotColor(ColorType);
		_nextColorImage.color = CellContentSpawnManager.Instance.GetDotColor(_isFirstColorTypeActive ? _secondColorType : _firstColorType);
	}
}
