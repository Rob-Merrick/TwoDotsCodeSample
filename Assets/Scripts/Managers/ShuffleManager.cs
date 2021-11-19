using System.Collections;
using UnityEngine;

public class ShuffleManager : Manager<ShuffleManager>
{
	[SerializeField] private GameObject _noMoreMovesWindow;

	public bool IsShuffling { get; private set; } = false;

	public bool IsAnyMoveAvailable()
	{
		Cell[,] cells = GridManager.Instance.GetAllCells();

		foreach(Cell cell in cells)
		{
			if(!cell.IsEmpty && cell.CellContent is ConnectableCellContent content)
			{
				foreach(Cell neighbor in cell.GetAllNeighbors())
				{
					if(!neighbor.IsEmpty && neighbor.CellContent is ConnectableCellContent neighborContent && content.CanConnectWith(neighborContent))
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public void Shuffle()
	{
		if(!IsShuffling)
		{
			StartCoroutine(ShuffleCoroutine(showNoMoreMovesWindow: false));
		}
	}

	public void CheckForNoMoves()
	{
		if(IsAnyMoveAvailable())
		{
			IsShuffling = false;
		}
		else
		{
			StartCoroutine(ShuffleCoroutine(showNoMoreMovesWindow: true));
		}
	}

	private IEnumerator ShuffleCoroutine(bool showNoMoreMovesWindow)
	{
		Cell[,] cells = GridManager.Instance.GetAllCells();
		float timeToMove = 3.0F / cells.Length;
		int leftColumnIndex = GridManager.Instance.LeftColumnIndex;
		int rightColumnIndex = GridManager.Instance.RightColumnIndex;
		int topColumnIndex = GridManager.Instance.TopColumnIndex;
		int bottomColumnIndex = GridManager.Instance.BottomColumnIndex;

		if(!IsShuffling)
		{
			IsShuffling = true;
			AnimationManager.Instance.QueueAnimation(() => !IsShuffling);

			if(showNoMoreMovesWindow)
			{
				_noMoreMovesWindow.SetActive(true);
				_noMoreMovesWindow.GetComponentInChildren<Animator>().SetTrigger("Display");
				yield return new WaitForSeconds(1.5F);

				this.DoAfter(seconds: 3.0F, () =>
				{
					_noMoreMovesWindow.SetActive(false);
				});
			}
		}

		for(int x = leftColumnIndex; x <= rightColumnIndex; x++)
		{
			for(int y = topColumnIndex; y < bottomColumnIndex; y++)
			{
				Cell currentCell = cells[x, y];
				Cell randomCell = cells[Random.Range(leftColumnIndex, rightColumnIndex + 1), Random.Range(topColumnIndex, bottomColumnIndex + 1)];
				CellContent currentContent = currentCell.CellContent;
				CellContent randomContent = randomCell.CellContent;
				Coroutine coroutine1 = StartCoroutine(currentContent.MoveToNewCellCoroutine(randomCell, timeToMove));
				Coroutine coroutine2 = StartCoroutine(randomContent.MoveToNewCellCoroutine(currentCell, timeToMove));
				AudioManager.Instance.PlaySoundOneShot("Woosh", Random.Range(0.8F, 1.3F));
				yield return coroutine1;
				yield return coroutine2;
				currentCell.SetCellContent(randomContent);
				randomCell.SetCellContent(currentContent);
			}
		}

		CheckForNoMoves();
	}
}
