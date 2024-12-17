using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{

    public WallObject WallPrefab;
    public ExitCellObject ExitCellPrefab;

    public class CellData
    {
        public bool passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;

    private Tilemap m_Tilemap;

    public int Height;
    public int Width;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    private Grid m_Grid;
    public PlayerController playerCont;

    public FoodObject FoodPrefab;
    public BigFoodObject BigFoodPrefab;
    public EnemyObject EnemyPrefab;

    private List<Vector2Int> m_EmptyCells;


    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();

        m_Grid = GetComponentInChildren<Grid>();

        m_BoardData = new CellData[Height, Width];
        m_EmptyCells = new List<Vector2Int>();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].passable = true;

                    m_EmptyCells.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }

        }
        m_EmptyCells.Remove(new Vector2Int(1, 1));
        Vector2Int endCoord = new Vector2Int (Height - 2, Width - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCells.Remove(endCoord);

        GenerateWall();

        GenerateEnemies();

        GenerateFood();

    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateFood()
    {
        int foodCount = 3;

        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];
            m_EmptyCells.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            FoodObject newFood = Instantiate(FoodPrefab);
            AddObject(newFood, coord);

        }
        int bigfoodCount = 2;

        for (int i = 0; i < bigfoodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];
            m_EmptyCells.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            BigFoodObject newBigFood = Instantiate(BigFoodPrefab);
            AddObject(newBigFood, coord);

        }
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(5,10);

        for (int i = 0; i < wallCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];
            m_EmptyCells.RemoveAt(randomIndex);
            CellData data = m_BoardData[coord.x, coord.y];
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);

        }
    }

    void GenerateEnemies()
    {
        int enemiesCount = Random.Range(2, 5);

        for (int i = 0; i < enemiesCount;i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];
            m_EmptyCells.RemoveAt(randomIndex);
            EnemyObject newEnemy = Instantiate(EnemyPrefab);
            AddObject(newEnemy, coord);
        }
       
    }

    void AddObject (CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void DeleteMap()
    {
        if (m_BoardData == null) return;
        
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}