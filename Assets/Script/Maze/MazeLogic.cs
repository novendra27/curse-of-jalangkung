using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class MapLocation
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public override string ToString()
    {
        return $"({x}, {z})";

    }
}

public class MazeLogic : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public int scale = 1;
    public List<GameObject> Cube;
    public byte[,] map;
    public GameObject Character;
    public List<GameObject> EnemiesPrefabs;  // List of enemy prefabs to randomly select from
    public int EnemyCount = 3;
    public int RoomCount = 3;
    public int RoomMinSize = 6;
    public int RoomMaxSize = 10;
    public GameObject Totem;  // GameObject to represent the totem
    public NavMeshSurface surface;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMap();
        GenerateMaps();
        AddRooms(RoomCount, RoomMinSize, RoomMaxSize);
        DrawMaps();
        surface.BuildNavMesh();
        PlaceCharacter();
        PlaceEnemies();  // Now using PlaceEnemies to place random enemies
        PlaceTotem();  // Place the totem
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;  // Initializing with walls
                Debug.Log("Setting map[" + x + ", " + z + "] = " + map[x, z]);
            }
        }
    }

    public virtual void GenerateMaps()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                {
                    map[x, z] = 0;  // Empty space
                }
                Debug.Log("Setting map[" + x + ", " + z + "] = " + map[x , z ]);
            }
        }
    }

    void DrawMaps()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)  // If it's a wall
                {
                    Vector3 position = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Cube[Random.Range(0, Cube.Count)], position, Quaternion.identity);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    public virtual void PlaceCharacter()
    {
        bool PlayerSet = false;
        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int x = Random.Range(0, width);
                int z = Random.Range(0, depth);
                if (map[x, z] == 0 && !PlayerSet)
                {
                    Debug.Log("Placing player at " + x + ", " + z);
                    PlayerSet = true;
                    Character.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                else if (PlayerSet)
                {
                    Debug.Log("Player already set");
                    return;
                }
            }
        }
    }

    // Modified to place a random enemy at each random position
    public virtual void PlaceEnemies()
    {
        int EnemySet = 0;
        while (EnemySet < EnemyCount)
        {
            // Randomly pick a position within the map boundaries
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            // Check if the space is empty (map[x, z] == 0)
            if (map[x, z] == 0)  // Only place enemy on empty spaces (marked with 0)
            {
                bool positionOccupied = false;

                // Check if any existing enemy is already at the location
                foreach (var enemy in EnemiesPrefabs)
                {
                    if (enemy.transform.position.x == x * scale && enemy.transform.position.z == z * scale)
                    {
                        positionOccupied = true;
                        break;
                    }
                }

                // If the position is not occupied, place the enemy
                if (!positionOccupied)
                {
                    // Randomly select an enemy prefab from the list
                    GameObject randomEnemy = EnemiesPrefabs[Random.Range(0, EnemiesPrefabs.Count)];

                    Debug.Log("Placing enemy at " + x + ", " + z);
                    EnemySet++;
                    Instantiate(randomEnemy, new Vector3(x * scale, 0, z * scale), Quaternion.identity);
                }
            }
        }
    }

    // Method to place a Totem at a random location
    public virtual void PlaceTotem()
    {
        int x = Random.Range(0, width);
        int z = Random.Range(0, depth);
        while (map[x, z] != 0)  // Ensure it's placed on an empty space
        {
            x = Random.Range(0, width);
            z = Random.Range(0, depth);
        }

        Debug.Log("Placing totem at " + x + ", " + z);
        Instantiate(Totem, new Vector3(x * scale, 0, z * scale), Quaternion.identity);
    }

    public int CountSquareNeighbors(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        return count;
    }

    public virtual void AddRooms(int count, int minSize, int maxSize)
    {
        for (int c = 0; c < count; c++)
        {
            int startX = Random.Range(3, width - 3);
            int startZ = Random.Range(3, depth - 3);
            int roomWidth = Random.Range(minSize, maxSize);
            int roomDepth = Random.Range(minSize, maxSize);
            for (int x = startX; x < width - 3 && x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < depth - 3 && z < startZ + roomDepth; z++)
                {
                    map[x, z] = 2;  // Marking the room space as empty
                }
            }
        }
    }
}

