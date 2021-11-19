using UnityEngine;

public class PopupWindow : MonoBehaviour
{
	[SerializeField] private bool _autoClose = false;
	[SerializeField] private bool _playDialogueSound = true;
	[SerializeField] private Animator _animator;

	public bool IsOpen { get; private set; } = false;

	public void Open()
	{
		transform.parent.gameObject.SetActive(true);
		_animator.enabled = true;
		_animator.Play("NoMoreMoves");
		IsOpen = true;
	}

	public void OnOpen()
	{
		if(_playDialogueSound)
		{
			AudioManager.Instance.PlaySound("DialogueOpen");
		}
	}

	public void OnWindowDisplayed()
	{
		if(!_autoClose)
		{
			_animator.enabled = false;
		}
	}

	public void Close()
	{
		_animator.enabled = true;
		_animator.Play("NoMoreMoves", 0, 190.0F / 210.0F);
	}

	public void OnWindowHidden()
	{
		_animator.enabled = false;
		transform.parent.gameObject.SetActive(false);
		IsOpen = false;
	}
}
