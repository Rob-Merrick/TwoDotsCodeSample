using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MoveDirectionExtensions
{
	/// <summary>
	/// Returns a vector that represents 1 unit of movement for the given direction, where positive x is to the right and positive y is downward.
	/// Returns a zero vector for a value of None.
	/// </summary>
	public static Vector2Int ToVector2Int(this MoveDirection moveDirection)
	{
		switch(moveDirection)
		{
			case MoveDirection.Up:		return Vector2Int.down;
			case MoveDirection.Down:	return Vector2Int.up;
			case MoveDirection.Left:	return Vector2Int.left;
			case MoveDirection.Right:	return Vector2Int.right;
			case MoveDirection.None:	return Vector2Int.zero;
			default:					throw new NotImplementedException();
		}
	}
}
