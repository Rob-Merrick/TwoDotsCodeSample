using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static void DoAfter(this MonoBehaviour monoBehaviour, Func<bool> condition, Action action)
	{
		monoBehaviour.StartCoroutine(DoAfterCoroutine(condition, action));
	}

	public static void DoAfter(this MonoBehaviour monoBehaviour, float seconds, Action action)
	{
		monoBehaviour.StartCoroutine(DoAfterCoroutine(seconds, action));
	}

	public static void DoAfter(this MonoBehaviour monoBehaviour, int frames, Action action)
	{
		monoBehaviour.StartCoroutine(DoAfterCoroutine(frames, action));
	}

	private static IEnumerator DoAfterCoroutine(Func<bool> condition, Action action)
	{
		while(!condition.Invoke())
		{
			yield return null;
		}

		action.Invoke();
	}

	private static IEnumerator DoAfterCoroutine(float seconds, Action action)
	{
		yield return new WaitForSeconds(seconds);
		action.Invoke();
	}

	private static IEnumerator DoAfterCoroutine(int frames, Action action)
	{
		for(int i = 0; i < frames; i++)
		{
			yield return null;
		}

		action.Invoke();
	}
}
