using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Manager<TutorialManager>
{
	[SerializeField] private TutorialInfo[] _tutorialInfo;
	[SerializeField] private Image _emilyImage;
	[SerializeField] private PopupWindow _tutorialPopupWindow;
	[SerializeField] private Text _tutorialText;
	[SerializeField] private GameObject _tutorialShowOffContainer;

	private TutorialInfo[] _activeTutorialInfo;

	public int TutorialStep { get; private set; } = 0;
	public bool IsGivingTutorial { get; private set; } = false;
	public GameObject ShowOffContainer => _tutorialShowOffContainer;

	private void Start()
	{
		BuildActiveTutorialInfo();

		if(_activeTutorialInfo.Length > 0)
		{
			IsGivingTutorial = true;
			GridManager.Instance.gameObject.SetActive(false);
			ScreenOverlayManager.Instance.EnableOverlay();

			this.DoAfter(seconds: 2.0F, () =>
			{
				_emilyImage.gameObject.SetActive(true);
				_emilyImage.GetComponent<Animator>().Play("Emily");
				_tutorialText.text = _activeTutorialInfo[TutorialStep].TutorialMessage;
				_tutorialPopupWindow.Open();
				ScreenOverlayManager.Instance.DisableOverlay();
			});
		}
	}

	public void AdvanceTutorial()
	{
		bool isReadyToContinue = true;

		if(_activeTutorialInfo[TutorialStep].ClosePopupAfterMessageDisplayed)
		{
			_tutorialPopupWindow.Close();
			isReadyToContinue = false;
		}

		this.DoAfter(() => isReadyToContinue || !_tutorialPopupWindow.IsOpen, () =>
		{
			TutorialStep++;

			if(TutorialStep >= _activeTutorialInfo.Length)
			{
				EndTutorial();
			}
			else
			{
				TutorialInfo tutorialInfo = _activeTutorialInfo[TutorialStep];

				if(tutorialInfo.UseUnityEvent)
				{
					tutorialInfo.TutorialEvent.Invoke();
				}
				else
				{
					_tutorialText.text = tutorialInfo.TutorialMessage;
					float windowDelay = tutorialInfo.SecondsBeforeMessageIsDisplayed;

					this.DoAfter(windowDelay, () =>
					{
						if(!_tutorialPopupWindow.IsOpen)
						{
							_tutorialPopupWindow.Open();
						}
					});
				}
			}
		});
	}

	public void EndTutorial()
	{
		TutorialStep = 0;
		_tutorialPopupWindow.Close();
		_emilyImage.GetComponent<Animator>().Play("HideEmily");
		this.DoAfter(seconds: 1.0F, () => _emilyImage.gameObject.SetActive(false));
		GridManager.Instance.gameObject.SetActive(true);
		IsGivingTutorial = false;
	}

	private void BuildActiveTutorialInfo()
	{
		List<TutorialInfo> result = new List<TutorialInfo>();

		foreach(TutorialInfo tutorialInfo in _tutorialInfo)
		{
			if(tutorialInfo.Active)
			{
				result.Add(tutorialInfo);
			}
		}

		_activeTutorialInfo = result.ToArray();
	}
}
