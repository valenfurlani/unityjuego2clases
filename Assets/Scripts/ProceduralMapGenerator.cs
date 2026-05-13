using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralMapGenerator : MonoBehaviour
{
    [Header("Configuración del Mapa")]
    public int width = 100;
    public int height = 100;
    public string seed;
    public bool useRandomSeed = true;

    [Range(0, 100)]
    public int randomFillPercent = 45;

    [Header("Ajustes de Autómatas")]
    public int smoothIterations = 5;

    [Header("Referencias Unity")]
    public Tilemap tilemap;
    public TileBase ruleTile; // Aquí arrastras tu Rule Tile

    private int[,] map;

    // Esto permite ejecutarlo desde el inspector haciendo click derecho en el componente
    [ContextMenu("Generar Mapa")]
    public void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        DrawMap();
    }

    [ContextMenu("Limpiar Mapa")]
    public void ClearMap()
    {
        if (tilemap != null) tilemap.ClearAllTiles();
    }

    void RandomFillMap()
    {
        if (useRandomSeed) seed = Time.time.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Los bordes siempre son paredes para cerrar el mapa
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        int[,] nextMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallCount = GetSurroundingWallCount(x, y);

                if (neighbourWallCount > 4)
                    nextMap[x, y] = 1;
                else if (neighbourWallCount < 4)
                    nextMap[x, y] = 0;
                else
                    nextMap[x, y] = map[x, y];
            }
        }
        map = nextMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++; // Los bordes externos cuentan como pared
                }
            }
        }
        return wallCount;
    }

    void DrawMap()
    {
        if (tilemap == null || ruleTile == null) return;
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    // Colocamos el Tile en la posición centrada
                    tilemap.SetTile(new Vector3Int(x - width / 2, y - height / 2, 0), ruleTile);
                }
            }
        }
    }
}