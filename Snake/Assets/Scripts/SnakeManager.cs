using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GridSystem;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private GridGenerator gridGenerator;
    [SerializeField]
    private int snakeLength;
    //TODO: why we need this??????
    public class SnakeGrid
    {
        private GridObject currentGridObject;
        public GridObject CurrentGridObject => currentGridObject;
        private GridObject previousGridObject;
        public GridObject PreviousGridObject => previousGridObject;
        private GridObject nextGridObject;
        public GridObject NextGridObject => nextGridObject;

        public void SetCurrentGridObject(GridObject gridObject)
        {
            currentGridObject = gridObject;
            gridObject.SetBoolValue(true);
        }
        public void SetPreviousGridObject(GridObject gridObject)
        {
            previousGridObject = gridObject;
        }
        public void SetNextGridObject(GridObject gridObject)
        {
            nextGridObject = gridObject;
        }
    }
    private GridObject snakeHead;
    private GridObject snakeTail;
    //[SerializeField]
    //private Vector2 snakeGeneratePosition;
    private GridSystem.Grid grid;
    private List<Vector2> debugList;
    void Start()
    {
        GenerateSnake();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            snakeHead = null;
            for(int i = 0; i < debugList.Count; i++)
            {
                Color color = new Color(1, 1, 1);
                grid.SetSpriteColor((int)debugList[i].x, (int)debugList[i].y, color);
            }
            debugList.Clear();
            GenerateSnake();
        }
    }

    private void GenerateSnake()
    {   
        grid =  gridGenerator.CurrentGrid;
        int randomX = UnityEngine.Random.Range(0, grid.Width);
        int randomY = UnityEngine.Random.Range(0, grid.Height);
        snakeHead = grid.GridObjects[randomX, randomY];
        debugList = CheckValid(randomX, randomY);
        for(int i = 1; i < debugList.Count + 1; i++)
        {
            Color color = new Color((float)i / debugList.Count, (float)i / debugList.Count, (float)i / debugList.Count);
            grid.SetSpriteColor((int)debugList[i - 1].x, (int)debugList[i - 1].y, color);
        }
        
    }

    private List<Vector2> CheckValid(int x, int y, int count = 0)
    {
        count ++;
        Debug.Log("Count: " + count);
        if(count >= 10) 
        {
            Debug.Log("FAILED");
            return null;
        }
        List<Vector2> usedGrid = new List<Vector2>();
        usedGrid.Add(new Vector2(x, y));
        for(int i = 0; i < snakeLength - 1; i++)
        {
            Debug.Log("Used count:" + usedGrid.Count);
            Vector2 newPosition = GetRandomAvaliableAdjacent(usedGrid, grid.Width, grid.Height, (int)usedGrid[i].x, (int)usedGrid[i].y);
            if(newPosition.x == -1 && newPosition.y == -1)
            {
                return CheckValid(x, y, count);
            }
            else
            {
                Debug.Log("newPosition: " + newPosition);
                usedGrid.Add(newPosition);
            }

        }
        return usedGrid;
    }

    private Vector2 GetRandomAvaliableAdjacent(List<Vector2> usedGrid, int gridWidth, int gridHeight, int x, int y)
    {
        int random = UnityEngine.Random.Range(1, 5);
        List<Vector2> adjacentGrid = new List<Vector2>{new Vector2(x + 1, y), new Vector2(x - 1, y), new Vector2(x, y +1), new Vector2(x, y - 1)};
        List<int> invalidGrid = new List<int>();
        for(int i = 0; i < adjacentGrid.Count; i++)
        {
            //check if in grid
            if(Mathf.Clamp(adjacentGrid[i].x, 0, gridWidth - 1) != adjacentGrid[i].x) { invalidGrid.Add(i); continue; } 
            if(Mathf.Clamp(adjacentGrid[i].y, 0, gridHeight - 1) != adjacentGrid[i].y) { invalidGrid.Add(i); continue; }  
            //check if used
            if(usedGrid.Contains(adjacentGrid[i])) { invalidGrid.Add(i); continue; }
        }
        for(int j = invalidGrid.Count - 1; j >= 0; j--)
        {
            adjacentGrid.RemoveAt(invalidGrid[j]);
        }
        if(adjacentGrid.Count < 1) { Debug.Log("No adjacent"); return new Vector2(-1, -1); }
        else
        {
            random = UnityEngine.Random.Range(0, adjacentGrid.Count);
            return adjacentGrid[random];
        }
    }
}
