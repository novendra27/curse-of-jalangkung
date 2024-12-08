using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public int scale = 1;
    public List<GameObject> Cube;  // Dinding (wall) list
    public List<GameObject> FloorPrefabs;  // List objek lantai
    public GameObject RoofPrefabs;
    public byte[,] map;
    public GameObject Character;
    public List<GameObject> EnemiesPrefabs;  // List objek musuh
    public int EnemyCount = 3;
    public int RoomCount = 3;
    public int RoomMinSize = 3;  // Reduced room minimum size
    public int RoomMaxSize = 5;  // Reduced room maximum size
    public GameObject Totem;  // GameObject totem
    public NavMeshSurface surface;

    private static System.Random rng = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        InitializeMap();
        GenerateMaze(5, 5);  // Start generating the maze with DFS
        AddRooms(RoomCount, RoomMinSize, RoomMaxSize);
        DrawMaps();
        surface.BuildNavMesh();  // Build NavMesh sebelum penempatan karakter, musuh, dan totem
        PlaceCharacter();
        PlaceEnemies();  // Menempatkan musuh
        PlaceTotem();  // Menempatkan totem
    }

    // Update is called once per frame
    void Update() { }

    void InitializeMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;  // Mengatur semua lokasi awal sebagai dinding
                Debug.Log("Setting map[" + x + ", " + z + "] = " + map[x, z]);
            }
        }
    }

    void GenerateMaze(int x, int z)
    {
        if (CountSquareNeighbors(x, z) >= 2) return;
        map[x, z] = 0;  // Mark the cell as part of the maze (empty space)

        var directions = new List<MapLocation>
        {
            new MapLocation(1, 0),
            new MapLocation(0, 1),
            new MapLocation(-1, 0),
            new MapLocation(0, -1)
        };

        Shuffle(directions);

        GenerateMaze(x + directions[0].x, z + directions[0].z);
        GenerateMaze(x + directions[1].x, z + directions[1].z);
        GenerateMaze(x + directions[2].x, z + directions[2].z);
        GenerateMaze(x + directions[3].x, z + directions[3].z);
    }

    // Shuffle function for randomizing directions
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void DrawMaps()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)  // Jika lokasi adalah dinding
                {
                    Vector3 position = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Cube[Random.Range(0, Cube.Count)], position, Quaternion.identity);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    Vector3 position = new Vector3(x * scale, 0, z * scale);
                    GameObject floor = Instantiate(FloorPrefabs[Random.Range(0, FloorPrefabs.Count)], position, Quaternion.identity);
                    floor.transform.localScale = new Vector3(scale, scale, scale);

                    // Menambahkan atap yang sama dengan posisi lantai
                    GameObject ceiling = Instantiate(RoofPrefabs, new Vector3(x * scale, scale, z * scale), Quaternion.identity);
                    ceiling.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    public void PlaceCharacter()
    {
        bool PlayerSet = false;
        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int x = Random.Range(0, width);
                int z = Random.Range(0, depth);

                // Pastikan karakter ditempatkan di lokasi kosong dan dalam radius 3x3 tidak ada musuh
                if (map[x, z] == 0 && !PlayerSet && IsEmptyAreaForCharacter(x, z))
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

    bool IsEmptyAreaForCharacter(int x, int z)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int checkX = x + i;
                int checkZ = z + j;

                // Pastikan tidak keluar dari batas
                if (checkX >= 0 && checkX < width && checkZ >= 0 && checkZ < depth)
                {
                    // Periksa jika ada musuh di area tersebut
                    if (map[checkX, checkZ] == 2)  // 2 untuk musuh
                    {
                        return false;
                    }
                }
            }
        }
        return true;  // Area 3x3 kosong dari musuh
    }

    public void PlaceEnemies()
    {
        int EnemySet = 0;
        while (EnemySet < EnemyCount)
        {
            // Pilih posisi acak dalam batas peta
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            // Periksa jika lokasi kosong (map[x, z] == 0)
            if (map[x, z] == 0)  // Hanya menempatkan musuh di ruang kosong
            {
                bool positionOccupied = false;

                // Periksa jika posisi sudah ditempati oleh musuh
                foreach (var enemy in EnemiesPrefabs)
                {
                    if (enemy.transform.position.x == x * scale && enemy.transform.position.z == z * scale)
                    {
                        positionOccupied = true;
                        break;
                    }
                }

                // Jika posisi tidak ditempati, tempatkan musuh
                if (!positionOccupied)
                {
                    // Pilih musuh secara acak dari list
                    GameObject randomEnemy = EnemiesPrefabs[Random.Range(0, EnemiesPrefabs.Count)];

                    Debug.Log("Placing enemy at " + x + ", " + z);
                    EnemySet++;
                    Instantiate(randomEnemy, new Vector3(x * scale, 0, z * scale), Quaternion.identity);
                }
            }
        }
    }

    public void PlaceTotem()
    {
        int x = Random.Range(0, width);
        int z = Random.Range(0, depth);
        while (map[x, z] != 0)  // Pastikan totem ditempatkan di ruang kosong
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

    public void AddRooms(int count, int minSize, int maxSize)
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
                    map[x, z] = 2;  // Menandakan ruang sebagai kosong
                }
            }
        }
    }
}

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
