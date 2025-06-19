using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    private bool[,] visited;
    private int pathcount=0;
    private int shortestPathLength = int.MaxValue;
    public PathTest()
    {
        visited = new bool[graph.GetLength(0), graph.GetLength(1)];
    }
    public int[,] graph = new int[5, 4]
    {
        {0, 0, 0, 0},
        {0, 0, 1, 0},
        {0, 1, 0, 0},
        {0, 1, 0, 0},
        {0, 0, 0, 0}
    };
    void Start()
    {
        FindPath(0, 0, 2, 2, 0);
        Debug.Log("Path count: " + pathcount);
        Debug.Log("Shortest path length: " + shortestPathLength);
    }

    void FindPath(int nowX, int nowY, int endX, int endY, int currentPathLength)
    {
        if (nowX == endX && nowY == endY)
        {
            pathcount++;
            if (currentPathLength < shortestPathLength)
            {
                shortestPathLength = currentPathLength;
            }
            return;
        }
        visited[nowX, nowY] = true;
        currentPathLength++;
        if (nowX > 0 && graph[nowX - 1, nowY] == 0 && visited[nowX - 1, nowY] == false)
        {
            FindPath(nowX - 1, nowY, endX, endY, currentPathLength);
        }
        if (nowX < 4 && graph[nowX + 1, nowY] == 0 && visited[nowX + 1, nowY] == false)
        {
            FindPath(nowX + 1, nowY, endX, endY, currentPathLength);
        }
        if (nowY > 0 && graph[nowX, nowY - 1] == 0 && visited[nowX, nowY - 1] == false)
        {
            FindPath(nowX, nowY - 1, endX, endY, currentPathLength);
        }
        if (nowY < 3 && graph[nowX, nowY + 1] == 0 && visited[nowX, nowY + 1] == false)
        {
            FindPath(nowX, nowY + 1, endX, endY, currentPathLength);
        }
        visited[nowX, nowY] = false;
    }
}
