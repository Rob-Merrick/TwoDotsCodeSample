using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellContentSpawnManager : Manager<CellContentSpawnManager>
{
	[SerializeField] private CellContentSpawnInfo[] _allSpawnInfo;
	[SerializeField] private ColorDefinition[] _dotColors;

	private readonly IDictionary<ColorType, ColorDefinition> _colorDefinitionLookup = new Dictionary<ColorType, ColorDefinition>();
	private readonly List<ColorType> _activeDotColors = new List<ColorType>();

	public bool IsInitialized { get; private set; } = false;
	public List<ColorType> ActiveDotColors => new List<ColorType>(_activeDotColors);

	private void Start()
	{
		GenerateDotColorLookup();
		IsInitialized = true;
	}

	public CellContent CreateRandomCellContent()
	{
		int tryCount = 0;

		while(tryCount < 1000)
		{
			List<CellContentSpawnInfo> activeSpawnInfo = new List<CellContentSpawnInfo>();

			foreach(CellContentSpawnInfo potentialSpawnInfo in _allSpawnInfo)
			{
				if(potentialSpawnInfo.Active)
				{
					activeSpawnInfo.Add(potentialSpawnInfo);
				}
			}

			while(activeSpawnInfo.Count > 0)
			{
				CellContentSpawnInfo spawnInfo = activeSpawnInfo.RemoveRandom();
				float randomValue = Random.value;

				if(randomValue <= spawnInfo.SpawnProbability)
				{
					return Instantiate(spawnInfo.CellContentPrefab).CreateWithRandomProperties();
				}
			}

			tryCount++;
		}

		throw new Exception("Could not generate a random cell content. Try increasing the probabilities of spawning a given cell content");
	}

	public T CreateCellContent<T>(bool isRandom = true) where T : CellContent
	{
		Type concreteType = typeof(T);

		foreach(CellContentSpawnInfo spawnInfo in _allSpawnInfo)
		{
			if(spawnInfo.CellContentPrefab.GetType() == concreteType)
			{
				T cellContent = (T) Instantiate(spawnInfo.CellContentPrefab);

				if(isRandom)
				{
					cellContent.CreateWithRandomProperties();
				}

				return cellContent;
			}
		}

		throw new Exception($"Could not find spawn info for cell content type {concreteType.Name}");
	}

	public bool IsDotColorAllowedInThisLevel(ColorType colorType)
	{
		return _colorDefinitionLookup[colorType].IsActive;
	}

	public bool IsDotColorActive(ColorType colorType)
	{
		return _activeDotColors.Contains(colorType);
	}

	public void ResetActiveColors()
	{
		GenerateDotColorLookup();
	}

	public void ActivateColor(ColorType colorType)
	{
		if(!_colorDefinitionLookup[colorType].IsActive)
		{
			throw new Exception($"Cannot activate the color {colorType} because it is not allowed in this level");
		}

		if(!_activeDotColors.Contains(colorType))
		{
			_activeDotColors.Add(colorType);
		}
	}

	public void DeactivateColor(ColorType colorType)
	{
		if(!_colorDefinitionLookup[colorType].IsActive)
		{
			throw new Exception($"Cannot deactivate the color {colorType} because it is not allowed in this level");
		}

		_activeDotColors.Remove(colorType);

		if(_activeDotColors.Count == 0)
		{
			throw new Exception("All colors have been deactivated! No dots can be generated");
		}
	}

	public Color GetDotColor(ColorType colorType)
	{
		return _colorDefinitionLookup[colorType].Color;
	}

	private void GenerateDotColorLookup()
	{
		_colorDefinitionLookup.Clear();
		_activeDotColors.Clear();

		foreach(ColorDefinition colorDefinition in _dotColors)
		{
			_colorDefinitionLookup.Add(colorDefinition.ColorType, colorDefinition);

			if(colorDefinition.IsActive)
			{
				_activeDotColors.Add(colorDefinition.ColorType);
			}
		}
	}
}
