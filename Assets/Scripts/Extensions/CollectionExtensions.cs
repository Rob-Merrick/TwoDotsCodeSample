using System;
using System.Collections.Generic;

public static class CollectionExtensions
{
	private static readonly Random _random = new Random();

	public static T GetRandom<T>(this IList<T> input)
	{
		if(input == null)
		{
			throw new ArgumentNullException(nameof(input));
		}

		if(input.Count == 0)
		{
			throw new IndexOutOfRangeException();
		}

		return input[_random.Next(0, input.Count)];
	}

	public static T RemoveRandom<T>(this List<T> input)
	{
		if(input == null)
		{
			throw new ArgumentNullException(nameof(input));
		}

		if(input.Count == 0)
		{
			throw new IndexOutOfRangeException();
		}

		int randomIndex = _random.Next(0, input.Count);
		T result = input[randomIndex];
		input.RemoveAt(randomIndex);
		return result;
	}
}
