using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoissonDiskSpawner : MonoBehaviour
{
    [Header("Area Settings")]
    public float width = 20f;
    public float height = 20f;

    [Header("Poisson Settings")]
    public float radius = 2f;
    public int rejectionSamples = 30;

    [Header("Grass Prefab")]
    public GameObject prefab;
    public Vector2 spriteScale = Vector2.one;

    private List<Vector2> points;

    void Start()
    {
        Regenerate();
    }

    public void Regenerate()
    {
        ClearSpawned();
        points = GeneratePoints(radius, new Vector2(width, height), rejectionSamples);
        SpawnGrass();
    }

    void ClearSpawned()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void SpawnGrass()
    {
        foreach (Vector2 point in points)
        {
            Vector3 position = new Vector3(point.x - width / 2f, 0f, point.y - height / 2f);
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f); // Lay flat on XZ plane
            GameObject instance = Instantiate(prefab, position, rotation, transform);
            instance.transform.localScale = new Vector3(spriteScale.x, spriteScale.y, 1f);
        }
    }

    List<Vector2> GeneratePoints(float radius, Vector2 regionSize, int numSamplesBeforeRejection)
    {
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(regionSize / 2);

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float dist = Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + dir * dist;

                if (IsValid(candidate, regionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    bool IsValid(Vector2 candidate, Vector2 regionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x < 0 || candidate.y < 0 || candidate.x >= regionSize.x || candidate.y >= regionSize.y)
            return false;

        int cellX = (int)(candidate.x / cellSize);
        int cellY = (int)(candidate.y / cellSize);

        int searchRadius = 2;
        for (int x = Mathf.Max(0, cellX - searchRadius); x <= Mathf.Min(cellX + searchRadius, grid.GetLength(0) - 1); x++)
        {
            for (int y = Mathf.Max(0, cellY - searchRadius); y <= Mathf.Min(cellY + searchRadius, grid.GetLength(1) - 1); y++)
            {
                int index = grid[x, y] - 1;
                if (index != -1)
                {
                    float sqrDist = (candidate - points[index]).sqrMagnitude;
                    if (sqrDist < radius * radius)
                        return false;
                }
            }
        }

        return true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PoissonDiskSpawner))]
public class PoissonDiskSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PoissonDiskSpawner spawner = (PoissonDiskSpawner)target;
        if (GUILayout.Button("Respawn Grass"))
        {
            spawner.Regenerate();
        }
    }
}
#endif
