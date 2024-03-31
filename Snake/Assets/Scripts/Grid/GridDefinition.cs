using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    [CreateAssetMenu(menuName = "GridSystem/GridDefinition")]
    public class GridDefinition : ScriptableObject
    {
        [SerializeField]
        private int gridWidth;
        public int GridWidth => gridWidth;
        [SerializeField]
        private int gridHeight;
        public int GridHeight => gridHeight;
        [SerializeField]
        private float cellSize;
        public float CellSize => cellSize;

        [SerializeField]
        private Sprite gridSprite;
        public Sprite GridSprite => gridSprite;
        [SerializeField]
        private Sprite templateSprite;
        public Sprite TemplateSprite => templateSprite;

        [SerializeField]
        private List<Color> usedColors;
        public List<Color> UsedColors => usedColors;

        [SerializeField]
        private List<GridColorData> gridColorDatas;
        public List<GridColorData> GridColorDatas => gridColorDatas;

        [System.Serializable]
        public class GridColorData
        {
            public int x;
            public int y;
            public Color color;
        }
        

        public void SetGridSpritesColor(int x, int y, Color newColor)
        {
            GridColorData newData = new GridColorData();
            newData.x = x;
            newData.y = y;
            newData.color = newColor;
            gridColorDatas.Add(newData);
        }

        public void ResetColorData()
        {
            usedColors.Clear();
            gridColorDatas.Clear();
        }

        public void Duplicate(Grid gridToDuplicate)
        {
            if(gridToDuplicate == null) { return; }
            gridWidth = gridToDuplicate.Width;
            gridHeight = gridToDuplicate.Height;
            cellSize = gridToDuplicate.CellSize;
            gridSprite = gridToDuplicate.GridSprite;
            usedColors = new List<Color>();
            gridColorDatas = new List<GridColorData>();
            for(int x = 0; x < gridToDuplicate.GridObjects.GetLength(0); x++)
            {
                for(int y = 0; y < gridToDuplicate.GridObjects.GetLength(1); y++)
                {
                    var duplicateColor = gridToDuplicate.GridObjects[x, y].gridSprite.color;
                    if(!usedColors.Contains(duplicateColor))
                    {
                        usedColors.Add(duplicateColor);
                    }
                    SetGridSpritesColor(x, y, duplicateColor);
                }
            }

        }

        internal void AddUsedColor(Color usedColor)
        {
            usedColors.Add(usedColor);
        }
    }
}
