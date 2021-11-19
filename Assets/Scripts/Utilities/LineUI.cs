using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class LineUI : MonoBehaviour
{
	[SerializeField] private float _thickness = 10; 
	[SerializeField] private Vector2 _startPoint = Vector2.zero;
	[SerializeField] private Vector2 _endPoint = Vector2.right;

	private RectTransform _rectTransform;
	private Image _image;

	public float Thickness { get => _thickness; set => _thickness = value; }
	public Vector2 StartPoint { get => _startPoint; set => _startPoint = value; }
	public Vector2 EndPoint { get => _endPoint; set => _endPoint = value; }
	public Color Color { get => _image.color; set => _image.color = value; }

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_image = GetComponent<Image>();
	}

	private void Update()
	{
		_thickness = Mathf.Max(0.0F, _thickness);
		RedrawLine();
	}

	private void RedrawLine()
	{
		float scaleFactor = 1.0F;
		CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

		if(canvasScaler != null && canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
		{
			float matchWidthOrheight = canvasScaler.matchWidthOrHeight;
			scaleFactor = Mathf.Pow(Screen.width / canvasScaler.referenceResolution.x, 1.0F - matchWidthOrheight) * Mathf.Pow(Screen.height / canvasScaler.referenceResolution.y, matchWidthOrheight);
		}

		Vector2 scaledStartPoint = _startPoint / scaleFactor;
		Vector2 scaledEndPoint = _endPoint / scaleFactor;
		Vector2 lineVector = scaledEndPoint - scaledStartPoint;
		float angle = Vector2.SignedAngle(Vector2.right, lineVector);
		float width = lineVector.magnitude;
		_rectTransform.anchoredPosition = scaledStartPoint;
		_rectTransform.sizeDelta = new Vector2(width, _thickness);
		_rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
