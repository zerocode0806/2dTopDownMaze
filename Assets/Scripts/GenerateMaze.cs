using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject playerPrefab;


    Room[,] rooms;

    [SerializeField] int numX = 10;
    [SerializeField] int numY = 10;

    float roomWidth;
    float roomHeight;

    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    // ===============================
    // GET ROOM SIZE
    // ===============================
    private void GetRoomSize()
    {
        SpriteRenderer[] renderers =
            roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach (SpriteRenderer r in renderers)
        {
            min = Vector3.Min(min, r.bounds.min);
            max = Vector3.Max(max, r.bounds.max);
        }

        roomWidth = max.x - min.x;
        roomHeight = max.y - min.y;
    }

    // ===============================
    // SET CAMERA
    // ===============================
    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
            (numX - 1) * roomWidth / 2,
            (numY - 1) * roomHeight / 2,
            -10f
        );

        float sizeX = numX * roomWidth;
        float sizeY = numY * roomHeight;

        Camera.main.orthographicSize =
            Mathf.Max(sizeX / Camera.main.aspect, sizeY) * 0.6f;
    }

    // ===============================
    // START
    // ===============================
    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                GameObject roomObj = Instantiate(
                    roomPrefab,
                    new Vector3(x * roomWidth, y * roomHeight, 0),
                    Quaternion.identity
                );

                roomObj.name = $"Room_{x}_{y}";

                Room room = roomObj.GetComponent<Room>();
                room.Index = new Vector2Int(x, y);

                rooms[x, y] = room;
            }
        }

        SetCamera();
    }

    // ===============================
    // REMOVE WALL
    // ===============================
    private void RemoveRoomWall(int x, int y, Room.Directions dir)
    {
        if (dir == Room.Directions.NONE)
            return;

        rooms[x, y].SetDirFlag(dir, false);

        int nx = x;
        int ny = y;
        Room.Directions opposite = Room.Directions.NONE;

        switch (dir)
        {
            case Room.Directions.TOP:
                if (y < numY - 1)
                {
                    ny++;
                    opposite = Room.Directions.BOTTOM;
                }
                break;

            case Room.Directions.RIGHT:
                if (x < numX - 1)
                {
                    nx++;
                    opposite = Room.Directions.LEFT;
                }
                break;

            case Room.Directions.BOTTOM:
                if (y > 0)
                {
                    ny--;
                    opposite = Room.Directions.TOP;
                }
                break;

            case Room.Directions.LEFT:
                if (x > 0)
                {
                    nx--;
                    opposite = Room.Directions.RIGHT;
                }
                break;
        }

        if (opposite != Room.Directions.NONE)
        {
            rooms[nx, ny].SetDirFlag(opposite, false);
        }
    }

    // ===============================
    // GET UNVISITED NEIGHBOURS
    // ===============================
    public List<Tuple<Room.Directions, Room>> GetNeighboursNotVisited(int cx, int cy)
    {
        List<Tuple<Room.Directions, Room>> neighbours =
            new List<Tuple<Room.Directions, Room>>();

        foreach (Room.Directions dir in Enum.GetValues(typeof(Room.Directions)))
        {
            if (dir == Room.Directions.NONE)
                continue;

            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Room.Directions.TOP:
                    if (y < numY - 1)
                        y++;
                    else continue;
                    break;

                case Room.Directions.RIGHT:
                    if (x < numX - 1)
                        x++;
                    else continue;
                    break;

                case Room.Directions.BOTTOM:
                    if (y > 0)
                        y--;
                    else continue;
                    break;

                case Room.Directions.LEFT:
                    if (x > 0)
                        x--;
                    else continue;
                    break;
            }

            if (!rooms[x, y].visited)
            {
                neighbours.Add(
                    new Tuple<Room.Directions, Room>(dir, rooms[x, y]));
            }
        }

        return neighbours;
    }

    // ===============================
    // GENERATE STEP (DFS)
    // ===============================
    private bool GenerateStep()
    {
        if (stack.Count == 0)
            return true;

        Room current = stack.Peek();

        var neighbours =
            GetNeighboursNotVisited(current.Index.x, current.Index.y);

        if (neighbours.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, neighbours.Count);

            var item = neighbours[index];
            Room nextRoom = item.Item2;

            nextRoom.visited = true;

            RemoveRoomWall(current.Index.x, current.Index.y, item.Item1);

            stack.Push(nextRoom);
        }
        else
        {
            stack.Pop();
        }

        return false;
    }

    // ===============================
    // CREATE MAZE
    // ===============================
    public void CreateMaze()
    {
        if (generating) return;

        var oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null)
            Destroy(oldPlayer);

        ResetMaze();

        rooms[0, 0].visited = true;
        stack.Push(rooms[0, 0]);

        StartCoroutine(Coroutine_Generate());
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;

        bool finished = false;

        while (!finished)
        {
            finished = GenerateStep();
            yield return new WaitForSeconds(0.02f);
        }

        generating = false;

        SpawnPlayer(); // Spawn AFTER maze is complete
    }

    // ===============================
    // RESET
    // ===============================
    private void ResetMaze()
    {
        stack.Clear();

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                rooms[x, y].SetDirFlag(Room.Directions.TOP, true);
                rooms[x, y].SetDirFlag(Room.Directions.RIGHT, true);
                rooms[x, y].SetDirFlag(Room.Directions.BOTTOM, true);
                rooms[x, y].SetDirFlag(Room.Directions.LEFT, true);

                rooms[x, y].visited = false;
            }
        }
    }

    // ===============================
    // INPUT (NEW INPUT SYSTEM)
    // ===============================
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CreateMaze();
        }
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = rooms[0, 0].transform.position;

        // Slight offset to avoid overlapping wall pivot issues
        spawnPos += new Vector3(roomWidth * 0.25f, roomHeight * 0.25f, 0);

        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }
}
