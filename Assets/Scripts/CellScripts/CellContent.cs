using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CellContent : MonoBehaviour
{
	public abstract bool CanFall { get; }
	public abstract bool TerminatesFalling { get; }

	public Cell Cell => transform.parent.GetComponent<Cell>();

	public abstract CellContent CreateWithRandomProperties();
	public virtual void OnSpecialPowerPostUserTurn() { }

	public IEnumerator MoveToNewCellCoroutine(Cell targetCell, float timeToMove = 2.0F)
	{
		float totalMoveTime = 0.0F;
		Vector3 startPosition = gameObject.transform.position;
		Vector3 targetPosition = targetCell.transform.position;

		while(totalMoveTime < timeToMove)
		{
			gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, totalMoveTime / timeToMove);
			yield return null;
			totalMoveTime += Time.deltaTime;
		}

		gameObject.transform.position = targetPosition;
	}

	public void ProcessFall(Action<CellContent> finishedCallback)
	{
		ProcessFallInternal(columnIndex: null, distanceToFall: null, finishedCallback);
	}

	public void ProcessFall(int columnToFallIn, int distanceToFall, Action<CellContent> finishedCallback)
	{
		ProcessFallInternal(columnToFallIn, distanceToFall, finishedCallback);
	}

	private void ProcessFallInternal(int? columnIndex, int? distanceToFall, Action<CellContent> finishedCallback)
	{
		if(CanFall)
		{
			Cell originCell = columnIndex == null ? Cell : GridManager.Instance[columnIndex.Value, GridManager.Instance.TopColumnIndex];
			int totalCellsToFallThrough = originCell.GetOtherCellsInDirection(MoveDirection.Down, (cell) => cell.IsEmpty || !cell.CellContent.TerminatesFalling).Length;
			int x = originCell.X;
			int y = columnIndex == null ? originCell.Y + totalCellsToFallThrough : distanceToFall.Value - 1;
			Cell targetCell = GridManager.Instance[x, y];
			StartCoroutine(FallCoroutine(originCell, targetCell, finishedCallback, startOffscreen: columnIndex != null));
		}
	}

	private IEnumerator FallCoroutine(Cell originCell, Cell targetCell, Action<CellContent> finishedCallback, bool startOffscreen)
	{
		float totalFallTime = 0.0F;
		float timeToFall = 0.2F;
		Vector3 originalPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y * (startOffscreen ? 2.0F : 1.0F), gameObject.transform.position.z);
		Vector3 targetPosition = targetCell.transform.position;

		while(totalFallTime < timeToFall)
		{
			gameObject.transform.position = Vector3.Lerp(originalPosition, targetPosition, totalFallTime / timeToFall);
			yield return null;
			totalFallTime += Time.deltaTime;
		}

		gameObject.transform.position = targetPosition;
		originCell.DetachCellContent();
		this.DoAfter(() => !GridManager.Instance.IsProcessingFalling, () => targetCell.SetCellContent(this));
		finishedCallback.Invoke(this);
	}
}
