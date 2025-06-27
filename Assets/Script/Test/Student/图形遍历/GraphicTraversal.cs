using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class GraphicTraversal : MonoBehaviour
{
    public int[,] grid = new int[6, 5]{
            {0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0},
            {0, 1, 0, 1, 0},
            {0, 1, 0, 1, 0},
            {0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0}
        };
    private bool[,] visited;
    void Start()
    {
        visited = new bool[grid.GetLength(0), grid.GetLength(1)];
    }
    [Button("计算抵达路径")]
    private void CalculatePathKenth()
    {

    }

    private void Run(int startX, int startY, int endX, int endY, int[,] grid, ref int minStepNumber)
    {
        if (startX == endX && startY == endY)
        {
            Debug.Log($"Task completion, Min step number: {minStepNumber}");
            return;
        }

        //迭代计算
        visited[startX, startY] = true;
        minStepNumber++;
        if ((startX - 1) >= 0 && visited[startX - 1, startY] == false && grid[startX - 1, startY] == 0)
        {
            Run(startX - 1, startY, endX, endY, grid, ref minStepNumber);
        }
        if ((startX + 1) <= (grid.GetLength(0) - 1) && visited[startX + 1, startY] == false && grid[startX + 1, startY] == 0)
        {
            Run(startX + 1, startY, endX, endY, grid, ref minStepNumber);
        }
        if ((startY - 1) >= 0 && visited[startX, startY - 1] == false && grid[startX, startY - 1] == 0)
        {
            Run(startX, startY - 1, endX, endY, grid, ref minStepNumber);
        }
        if ((startY + 1) <= (grid.GetLength(1) - 1) && visited[startX, startY + 1] == false && grid[startX, startY + 1] == 0)
        {
            Run(startX, startY + 1, endX, endY, grid, ref minStepNumber);
        }
        visited[startX, startY] = false;

    }
}
