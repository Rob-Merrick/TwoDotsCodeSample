using UnityEngine;

public class JitterUI : MonoBehaviour
{
	[SerializeField] private Vector2 _anchorOffsetMin = -0.001F * Vector2.one;
	[SerializeField] private Vector2 _anchorOffsetMax = 0.001F * Vector2.one;
	[SerializeField] [Range(0.0F, 1.0F)] private float _secondsBetweenJitter = 0.0F;

	private RectTransform _rectTransform;
	private Vector2 _originalAnchorMin;
	private Vector2 _originalAnchorMax;
	private float _timeSinceLastJitter = 0.0F;

	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
		_originalAnchorMin = _rectTransform.anchorMin;
		_originalAnchorMax = _rectTransform.anchorMax;
	}

	private void Update()
	{
		if(_timeSinceLastJitter >= _secondsBetweenJitter)
		{
			_rectTransform.anchorMin = _originalAnchorMin + Vector2.Lerp(_anchorOffsetMin, _anchorOffsetMax, Random.value);
			_rectTransform.anchorMax = _originalAnchorMax + Vector2.Lerp(_anchorOffsetMin, _anchorOffsetMax, Random.value);
			_timeSinceLastJitter = 0.0F;
		}
		else
		{
			_timeSinceLastJitter += Time.deltaTime;
		}
	}
}
