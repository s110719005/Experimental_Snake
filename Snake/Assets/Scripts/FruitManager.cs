using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    [SerializeField]
    private int maxGenerateAmount = 3;
    [SerializeField]
    private SnakeManager snakeManager;
    [SerializeField]
    private AudioSource audioSource;
    private GridSystem.Grid grid;
    private List<GridObject> fruits;
    // Start is called before the first frame update
    void Start()
    {
        grid = GridManager.Instance.CurrentGrid;
        fruits = new List<GridObject>();
        //GenerateFruit();
    }

    public void GenerateFruit()
    {
        GridObject fruitGridObject = GridManager.Instance.GetRandomAvailableGrid();
        if(fruitGridObject != null)
        {
            Color tailColor = snakeManager.SnakeTail.GridSprite.color;
            float randomR = UnityEngine.Random.Range(-0.05f, 0.05f);
            float randomG = UnityEngine.Random.Range(-0.05f, 0.05f);
            float randomB = UnityEngine.Random.Range(-0.05f, 0.05f);
            Color color = new Color(randomR, randomG, randomB) + tailColor;
            GridManager.Instance.SetGridColor(fruitGridObject, color);
            GridManager.Instance.SetGridBoolValue(fruitGridObject, true);
            fruits.Add(fruitGridObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            GenerateFruit();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetFruit();
        }
    }

    public void ResetFruit()
    {
        foreach (var fruit in fruits)
        {
            GridManager.Instance.SetGridColor(fruit, new Color(1, 1, 1, 0));
            GridManager.Instance.SetGridBoolValue(fruit, false);
        }
        fruits.Clear();
    }

    public bool IsContainGird(GridObject gridObject)
    {
        if(fruits.Count < 1)
        {
            GenerateFruit();
        }
        return fruits.Contains(gridObject);
    }

    internal void RemoveFruit(GridObject gridObject)
    {
        fruits.Remove(gridObject);
        audioSource.Play();
        ResetFruit();
        int random = UnityEngine.Random.Range(2, maxGenerateAmount);
        for(int i = 0; i < random; i++)
        {
            GenerateFruit();
        }
    }
}
