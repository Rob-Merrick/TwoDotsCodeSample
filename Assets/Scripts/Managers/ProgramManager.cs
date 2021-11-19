public class ProgramManager : Manager<ProgramManager>
{
	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		UnityEngine.Application.Quit();
#endif
	}
}
