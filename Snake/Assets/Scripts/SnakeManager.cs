using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GridSystem;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private int snakeLength;
    [SerializeField]
    private FruitManager fruitManager;
    private GridObject snakeHead;
    private GridObject snakeTail;
    private GridObject previousTail;
    //[SerializeField]
    //private Vector2 snakeGeneratePosition;
    private GridSystem.Grid grid;
    private List<Vector2> snakeList;
    public List<GridObject> snake;
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
        previousTail = null;
        //reset color
        for(int i = 0; i < snake.Count; i++)
        {
            Color color = new Color(1, 1, 1);
            snake[i].SetColor(color);
            GridManager.Instance.SetGridBoolValue(snake[i], false);
        }
        snakeList.Clear();
        snake.Clear();
    }

    private void MoveSnake(int x, int y)
    {
        StuckCheck();
        if(snakeHead.X + x < 0 || snakeHead.Y + y < 0 || snakeHead.X + x >= grid.Width || snakeHead.Y + y >= grid.Height) 
        {
            Debug.Log("Can't move");
            return;
        }
        GridObject newGridObject = grid.GridObjects[snakeHead.X + x, snakeHead.Y + y];
        if(snake.Contains(newGridObject))
        {
            Debug.Log("Can't move");
            return;
        }
        else
        {
            for(int i = snake.Count - 1; i >= 0; i--)
            {
                Color color = new Color(1f - (float)(i + 1) / snake.Count, 1f, 0.5f);
                if(snakeTail == snake[i])
                {
                    snake[i].SetColor(Color.white);
                    GridManager.Instance.SetGridBoolValue(snake[i], false);
                    previousTail = snake[i];
                    snake[i] = snake[i - 1];
                }
                else if(snakeHead == snake[i])
                {
                    snake[i] = newGridObject;
                    snake[i].SetColor(color);
                    GridManager.Instance.SetGridBoolValue(snake[i], true);
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
        FruitCollideCheck(newGridObject);
    }

    private void StuckCheck()
    {
        if(!IsAvailableAdjacent(snakeHead))
        {
            ResetSnake();
            fruitManager.ResetFruit();
            GenerateSnake();
            fruitManager.GenerateFruit();
        }
    }

    private void FruitCollideCheck(GridObject gridObject)
    {
        if(fruitManager == null) { return; }
        //if collide
        if(fruitManager.IsContainGird(gridObject))
        {
            Grow();
            fruitManager.RemoveFruit(gridObject);
        }
    }

    private void Grow()
    {
        if(previousTail != snakeTail)
        {
            GridManager.Instance.SetGridBoolValue(previousTail, true);
            snake.Add(previousTail);
            snakeLength ++;
            snakeTail = previousTail;
            for(int i = 0; i < snakeLength; i++)
            {
                Color color = new Color(1f - (float)(i + 1) / snake.Count, 1f, 0.5f);
                snake[i].SetColor(color);
            }
            if(snakeLength == grid.Width * grid.Height)
            {
                Debug.Log("END");
            }
        }
    }

    private void GenerateSnake()
    {   
        grid =  GridManager.Instance.CurrentGrid;
        snakeHead = GridManager.Instance.GetRandomAvailableGrid();
        snakeList = CheckValid(snakeHead.X, snakeHead.Y);
        for(int i = 1; i < snakeList.Count + 1; i++)
        {
            Color color = new Color(1f - (float)i / snakeList.Count, 1f, 0.5f);
            GridObject newGridObejct = grid.GridObjects[(int)snakeList[i - 1].x, (int)snakeList[i - 1].y];
            newGridObejct.SetColor(color);
            GridManager.Instance.SetGridBoolValue(newGridObejct, true);
            snake.Add(newGridObejct);
        }
        snakeTail = snake[snakeLength - 1];
        previousTail = snakeTail;
    }

    private List<Vector2> CheckValid(int x, int y, int count = 0)
    {
        count ++;
        Debug.Log("Count: " + count);
        if(count >= 100) 
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

    private bool IsAvailableAdjacent(GridObject gridObject)
    {
        List<Vector2> adjacentGrid = new List<Vector2>{ new Vector2(gridObject.X + 1, gridObject.Y), 
                                                        new Vector2(gridObject.X - 1, gridObject.Y), 
                                                        new Vector2(gridObject.X, gridObject.Y + 1), 
                                                        new Vector2(gridObject.X, gridObject.Y - 1)};
        for(int i = 0; i < adjacentGrid.Count; i++)
        {
            //check if in grid
            //if() { return true; } 
            //if() { return true; }  
            //check if used
            if(Mathf.Clamp(adjacentGrid[i].x, 0, grid.Width - 1) == adjacentGrid[i].x && Mathf.Clamp(adjacentGrid[i].y, 0, grid.Height - 1) == adjacentGrid[i].y) 
            {
                if(!snake.Contains(grid.GridObjects[(int)adjacentGrid[i].x, (int)adjacentGrid[i].y])) { return true; }
            }
        }
        return false;
    }
}
