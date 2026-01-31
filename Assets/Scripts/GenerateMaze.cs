using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField]
    GameObject roomPrefab;

    // The Grid
    Room[,] rooms = null;

    [SerializeField]
    int numX = 10;
    int numY = 10;

    // The Room width and height
    float roomWidth;
    float roomHeight;

    // The stack for backtracking
    Stack<Room> stack = new Stack<Room>();

    bool generating = false;

    private void GetRoomSize() 
    {
        SpriteRenderer[] spriteRenderers = 
            roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 mmaxBounds = Vector3.negativeInfinity;

        foreach(SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(
                minBounds,
                ren.bounds.min);

            mmaxBounds = Vector3.Max(
                mmaxBounds,
                ren.bounds.max);
        }

        roomWidth = mmaxBounds.x - minBounds.x;
        roomHeight = mmaxBounds.y - minBounds.y;
    }

    private void SetCamera()
    {
        Camera.main.transform.position = new Vector3(
            numX * (roomWidth - 1) / 2,
            numY * (roomHeight - 1) / 2,
            -100.0f);

        float min_value = Mathf.Min(numY * (roomWidth - 1), numY * (roomHeight - 1));
        Camera.main.orthographicSize = min_value * 0.75f;
    }

    private void Start()
    {
        GetRoomSize();

        rooms = new Room[numX, numY];

        for(int i = 0; i < numX; i++)
        {
            for(int j = 0; j< numY; j++)
            {
                GameObject room = Instantiate(roomPrefab,
                    new Vector3(i * roomWidth, j * roomHeight, 0.0f),
                    Quaternion.identity);

                room.name = "Room_" + i.ToString() + "_" + j.ToString();
                rooms[i, j] = room.GetComponent<Room>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();
    }


}
