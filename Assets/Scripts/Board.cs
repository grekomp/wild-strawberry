using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public static Board instance;
	public static Dot[,] dots;

	// Board options
	public int width = 8;
	public int height = 8;
	public Color[] colors;
	public Vector2 spawnOffset = new Vector2(0, 10);

	// Dot prefab
	public GameObject dotPrefab;

	private void Start()
	{
		// Updating instance
		if (instance == null)
		{
			instance = this;
		} else
		{
			if (instance != this)
			{
				Destroy(gameObject);
			}
		}

		// Initializing dots
		dots = new Dot[width, height];

		// Filling dots array
		FillEmptyBoardSlots();
	}

	/// <summary>
	/// Handles all actions after a dot has been clicked
	/// </summary>
	/// <param name="clickedDot">The Dot that was clicked</param>
	public static void Clicked(Dot clickedDot)
	{
		// Getting index of clicked dot
		Vector2 index;
		bool isValidClickedDot = GetIndex(clickedDot, out index);

		int x = (int)index.x;
		int y = (int)index.y;

		if (isValidClickedDot)
		{
			instance.PopAdjacentDotsOfSameColor(x, y, clickedDot);

			instance.MoveDotsDown();

			instance.FillEmptyBoardSlots();
		}

	}

	/// <summary>
	/// Returns world position of a dot at specified index
	/// </summary>
	Vector2 IndexToWorldPosition(int x, int y)
	{
		return IndexToWorldPosition(new Vector2(x, y));
	}

	/// <summary>
	/// Returns world position of a dot at specified index
	/// </summary>
	Vector2 IndexToWorldPosition(Vector2 index)
	{
		return new Vector2(index.x, index.y);
	}

	/// <summary>
	/// Returns the indexes of a dot on board
	/// </summary>
	static bool GetIndex(Dot dot, out Vector2 index)
	{
		for (int i = 0; i < instance.width; i++)
		{
			for (int j = 0; j < instance.height; j++)
			{
				if(Object.ReferenceEquals(dot, dots[i, j]))
				{
					index = new Vector2(i, j);
					return true;
				}
			}
		}

		index = new Vector2();
		return false;
	}

	/// <summary>
	/// Returns a dot from dots array with specified index
	/// </summary>
	Dot GetDotAtIndex(int x, int y)
	{
		return GetDotAtIndex(new Vector2(x, y));
	}

	/// <summary>
	/// Returns a dot from dots array with specified index
	/// </summary>
	Dot GetDotAtIndex(Vector2 index)
	{
		return dots[(int)index.x, (int)index.y];
	}

	/// <summary>
	/// Fills null board slots with random dots
	/// </summary>
	/// <returns>Amount of filled slots</returns>
	int FillEmptyBoardSlots()
	{
		int filledSlots = 0;

		// Checks every slot
		for(int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				// If slot was empty
				if (GetDotAtIndex(i, j) == null)
				{
					filledSlots++;

					PlaceRandomDotAtIndex(new Vector2(i, j));
				}
			}
		}

		return filledSlots;
	}

	/// <summary>
	/// Adds a random dot to the dots array at a specified index
	/// </summary>
	/// <param name="index"></param>
	void PlaceRandomDotAtIndex(Vector2 index)
	{
		GameObject createdDot = GameObject.Instantiate(dotPrefab) as GameObject;
		Dot dot = createdDot.GetComponent<Dot>();

		// Assigning a random color
		dot.color = GetRandomDotColor();

		// Assigning position
		Vector2 dotPosition = IndexToWorldPosition(index);
		createdDot.transform.position = dotPosition + spawnOffset;
		dot.targetPosition = dotPosition;

		// Add dot to array
		dots[(int)index.x, (int)index.y] = dot;
	}

	/// <summary>
	/// Gets a random color from colors array
	/// </summary>
	/// <returns>A random dot color</returns>
	public Color GetRandomDotColor()
	{
		return colors[Random.Range(0, colors.Length)];
	}

	/// <summary>
	/// Deletes dots adjacent to the index, if they are of the same color as the dot provided
	/// </summary>
	/// <param name="index">Index in the dots array</param>
	/// <param name="dot">Original dot</param>
	void PopAdjacentDotsOfSameColor(int x, int y, Dot dot)
	{
		// Top
		if (PopDotIfSameColor(x, y + 1, dot))
		{
			PopAdjacentDotsOfSameColor(x, y + 1, dot);
		}

		// Bottom
		if (PopDotIfSameColor(x, y - 1, dot))
		{
			PopAdjacentDotsOfSameColor(x, y - 1, dot);
		}

		// Right
		if (PopDotIfSameColor(x + 1, y, dot))
		{
			PopAdjacentDotsOfSameColor(x + 1, y, dot);
		}

		// Left
		if (PopDotIfSameColor(x - 1, y, dot))
		{
			PopAdjacentDotsOfSameColor(x - 1, y, dot);
		}
	}

	/// <summary>
	/// Removes a dot from dots array if its the same color as dot provided
	/// </summary>
	/// <returns>Returns true if dot was removed</returns>
	bool PopDotIfSameColor(int x, int y, Dot dot)
	{
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			Dot dotToPop = GetDotAtIndex(x, y);

			if (dotToPop != null && dotToPop.color == dot.color)
			{
				dots[x, y] = null;
				dotToPop.Pop();

				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Moves dots with nothing under down
	/// </summary>
	public void MoveDotsDown()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				// If slot was empty
				if (GetDotAtIndex(i, j) == null)
				{
					Dot dotAbove = FindDotAbove(i, j);

					if (dotAbove != null)
					{
						Vector2 index;
						GetIndex(dotAbove, out index);

						dots[(int)index.x, (int)index.y] = null;
						dots[i, j] = dotAbove;
						dotAbove.targetPosition = IndexToWorldPosition(i, j);
					}
				}
			}
		}
	}

	/// <summary>
	/// Finds nearest dot above given index
	/// </summary>
	Dot FindDotAbove(int x, int y)
	{
		for (int i = y + 1; i < height; i++)
		{
			if (GetDotAtIndex(x, i) != null)
			{
				return GetDotAtIndex(x, i);
			}
		}

		return null;
	}
}
