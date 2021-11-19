using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
	public int X { get; private set; }
	public int Y { get; private set; }
	public bool IsInitialized { get; private set; } = false;
	public CellContent CellContent { get; private set; }

	public bool IsEmpty => CellContent == null;

	public void Initialize(int x, int y)
	{
		if(IsInitialized)
		{
			throw new Exception("Attempting to re-initialize a cell. Cells can only be initialized once");
		}

		X = x;
		Y = y;
		IsInitialized = true;
	}

	public void SetCellContent(CellContent cellContent)
	{
		cellContent.transform.SetParent(transform, worldPositionStays: false);
		CellContent = cellContent;
	}

	public void DetachCellContent()
	{
		CellContent = null;
	}

	public void DeleteCellContent()
	{
		if(!IsEmpty)
		{
			Destroy(CellContent.gameObject);
			CellContent = null;
		}
	}

	public Cell[] GetOtherCellsInDirection(MoveDirection direction)
	{
		return GetOtherCellsInDirection(direction, filterCondition: (cell) => true);
	}

	public Cell[] GetOtherCellsInDirection(MoveDirection direction, Func<Cell, bool> filterCondition)
	{
		if(direction == MoveDirection.None)
		{
			return new Cell[0];
		}

		List<Cell> result = new List<Cell>();
		Vector2Int incrementValue = direction.ToVector2Int();
		Vector2Int currentLocation = new Vector2Int(X, Y);
		currentLocation += incrementValue;

		while(GridManager.Instance.IsCellCoordinateInBounds(currentLocation.x, currentLocation.y))
		{
			Cell potentialCell = GridManager.Instance[currentLocation.x, currentLocation.y];

			if(filterCondition.Invoke(potentialCell))
			{
				result.Add(potentialCell);
			}

			currentLocation += incrementValue;
		}

		return result.ToArray();
	}

	public Cell[] GetAllNeighbors()
	{
		List<Cell> result = new List<Cell>();

		Vector2Int[] directions =
		{
			new Vector2Int(X - 1, Y),
			new Vector2Int(X + 1, Y),
			new Vector2Int(X, Y - 1),
			new Vector2Int(X, Y + 1)
		};

		foreach(Vector2Int direction in directions)
		{
			if(GridManager.Instance.IsCellCoordinateInBounds(direction.x, direction.y))
			{
				result.Add(GridManager.Instance[direction.x, direction.y]);
			}
		}

		return result.ToArray();
	}

	public bool IsNeighbor(Cell cell)
	{
		int horizontalDistance = Mathf.Abs(X - cell.X);
		int verticalDistance = Mathf.Abs(Y - cell.Y);
		return (horizontalDistance == 1 && verticalDistance == 0) || (horizontalDistance == 0 && verticalDistance == 1);
	}
}
