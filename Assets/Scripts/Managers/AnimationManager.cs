using System;

public class AnimationManager : Manager<AnimationManager>
{
	private int _animationCount;

	/// <summary>
	/// Returns true if any animations have been queued via <see cref="QueueAnimation(Func{bool})"/> and haven't finished yet.
	/// </summary>
	public bool IsAnimating => _animationCount > 0;

	/// <summary>
	/// Call this method to track an animation that is playing (such as dots clearing). This will cause
	/// <see cref="IsAnimating"/> to return true until all animations have finished playing, signaled via 
	/// <paramref name="animationFinishedCondition"/>.
	/// </summary>
	public void QueueAnimation(Func<bool> animationFinishedCondition)
	{
		_animationCount++;
		this.DoAfter(animationFinishedCondition, () => _animationCount--);
	}
}
