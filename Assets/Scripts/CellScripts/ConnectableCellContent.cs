using System;
using System.Collections;

public abstract class ConnectableCellContent : CellContent
{
	public ColorType ColorType { get; protected set; }

	public override bool CanFall => true;

	public abstract IEnumerator ApplyConnectionCoroutine(ConnectableCellContent[] connectedCells, Action<ConnectableCellContent> successCallback, Action<ConnectableCellContent> failureCallback);

	public virtual bool CanConnectWith(ConnectableCellContent connectableCellContent)
	{
		return ColorType == connectableCellContent.ColorType && Cell.IsNeighbor(connectableCellContent.Cell);
	}

	public virtual IEnumerator ApplyConnectionCoroutine()
	{
		return ApplyConnectionCoroutine(connectedCells: new ConnectableCellContent[0], successCallback: NoOperation, failureCallback: NoOperation);
	}

	public virtual IEnumerator ApplyConnectionCoroutine(ConnectableCellContent[] connectedCells)
	{
		return ApplyConnectionCoroutine(connectedCells, successCallback: NoOperation, failureCallback: NoOperation);
	}

	public virtual IEnumerator ApplyConnectionCoroutine(Action<ConnectableCellContent> successCallback)
	{
		return ApplyConnectionCoroutine(connectedCells: new ConnectableCellContent[0], successCallback, failureCallback: NoOperation);
	}

	public virtual IEnumerator ApplyConnectionCoroutine(Action<ConnectableCellContent> successCallback, Action<ConnectableCellContent> failureCallback)
	{
		return ApplyConnectionCoroutine(connectedCells: new ConnectableCellContent[0], successCallback, failureCallback);
	}

	public virtual IEnumerator ApplyConnectionCoroutine(ConnectableCellContent[] connectedCells, Action<ConnectableCellContent> successCallback)
	{
		return ApplyConnectionCoroutine(connectedCells, successCallback, failureCallback: NoOperation);
	}

	private void NoOperation(ConnectableCellContent connectableCellContent)
	{
		//Used as a placeholder for optional callbacks. This avoids unnecessary null checks in every derived class that implements the abstract method ApplyConnectionCoroutine.
	}
}
