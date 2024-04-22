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
    [SerializeField]
    private WaterinkEffect waterinkEffectManager;
    private GridObject snakeHead;
    private GridObject snakeTail;
    public GridObject SnakeTail => snakeTail;
    private GridObject previousTail;
    //[SerializeField]
    //private Vector2 snakeGeneratePosition;
    private GridSystem.Grid grid;
    private List<Vector2> snakeList;
    public List<GridObject> snake;
    void Start()
    {
        
        //GenerateSnake();
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.F))
        // {
        //     ResetSnake();
        //     GenerateSnake();
        //     fruitManager.ResetFruit();
        //     fruitManager.GenerateFruit();
        // }
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
            Color color = new Color(1, 1, 1, 0);
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
        GameCore.Instance.MenuCollisionCheck(newGridObject);
        if(newGridObject == snakeTail)
        {
            Debug.Log("TOUCH TAIL");
            GameCore.Instance.EndGame();
            return;
        }
        else if(snake.Contains(newGridObject))
        {
            Debug.Log("Can't move");
            return;
        }
        else
        {
            snake.Insert(0, newGridObject);
            snakeHead = newGridObject;
            GridManager.Instance.SetGridBoolValue(newGridObject, true);
            FruitCollideCheck(newGridObject);
            for(int i = 0; i < snake.Count - 1; i++)
            {
                GridManager.Instance.SetGridColor(snake[i], snake[i + 1].GridSprite.color);
            }
            GridManager.Instance.SetGridColor(snakeTail, new Color(1, 1, 1, 0));
            GridManager.Instance.SetGridBoolValue(snakeTail, false);
            previousTail = snakeTail;
            snake.Remove(snakeTail);
            snakeTail = snake[snakeLength - 1];
        }
    }

    private void StuckCheck()
    {
        if(!IsAvailableAdjacent(snakeHead))
        {
            //TODO: restart
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
            Grow(gridObject.GridSprite.color);
            fruitManager.RemoveFruit(gridObject);
        }
    }

    private void Grow(Color color)
    {
        if(previousTail != snakeTail)
        {
            GridManager.Instance.SetGridBoolValue(previousTail, true);
            GridManager.Instance.SetGridColor(previousTail, color);
            snake.Add(previousTail);
            snakeLength ++;
            snakeTail = previousTail;
            if(waterinkEffectManager != null)
            {
                Vector3 gridPosition = grid.GetWorldPosition(snakeHead.X, snakeHead.Y);
                Vector3 waterinkPosition = new Vector3(gridPosition.x + 20, -20, gridPosition.z + 20);
                waterinkEffectManager.PlayWaterinkEffect(waterinkPosition, color);
            }
            if(snakeLength == grid.Width * grid.Height)
            {
                Debug.Log("END");
                GameCore.Instance.EndGame();
            }
        }
    }

    public void GenerateSnake()
    {   
        snake = new List<GridObject>();
        grid =  GridManager.Instance.CurrentGrid;
        snakeHead = GridManager.Instance.GetRandomAvailableGrid();
        snakeList = CheckValid(snakeHead.X, snakeHead.Y);
        Color color;
        float randomR = UnityEngine.Random.Range(0.2f, 0.8f);
        float randomG = UnityEngine.Random.Range(0.2f, 0.8f);
        float randomB = UnityEngine.Random.Range(0.2f, 0.8f);
        color = new Color(randomR, randomG, randomB);
        for(int i = 1; i < snakeList.Count + 1; i++)
        {
            randomR = UnityEngine.Random.Range(-0.05f, 0.05f);
            randomG = UnityEngine.Random.Range(-0.05f, 0.05f);
            randomB = UnityEngine.Random.Range(-0.05f, 0.05f);
            color = new Color(randomR, randomG, randomB) + color;
            GridObject newGridObejct = grid.GridObjects[(int)snakeList[i - 1].x, (int)snakeList[i - 1].y];
            newGridObejct.SetColor(color);
            GridManager.Instance.SetGridBoolValue(newGridObejct, true);
            snake.Add(newGridObejct);
        }
        snakeTail = snake[snakeLength - 1];
        previousTail = snakeTail;
        StuckCheck();
    }

    private List<Vector2> CheckValid(int x, int y, int count = 0)
    {
        count ++;
        Debug.Log("Count: " + count);
        if(count >= 100) 
        {
            Debug.Log("FAILED");
            GameCore.Instance.EndGame();
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
            if(GridManager.Instance.CurrentGrid.GridObjects[(int)adjacentGrid[i].x, (int)adjacentGrid[i].y].BoolValue == true)
            {
                invalidGrid.Add(i); continue;
            }
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
            if(Mathf.Clamp(adjacentGrid[i].x, 0, grid.Width - 1) == adjacentGrid[i].x && Mathf.Clamp(adjacentGrid[i].y, 0, grid.Height - 1) == adjacentGrid[i].y) 
            {
                if(!snake.Contains(grid.GridObjects[(int)adjacentGrid[i].x, (int)adjacentGrid[i].y])) { return true; }
            }
        }
        return false;
    }
}
