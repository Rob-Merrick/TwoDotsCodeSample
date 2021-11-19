using UnityEngine;

[System.Serializable]
public class CellContentSpawnInfo
{
	public bool Active = true;
	public CellContent CellContentPrefab;
	[Range(0.001F, 1.0F)] public float SpawnProbability;
}
