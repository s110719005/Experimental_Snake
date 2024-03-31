using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    private GridSystem.Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = GridManager.Instance.CurrentGrid;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
