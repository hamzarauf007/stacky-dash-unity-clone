using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacement : MonoBehaviour
{
	[SerializeField] private Vector2Int m_GridSize = Vector2Int.one;
	[SerializeField] private float m_ChunkSize = 1;
	[SerializeField] private float m_Spacing = 0;
	[SerializeField] private Vector3 m_Origin = Vector3.zero;

	public void Init(Vector2Int gridSize, Vector3 origin, float chunkSize = 1, float spacing = 0)
	{
		m_GridSize = gridSize;
		m_Origin = origin;
		m_ChunkSize = chunkSize;
		m_Spacing = spacing;
	}

	public bool WorldToGrid(Vector3 worldPosition, out Vector2Int grid)
	{
		grid = Vector2Int.zero;
		grid.x = Mathf.RoundToInt(worldPosition.x);
		grid.y = Mathf.RoundToInt(worldPosition.z);

		return grid.x >= 0 && grid.x < m_GridSize.x && grid.y >= 0 && grid.y < m_GridSize.y;
	}

	public Vector3 GridToWorldXZ(Vector2Int grid)
	{
		Vector2 xy = GetPosition(grid.x, grid.y);
		Vector3 pos = new Vector3(xy.x, 0, xy.y) + m_Origin;
		return pos;
	}

	private Vector2 GetPosition(int x, int y)
	{
		Vector2 xy = new Vector2(x, y);

		Vector2 centerOffset = Vector2.one * 0.5f;

		Vector2 gridOffset = new Vector2(-m_GridSize.x * 0.5f, -m_GridSize.y * 0.5f);

		Vector2 pos = gridOffset + xy + centerOffset;

		Vector2 position = pos * m_ChunkSize + pos * m_Spacing;

		return position;
	}

	public Vector3[] CalculateXZ()
	{
		Vector3[] positions = new Vector3[m_GridSize.x * m_GridSize.y];

		for (int y = 0; y < m_GridSize.y; y++)
		{
			for (int x = 0; x < m_GridSize.x; x++)
			{
				//Vector2 xy = GetPosition(x, y);

				int index = y * m_GridSize.x + x;
				positions[index] = GridToWorldXZ(new Vector2Int(x, y));
			}
		}

		return positions;
	}
	public static Vector3 SnapToGrid(Vector3 position, Vector3 cellSize)
	{
		position.x = Mathf.Round(position.x / cellSize.x) * cellSize.x;
		position.y = Mathf.Round(position.y / cellSize.y) * cellSize.y;
		position.z = Mathf.Round(position.z / cellSize.z) * cellSize.z;
		return position;
	}
}
