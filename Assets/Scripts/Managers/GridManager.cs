using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Manager<GridManager>
{
	[SerializeField] private bool _loadLevel = false;

	//TODO: Implement variable sized boards
	/*[SerializeField] [Range(0, _maxBoardWidth - _minBoardWidth)]*/   private int _leftColumnIndex = 0;
	/*[SerializeField] [Range(0, _maxBoardHeight - _minBoardHeight)]*/ private int _topColumnIndex = 0;
	/*[SerializeField] [Range(_minBoardWidth, _maxBoardWidth)]*/       private int _columnCount = 8;
	/*[SerializeField] [Range(_minBoardHeight, _maxBoardHeight)]*/     private int _rowCount = 11;

	private const int _minBoardWidth = 2;
	private const int _minBoardHeight = 2;
	private const int _maxBoardWidth = 8;
	private const int _maxBoardHeight = 11;

	private Cell[,] _cells;
	private readonly List<Cell> _cellsToErase = new List<Cell>();
	private bool _isUserTurnFinished = true;
	private bool _isProcessingUserTurn = false;

	public bool IsInitialized { get; private set; } = false;
	public bool IsProcessingFalling { get; private set; } = false;
	public int LeftColumnIndex => _leftColumnIndex;
	public int RightColumnIndex => _leftColumnIndex + _columnCount - 1;
	public int TopColumnIndex => _topColumnIndex;
	public int BottomColumnIndex => _topColumnIndex + _rowCount - 1;

	public Cell this[int x, int y] => GetCellAt(x, y);

	private void Start()
	{
		CollectAllCells();
		GenerateBoard();
		IsInitialized = true;
	}

	private void Update()
	{
		if(!_isUserTurnFinished && !_isProcessingUserTurn)
		{
			AnimationManager.Instance.QueueAnimation(() => _isUserTurnFinished);
			_isProcessingUserTurn = true;
			EmptyCellsToEraseQueue();
			//TODO: Implement checks for power ups that run before gravity kicks in
			ProcessFalling();
		}
	}

	public bool IsCellCoordinateInBounds(int x, int y)
	{
		return x >= LeftColumnIndex && x <= RightColumnIndex && y >= TopColumnIndex && y <= BottomColumnIndex;
	}

	public void QueueCellErase(Cell cell)
	{
		_cellsToErase.Add(cell);
		_isUserTurnFinished = false;
	}

	public Cell GetCellAt(int x, int y)
	{
		return _cells[x, y];
	}

	public Cell[,] GetAllCells()
	{
		return (Cell[,]) _cells.Clone();
	}

	public Cell[] GetCellsWhere(Func<Cell, bool> filterCondition)
	{
		List<Cell> result = new List<Cell>();

		foreach(Cell cell in _cells)
		{
			if(filterCondition.Invoke(cell))
			{
				result.Add(cell);
			}
		}

		return result.ToArray();
	}

	private void GenerateBoard()
	{
		ClearBoard();

		for(int x = _leftColumnIndex; x < _leftColumnIndex + _columnCount; x++)
		{
			for(int y = _topColumnIndex; y < _topColumnIndex + _rowCount; y++)
			{
				if(_loadLevel)
				{
					Dot dot = CellContentSpawnManager.Instance.CreateCellContent<Dot>(isRandom: false);
					ColorType colorType = NoMovesLevel.GetColorType(NoMovesLevel.LevelData[y, x]);
					dot.InitializeDot(colorType);
					_cells[x, y].SetCellContent(dot);
				}
				else
				{
					_cells[x, y].SetCellContent(SpawnNewCellContent());
				}
			}
		}
	}

	private void ClearBoard()
	{
		for(int x = _leftColumnIndex; x < _leftColumnIndex + _columnCount; x++)
		{
			for(int y = _topColumnIndex; y < _topColumnIndex + _rowCount; y++)
			{
				Cell cell = _cells[x, y];

				if(cell != null && !cell.IsEmpty)
				{
					cell.DeleteCellContent();
				}
			}
		}
	}

	private void CollectAllCells()
	{
		_cells = new Cell[_leftColumnIndex + _columnCount, _topColumnIndex + _rowCount];

		for(int i = 0; i < transform.GetChild(0).childCount; i++)
		{
			int x = i % _maxBoardWidth;
			int y = i / _maxBoardWidth;

			if(IsCellCoordinateInBounds(x, y))
			{
				Cell cell = transform.GetChild(0).GetChild(i).GetComponent<Cell>();
				cell.Initialize(x: i % _maxBoardWidth, y: i / _maxBoardWidth);
				_cells[cell.X, cell.Y] = cell;
			}
		}
	}

	private void EmptyCellsToEraseQueue()
	{
		foreach(Cell cell in _cellsToErase)
		{
			cell.DeleteCellContent();
		}

		_cellsToErase.Clear();
	}

	private void ProcessFalling()
	{
		IsProcessingFalling = true;
		int totalFallingCells = 0;

		for(int x = _leftColumnIndex; x < _leftColumnIndex + _columnCount; x++)
		{
			int totalEmptyCellsInColumn = 0;

			for(int y = _topColumnIndex; y < _topColumnIndex + _rowCount; y++)
			{
				Cell cell = _cells[x, y];

				if(cell.IsEmpty)
				{
					totalFallingCells++;
					totalEmptyCellsInColumn++;
					SpawnNewCellContentFromAbove(x).ProcessFall(x, totalEmptyCellsInColumn, (cellContent) => totalFallingCells--);
				}
				else if(cell.CellContent.CanFall)
				{
					totalFallingCells++;
					cell.CellContent.ProcessFall((cellContent) => totalFallingCells--);
				}
			}
		}

		this.DoAfter(() => totalFallingCells == 0, () =>
		{
			IsProcessingFalling = false;
			_isProcessingUserTurn = false;
			_isUserTurnFinished = true;
			CellContentSpawnManager.Instance.ResetActiveColors();

			this.DoAfter(frames: 1, () =>
			{
				foreach(Cell cell in _cells)
				{
					cell.CellContent.OnSpecialPowerPostUserTurn();
				}

				this.DoAfter(() => !AnimationManager.Instance.IsAnimating, () => ShuffleManager.Instance.CheckForNoMoves());
			});
		});
	}

	private CellContent SpawnNewCellContent()
	{
		return CellContentSpawnManager.Instance.CreateRandomCellContent();
	}

	private CellContent SpawnNewCellContentFromAbove(int columnIndex)
	{
		CellContent cellContent = SpawnNewCellContent();
		Cell referenceCell = _cells[columnIndex, _topColumnIndex];
		cellContent.transform.SetParent(referenceCell.transform, worldPositionStays: false);
		return cellContent;
	}
}
