using UnityEngine;

public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
{
	public static T Instance { get; private set; }

	private void Awake()
	{
		if(Instance != null)
		{
			throw new System.Exception($"Attempting to overwrite the singleton instance for {name}");
		}

		Instance = (T) this;
		OnAwake();
	}

	/// <summary>
	/// Override to implement Unity's Awake functionality.
	/// </summary>
	protected virtual void OnAwake()
	{

	}
}
