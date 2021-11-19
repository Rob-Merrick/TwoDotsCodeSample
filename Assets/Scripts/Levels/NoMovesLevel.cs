//TODO: Make a level loader instead of the hard-coded data like this.
public static class NoMovesLevel
{
	public static char[,] LevelData => new [,]
	{
		{ 'R', 'R', 'Y', 'G', 'B', 'P', 'I', 'W' },
		{ 'O', 'Y', 'G', 'B', 'P', 'I', 'W', 'B' },
		{ 'R', 'O', 'Y', 'G', 'B', 'P', 'I', 'W' },
		{ 'O', 'Y', 'G', 'B', 'P', 'I', 'W', 'B' },
		{ 'R', 'O', 'Y', 'G', 'B', 'P', 'I', 'W' },
		{ 'O', 'Y', 'G', 'B', 'P', 'I', 'W', 'B' },
		{ 'R', 'O', 'Y', 'G', 'B', 'P', 'I', 'W' },
		{ 'O', 'Y', 'G', 'B', 'P', 'I', 'W', 'B' },
		{ 'R', 'O', 'Y', 'G', 'B', 'P', 'I', 'W' },
		{ 'O', 'Y', 'G', 'B', 'P', 'I', 'W', 'B' },
		{ 'R', 'O', 'Y', 'G', 'B', 'P', 'I', 'W' },
	};

	public static ColorType GetColorType(char lookup)
	{
		switch(lookup)
		{
			case 'R': return ColorType.Red;
			case 'O': return ColorType.Orange;
			case 'Y': return ColorType.Yellow;
			case 'G': return ColorType.Green;
			case 'B': return ColorType.Blue;
			case 'P': return ColorType.Purple;
			case 'I': return ColorType.Pink;
			case 'T': return ColorType.Turquoise;
			case 'W': return ColorType.White;
			case 'L': return ColorType.Black;
			default: throw new System.NotImplementedException();
		}
	}
}
