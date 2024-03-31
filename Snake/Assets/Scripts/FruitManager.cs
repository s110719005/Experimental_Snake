using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    [SerializeField]
    private int maxGenerateAmount = 3;
    private GridSystem.Grid grid;
    private List<GridObject> fruits;
    // Start is called before the first frame update
    void Start()
    {
        grid = GridManager.Instance.CurrentGrid;
        fruits = new List<GridObject>();
        GenerateFruit();
    }

    private void GenerateFruit()
    {
        int random = UnityEngine.Random.Range(1, maxGenerateAmount);
        for(int i = 0; i < random; i++)
        {
            GridObject fruitGridObject = GridManager.Instance.GetRandomAvailableGrid();
            GridManager.Instance.SetGridColor(fruitGridObject, Color.red);
            GridManager.Instance.SetGridBoolValue(fruitGridObject, true);
            fruits.Add(fruitGridObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsContainGird(GridObject gridObject)
    {
        return fruits.Contains(gridObject);
    }

    internal void RemoveFruit(GridObject gridObject)
    {
        fruits.Remove(gridObject);
        //GridManager.Instance.SetGridColor(gridObject, Color.white);
        //GridManager.Instance.SetGridBoolValue(gridObject, false);
        if(fruits.Count < 3)
        {
            GenerateFruit();
        }
    }
}
