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
    private List<Vector2> snakeList;
    private List<GridObject> snake;
    void Start()
    {
        snake = new List<GridObject>();
        GenerateSnake();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            ResetSnake();
            GenerateSnake();
        }
        if(Input.GetKeyDown(KeyCode.W)) { MoveSnake( 0,  1); }
        if(Input.GetKeyDown(KeyCode.A)) { MoveSnake(-1,  0); }
        if(Input.GetKeyDown(KeyCode.S)) { MoveSnake( 0, -1); }
        if(Input.GetKeyDown(KeyCode.D)) { MoveSnake( 1,  0); }
    }

    private void ResetSnake()
    {
        snakeHead = null;
        snakeTail = null;
        //reset color
        for(int i = 0; i < snakeList.Count; i++)
        {
            Color color = new Color(1, 1, 1);
            grid.SetSpriteColor((int)snakeList[i].x, (int)snakeList[i].y, color);
        }
        snakeList.Clear();
        snake.Clear();
    }

    private void MoveSnake(int x, int y)
    {
        GridObject newGridObject = grid.GridObjects[snakeHead.x + x, snakeHead.y + y];
        if(snake.Contains(newGridObject))
        {
            Debug.Log("Can't move");
        }
        else
        {
            for(int i = snake.Count - 1; i >= 0; i--)
            {
                Color color = new Color(1f - (float)(i + 1) / snake.Count, 1f, 0.5f);
                GridObject tempGridObject = snake[i];
                if(snakeTail == snake[i])
                {
                    snake[i].SetColor(Color.white);
                    snake[i].SetBoolValue(false);
                    snake[i] = snake[i - 1];
                }
                else if(snakeHead == snake[i])
                {
                    snake[i] = newGridObject;
                    snake[i].SetColor(color);
                    snake[i].SetBoolValue(true);
                }
                else
                {
                    snake[i].SetColor(color);
                    snake[i] = snake[i - 1];
                }
                
            }
            snakeHead = newGridObject;
            snakeTail = snake[snakeLength - 1];
        }
    }

    private void GenerateSnake()
    {   
        grid =  gridGenerator.CurrentGrid;
        int randomX = UnityEngine.Random.Range(0, grid.Width);
        int randomY = UnityEngine.Random.Range(0, grid.Height);
        snakeHead = grid.GridObjects[randomX, randomY];
        snakeList = CheckValid(randomX, randomY);
        for(int i = 1; i < snakeList.Count + 1; i++)
        {
            Color color = new Color(1f - (float)i / snakeList.Count, 1f, 0.5f);
            grid.SetSpriteColor((int)snakeList[i - 1].x, (int)snakeList[i - 1].y, color);
            grid.SetCorrect((int)snakeList[i - 1].x, (int)snakeList[i - 1].y, true);
            snake.Add(grid.GridObjects[(int)snakeList[i - 1].x, (int)snakeList[i - 1].y]);
        }
        snakeTail = grid.GridObjects[(int)snakeList[snakeLength - 1].x, (int)snakeList[snakeLength - 1].y];
        
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
            Vector2 newPosition = GetRandomAvaliableAdjacent(usedGrid, grid.Width, grid.Height, (int)usedGrid[i].x, (int)usedGrid[i].y);
            if(newPosition.x == -1 && newPosition.y == -1)
            {
                return CheckValid(x, y, count);
            }
            else
            {
                //Debug.Log("newPosition: " + newPosition);
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
