using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class DotActivationBorder : MonoBehaviour
{
    [SerializeField] private Image _activationImage;
    [SerializeField] private Image _regularImage;
    [SerializeField] [Range(0.0F, 1.0F)] private float _fillPercent;
    [SerializeField] private Color _activationColor;
    [SerializeField] private Color _regularColor;

    public float FillPercent { get => _fillPercent; set => _fillPercent = Mathf.Clamp01(value); }
    public Color ActivationColor { get => _activationColor; set => _activationColor = value; }
    public Color RegularColor { get => _regularColor; set => _regularColor = value; }

    private void Update()
    {
        _activationImage.fillAmount = _fillPercent;
        _regularImage.fillAmount = 1.0F - _fillPercent;
        _activationImage.color = _activationColor;
        _regularImage.color = _regularColor;
    }
}
