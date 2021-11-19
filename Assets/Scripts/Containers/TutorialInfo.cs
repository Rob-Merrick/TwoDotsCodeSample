using UnityEngine.Events;

[System.Serializable]
public class TutorialInfo
{
	public bool Active = true;
	public bool ClosePopupAfterMessageDisplayed = false;
	public bool UseUnityEvent = false;
	public string TutorialMessage = "";
	public float SecondsBeforeMessageIsDisplayed = 0.0F;
	public UnityEvent TutorialEvent;
}
