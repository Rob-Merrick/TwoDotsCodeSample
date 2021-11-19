using UnityEngine;
using UnityEngine.UI;

public class ShuffleButton : MonoBehaviour
{
    private Button _button;

	private void Start()
	{
		_button = GetComponent<Button>();
	}

	private void Update()
    {
		_button.interactable = !ShuffleManager.Instance.IsShuffling;
    }
}
