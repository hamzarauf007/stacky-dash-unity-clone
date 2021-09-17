using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [SerializeField] private float m_CellSize = 1;

    [SerializeField] private Vector3[] corners = null;

    [SerializeField] private GridPlacement m_Grid = null;

    public Transform MyPoint;
    public Vector2Int MyGrid;
    public List<Vector3> Subdivided;

	private void OnDrawGizmos()
	{
        if (transform.childCount < 2) return;

        corners = CalculateCorners();

        DrawLines();
        DrawCells();

        if (!m_Grid) return;

        Gizmos.color = Color.blue;
        DrawGrid(0.1f);

        bool inside = m_Grid.WorldToGrid(MyPoint.position, out MyGrid);
        Vector3 pos = m_Grid.GridToWorldXZ(MyGrid);

        Gizmos.color = inside ? Color.green : Color.red;
        Gizmos.DrawWireSphere(pos, 0.5f);

        Gizmos.color = Color.yellow;
        DrawPath(0.2f);
    }

	private void DrawLines()
	{
		for (int i = 0; i < corners.Length - 1; i++)
		{
			Vector3 a = corners[i];
			Vector3 b = corners[i + 1];
            Gizmos.DrawLine(a, b);
		}
	}

	private void DrawCells()
	{
        Vector3 size = Vector3.one * m_CellSize;
        size.y = 0;

        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawWireCube(corners[i], size);
        }
    }

    private void DrawGrid(float cellRadius)
	{
        CalculateBounds(out Vector3 center, out Vector3 size);

        //Gizmos.DrawWireCube(center, size);

        Vector2Int gridSize = Vector2Int.zero;
        gridSize.x = Mathf.RoundToInt(size.x / m_CellSize) + 1;
        gridSize.y = Mathf.RoundToInt(size.z / m_CellSize) + 1;

        m_Grid.Init(gridSize, center, m_CellSize);

        var positions = m_Grid.CalculateXZ();

		for (int i = 0; i < positions.Length; i++)
		{
            Gizmos.DrawWireSphere(positions[i], cellRadius);
		}
	}

    private void DrawPath(float cellRadius)
    {
        var path = SubdividePath(corners);

        Subdivided = path;

        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.DrawWireSphere(path[i], cellRadius);
        }
    }

    private Vector3[] CalculateCorners()
	{
        Vector3[] corners = new Vector3[transform.childCount];

        for (int i = 0; i < corners.Length; i++)
        {
            var p = transform.GetChild(i).position;
            corners[i] = GridPlacement.SnapToGrid(p, Vector3.one * m_CellSize);
        }

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 a = corners[i - 1];
            Vector3 b = corners[i];

            Vector3 dir = ConvertDirection(b - a, out float length);
            Vector3 finish = a + dir * length;

            corners[i] = finish;
        }

        return corners;
    }

    private void CalculateBounds(out Vector3 center, out Vector3 size)
	{
        Bounds bounds = new Bounds();

        for (int i = 0; i < corners.Length; i++)
        {
            bounds.Encapsulate(corners[i]);
        }

        center = bounds.center;
        size = bounds.size;
    }

    private Vector3 ConvertDirection(Vector3 direction, out float length)
	{
        direction.y = 0;

        length = direction.magnitude;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
		{
            return Vector3.right * Mathf.Sign(direction.x);
		}
        else
		{
            return Vector3.forward * Mathf.Sign(direction.z);
		}
	}

    private List<Vector3> SubdividePath(Vector3[] corners)
	{
        List<Vector3> path = new List<Vector3>();

        // Mathf.Sign returns shit
        int Sign(float number)
		{
            return number == 0 ? 0 : (number > 1 ? 1 : 0);
		}

		for (int i = 0; i < corners.Length - 1; i++)
		{
            Vector3 a = corners[i];
            Vector3 b = corners[i + 1];

            m_Grid.WorldToGrid(a, out Vector2Int start);
            m_Grid.WorldToGrid(b, out Vector2Int end);

            Vector2Int dir = end - start;
            dir.x = Sign(dir.x);
            dir.y = Sign(dir.y);

            Vector2Int gridCurrent = start;

            int j = 0;
            while(gridCurrent != end)
			{
                Vector3 world = m_Grid.GridToWorldXZ(gridCurrent);
                path.Add(world);

                gridCurrent += dir;

                if (j++ == 42)
				{
                    break;
				}
			}

            Vector3 w = m_Grid.GridToWorldXZ(gridCurrent);
            path.Add(w);
        }

        return path;
	}
}
