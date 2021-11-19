using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LineManager : Manager<LineManager>
{
	[SerializeField] private GraphicRaycaster _graphicRaycaster;
	[SerializeField] private EventSystem _eventSystem;
	[SerializeField] private LineUI _linePrefab;
	[SerializeField] private Transform _linesContainer;
	[SerializeField] private DotActivationBorder _dotActivationBorder;

	private readonly List<ConnectableCellContent> _connectedCells = new List<ConnectableCellContent>();
	private readonly Stack<CellConnection> _connectedLines = new Stack<CellConnection>();
	private ConnectableCellContent _previousCell;
	private ConnectableCellContent _backtrackCell;
	private bool _isSquareMade = false;
	private bool _areSoundsAllowed = false;
	private ColorType? _lineColor = null;

	public bool IsDrawingLines => _connectedLines.Count > 0;

	private void Update()
	{
		if(AnimationManager.Instance.IsAnimating)
		{
			StopConnecting();
			return;
		}

		if(Input.GetMouseButtonDown(0))
		{
			StartConnecting();
		}
		else if(Input.GetMouseButton(0) && IsDrawingLines)
		{
			UpdateConnecting();
		}
		else if(Input.GetMouseButtonUp(0))
		{
			StopConnecting();
		}
	}

	private void StartConnecting()
	{
		if(TryGetConnectableCellContentUnderPointer(out ConnectableCellContent cell))
		{
			_lineColor = cell.ColorType;
			_areSoundsAllowed = true;
			PushLine(cell);
		}
	}

	private void UpdateConnecting()
	{
		if(TryGetConnectableCellContentUnderPointer(out ConnectableCellContent cell) && cell != _previousCell && cell.CanConnectWith(_previousCell))
		{
			ProcessNewCellConnection(cell);
		}
		else
		{
			_connectedLines.Peek().Line.EndPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		}
	}

	private void StopConnecting()
	{
		List<ConnectableCellContent> cellsToConnect = new List<ConnectableCellContent>();
		bool wasSquareMade = _isSquareMade;
		bool isRegularConnection = !wasSquareMade && _connectedLines.Count > 1;
		_areSoundsAllowed = false;

		while(_connectedLines.Count > 0)
		{
			if(isRegularConnection)
			{
				cellsToConnect.Add(_connectedLines.Peek().CurrentCell);
			}

			PopLine();
		}

		if(wasSquareMade)
		{
			foreach(Cell sameColoredCell in GetSameColoredCells())
			{
				cellsToConnect.Add(sameColoredCell.CellContent as ConnectableCellContent);
			}

			CellContentSpawnManager.Instance.DeactivateColor(_lineColor.Value);
		}

		ApplyFinalConnections(cellsToConnect);
		_connectedCells.Clear();
		_lineColor = null;
		_previousCell = null;
		_backtrackCell = null;
		_isSquareMade = false;
	}

	private void ProcessNewCellConnection(ConnectableCellContent cell)
	{
		if(_connectedCells.Contains(cell))
		{
			if(cell == _backtrackCell)
			{
				PopLine();
			}
			else if(!_isSquareMade)
			{
				_isSquareMade = true;
				PushLine(cell);
			}
		}
		else if(!_isSquareMade)
		{
			PushLine(cell);
		}
	}

	private void PushLine(ConnectableCellContent cell)
	{
		if(_connectedLines.Count > 0)
		{
			_connectedLines.Peek().Line.EndPoint = cell.transform.position;
		}

		LineUI line = Instantiate(_linePrefab);
		line.transform.SetParent(_linesContainer, worldPositionStays: false);
		line.Color = _isSquareMade ? Color.clear : CellContentSpawnManager.Instance.GetDotColor(cell.ColorType);
		line.StartPoint = cell.transform.position;
		_connectedLines.Push(new CellConnection() { Line = line, CurrentCell = cell, PreviousCell = _previousCell, BacktrackCell = _backtrackCell });
		_connectedCells.Add(cell);
		_backtrackCell = _previousCell;
		_previousCell = cell;
		UpdateFX();
	}

	private void PopLine()
	{
		CellConnection cellConnection = _connectedLines.Pop();
		Destroy(cellConnection.Line.gameObject);
		_connectedCells.Remove(_previousCell);
		_backtrackCell = cellConnection.BacktrackCell;
		_previousCell = cellConnection.PreviousCell;
		_isSquareMade = false;
		UpdateFX();
	}

	private bool TryGetConnectableCellContentUnderPointer(out ConnectableCellContent connectableCellContent)
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		pointerEventData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		_graphicRaycaster.Raycast(pointerEventData, results);
		connectableCellContent = results.Count > 0 ? results[0].gameObject.GetComponentInParent<ConnectableCellContent>() : null;
		return connectableCellContent != null;
	}

	private void ApplyFinalConnections(List<ConnectableCellContent> connections)
	{
		int totalConnectedCells = connections.Count;

		foreach(ConnectableCellContent connection in connections)
		{
			AnimationManager.Instance.QueueAnimation(() => totalConnectedCells == 0);
			StartCoroutine(connection.ApplyConnectionCoroutine(successCallback: (cell) => totalConnectedCells--));
		}
	}

	private void UpdateFX()
	{
		UpdateDotActivationBorder();
		PlayConnectedDotSound();
	}

	private void UpdateDotActivationBorder()
	{
		_dotActivationBorder.FillPercent = _isSquareMade ? 1.0F : Mathf.Clamp01(_connectedLines.Count / (GetSameColoredCells().Length + 1.0F));
		_dotActivationBorder.ActivationColor = CellContentSpawnManager.Instance.GetDotColor(_lineColor.Value);
	}

	private void PlayConnectedDotSound()
	{
		if(_areSoundsAllowed)
		{
			if(_isSquareMade)
			{
				AudioManager.Instance.PlaySoundOneShot(_connectedLines.Count > 10 ? "SquareHigh" : "SquareLow");
			}
			else
			{
				int noteStep = Mathf.FloorToInt(Mathf.PingPong(_connectedLines.Count, 19)) - 8;
				float pitch = MusicalScale.GetDiatonicPitchShift(noteStep);
				AudioManager.Instance.PlaySoundOneShot("DotSelect", pitch);
			}
		}
	}

	private Cell[] GetSameColoredCells()
	{
		if(!_lineColor.HasValue)
		{
			return new Cell[0];
		}

		return GridManager.Instance.GetCellsWhere((cell) => cell.CellContent is ConnectableCellContent content && content.ColorType == _lineColor.Value);
	}
}
