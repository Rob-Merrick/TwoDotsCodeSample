using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchManager : Manager<SceneSwitchManager>
{
	public void SwitchScenes(string sceneName)
	{
		ScreenOverlayManager.Instance.FadeToColor(Color.white, 2.0F, () => SceneManager.LoadScene(sceneName));
	}
}
