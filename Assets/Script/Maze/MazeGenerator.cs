using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public int scale = 6;
    public List<GameObject> Cube;  // Dinding (wall) list
    public List<GameObject> FloorPrefabs;  // List objek lantai
    public GameObject RoofPrefabs;
    public byte[,] map;
    public GameObject Character;
    public List<GameObject> EnemiesPrefabs;  // List objek musuh
    public int EnemyCount = 3;
    public int RoomCount = 3;
    public int RoomMinSize = 3;
    public int RoomMaxSize = 5;
    public GameObject Totem;
    public int TotemCount = 5;
    public NavMeshSurface surface;
    public UIGameplayLogic uiGameplayLogic; // Referensi ke UIGameplayLogic

    private static System.Random rng = new System.Random();
    private List<Vector2> occupiedPositions = new List<Vector2>();

    void Start()
    {
        InitializeMap();
        GenerateMaze(5, 5);
        AddRooms(RoomCount, RoomMinSize, RoomMaxSize);
        DrawMaps();
        surface.BuildNavMesh();
        PlaceCharacter();
        PlaceEnemies();
        PlaceTotem();
        if (uiGameplayLogic != null)
        {
            uiGameplayLogic.SetTotalTotems(TotemCount);
        }
    }

    void Update() { }

    void InitializeMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;
                Debug.Log("Setting map[" + x + ", " + z + "] = " + map[x, z]);
            }
        }
    }

    void GenerateMaze(int x, int z)
    {
        if (CountSquareNeighbors(x, z) >= 2) return;
        map[x, z] = 0;

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
        float spawnOffset = -0.5f; // Offset for initial spawn position

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 basePosition = new Vector3(x * scale, 0f, z * scale);
                Vector3 spawnPosition = basePosition + new Vector3(0, spawnOffset, 0);

                if (map[x, z] == 1)
                {
                    GameObject wall = Instantiate(Cube[Random.Range(0, Cube.Count)], spawnPosition, Quaternion.identity);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    GameObject floor = Instantiate(FloorPrefabs[Random.Range(0, FloorPrefabs.Count)], spawnPosition, Quaternion.identity);
                    floor.transform.localScale = new Vector3(scale, scale, scale);

                    GameObject ceiling = Instantiate(RoofPrefabs, spawnPosition + new Vector3(0, scale, 0), Quaternion.identity);
                    ceiling.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    public void PlaceCharacter()
    {
        float spawnOffset = -0.5f;
        bool PlayerSet = false;
        while (!PlayerSet)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            if (map[x, z] == 0)
            {
                Vector3 basePosition = new Vector3(x * scale, 0f, z * scale);
                Vector3 spawnPosition = basePosition + new Vector3(0, spawnOffset, 0);

                Debug.Log("Placing player at " + x + ", " + z);
                PlayerSet = true;
                Character.transform.position = spawnPosition;
                occupiedPositions.Add(new Vector2(x, z));
            }
        }
    }

    bool IsValidSpawnPosition(int x, int z)
    {
        // Check if position is empty in map
        if (map[x, z] != 0) return false;

        // Check if position is already occupied
        if (occupiedPositions.Contains(new Vector2(x, z))) return false;

        // Check distance from player
        Vector2 playerPos = new Vector2(Character.transform.position.x / scale, Character.transform.position.z / scale);
        float distanceToPlayer = Vector2.Distance(new Vector2(x, z), playerPos);
        if (distanceToPlayer < 2) return false;

        return true;
    }

    public void PlaceEnemies()
    {
        float spawnOffset = -0.5f;
        int EnemySet = 0;
        int maxAttempts = 100000;
        int attempts = 0;

        while (EnemySet < EnemyCount && attempts < maxAttempts)
        {
            attempts++;
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);

            if (IsValidSpawnPosition(x, z))
            {
                Vector3 basePosition = new Vector3(x * scale, 0f, z * scale);
                Vector3 spawnPosition = basePosition + new Vector3(0, spawnOffset, 0);

                GameObject randomEnemy = EnemiesPrefabs[Random.Range(0, EnemiesPrefabs.Count)];
                Debug.Log("Placing enemy at " + x + ", " + z);
                Instantiate(randomEnemy, spawnPosition, Quaternion.identity);
                occupiedPositions.Add(new Vector2(x, z));
                EnemySet++;
            }
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not place all enemies within maximum attempts");
        }
    }


    bool IsLCorner(int x, int z)
    {
        // Pastikan posisi dalam batas map
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return false;

        // Periksa apakah ada tembok yang membentuk L
        // Pattern L bisa dalam 4 orientasi:
        //
        // Pattern 1:    Pattern 2:    Pattern 3:    Pattern 4:
        // 1 1 0        0 1 1         1 0 0         0 0 1
        // 1 0 0        0 0 1         1 1 0         0 1 1
        // 1 0 0        0 0 1         1 0 0         0 0 1

        // Check Pattern 1 (L di kiri atas)
        if (map[x - 1, z] == 1 && map[x - 1, z - 1] == 1 &&
            map[x, z - 1] == 1 && map[x, z] == 0)
            return true;

        // Check Pattern 2 (L di kanan atas)
        if (map[x + 1, z] == 1 && map[x + 1, z - 1] == 1 &&
            map[x, z - 1] == 1 && map[x, z] == 0)
            return true;

        // Check Pattern 3 (L di kiri bawah)
        if (map[x - 1, z] == 1 && map[x - 1, z + 1] == 1 &&
            map[x, z + 1] == 1 && map[x, z] == 0)
            return true;

        // Check Pattern 4 (L di kanan bawah)
        if (map[x + 1, z] == 1 && map[x + 1, z + 1] == 1 &&
            map[x, z + 1] == 1 && map[x, z] == 0)
            return true;

        return false;
    }

    bool IsNextToWall(int x, int z)
    {
        // Cek apakah ada tembok di sekitar posisi
        if (x > 0 && map[x - 1, z] == 1) return true;  // Tembok di kiri
        if (x < width - 1 && map[x + 1, z] == 1) return true;  // Tembok di kanan
        if (z > 0 && map[x, z - 1] == 1) return true;  // Tembok di atas
        if (z < depth - 1 && map[x, z + 1] == 1) return true;  // Tembok di bawah
        return false;
    }

    (Vector3, Quaternion) GetWallOffsetAndRotation(int x, int z)
    {
        float offsetAmount = 2f;
        Vector3 offset = Vector3.zero;
        float rotationY = 0f;
        bool isCorner = false;
        bool isUShape = false;

        // Check tembok di semua arah
        bool wallLeft = x > 0 && map[x - 1, z] == 1;
        bool wallRight = x < width - 1 && map[x + 1, z] == 1;
        bool wallUp = z > 0 && map[x, z - 1] == 1;
        bool wallDown = z < depth - 1 && map[x, z + 1] == 1;

        // Tentukan offset
        if (wallLeft) offset.x = -offsetAmount;
        if (wallRight) offset.x = offsetAmount;
        if (wallUp) offset.z = -offsetAmount;
        if (wallDown) offset.z = offsetAmount;

        // Cek bentuk U (3 tembok)
        if ((wallLeft && wallUp && wallRight) ||    // ฅ bentuk U atas
            (wallLeft && wallDown && wallRight) ||   // น bentuk U bawah
            (wallUp && wallRight && wallDown) ||     // โ bentuk U kanan
            (wallUp && wallLeft && wallDown))        // เ bentuk U kiri
        {
            isUShape = true;
        }

        if (isUShape)
        {
            // Tentukan arah hadap berdasarkan bukaan U
            if (wallLeft && wallUp && wallRight) rotationY = 180f;      // Menghadap bawah (bukaan U bawah)
            else if (wallLeft && wallDown && wallRight) rotationY = 0f;  // Menghadap atas (bukaan U atas)
            else if (wallUp && wallRight && wallDown) rotationY = -90f;  // Menghadap kiri (bukaan U kiri)
            else if (wallUp && wallLeft && wallDown) rotationY = 90f;    // Menghadap kanan (bukaan U kanan)
        }
        else
        {
            // Logika untuk sudut L
            if (wallLeft && wallUp)    // Sudut kiri atas
            {
                rotationY = 45f;
                isCorner = true;
            }
            else if (wallRight && wallUp)   // Sudut kanan atas
            {
                rotationY = -45f;
                isCorner = true;
            }
            else if (wallLeft && wallDown)  // Sudut kiri bawah
            {
                rotationY = 135f;
                isCorner = true;
            }
            else if (wallRight && wallDown) // Sudut kanan bawah
            {
                rotationY = -135f;
                isCorner = true;
            }

            // Jika bukan sudut L dan bukan bentuk U, menghadap ke tembok
            if (!isCorner)
            {
                if (wallLeft) rotationY = -90f;      // Menghadap kiri
                else if (wallRight) rotationY = 90f;  // Menghadap kanan
                else if (wallUp) rotationY = 180f;    // Menghadap atas
                else if (wallDown) rotationY = -180f; // Menghadap bawah
            }
        }

        return (offset, Quaternion.Euler(0, rotationY, 0));
    }

    public void PlaceTotem()
    {
        float spawnOffset = -3f;
        int TotemSet = 0;
        int maxAttempts = 100000;
        int attempts = 0;

        List<Vector2Int> potentialPositions = new List<Vector2Int>();

        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < depth - 1; z++)
            {
                if (map[x, z] == 0 && (IsLCorner(x, z) || IsNextToWall(x, z)))
                {
                    potentialPositions.Add(new Vector2Int(x, z));
                }
            }
        }

        System.Random rnd = new System.Random();
        potentialPositions = potentialPositions.OrderBy(x => rnd.Next()).ToList();

        foreach (var pos in potentialPositions)
        {
            if (TotemSet >= TotemCount || attempts >= maxAttempts) break;

            if (IsValidSpawnPosition(pos.x, pos.y))
            {
                var (wallOffset, rotation) = GetWallOffsetAndRotation(pos.x, pos.y);
                Vector3 spawnPos = new Vector3(pos.x * scale, spawnOffset, pos.y * scale) + wallOffset;

                Debug.Log($"Placing totem {(TotemSet + 1)} at {pos.x}, {pos.y} with rotation {rotation.eulerAngles}");
                Instantiate(Totem, spawnPos, rotation);
                occupiedPositions.Add(new Vector2(pos.x, pos.y));
                TotemSet++;
            }
            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not place all totems within maximum attempts");
        }
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