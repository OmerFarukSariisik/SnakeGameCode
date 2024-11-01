using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Snake/Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int goal;
        public int rows;
        public int columns;
        public List<Cell> grid;

        // Enum for scriptable object.
        public enum CellItem
        {
            None = 0,
            Apple = 1,
            Wall = 2,
            Portal = 3,
            Dizzy = 4,
            SnakeHead = 10
        }

        [System.Serializable]
        public class Cell
        {
            public CellItem item = CellItem.None;
        }

        public void InitializeGrid()
        {
            var cellCount = rows * columns;
            if (grid != null && grid.Count == cellCount) return;
            
            grid = new List<Cell>(cellCount);
            for (var i = 0; i < cellCount; i++)
                grid.Add(new Cell());
        }
    }

    public class GridData
    {
        public Vector3 Position;
        public GridItem GridItem;
        public bool IsPortal;
    }

    public enum GridItem
    {
        None = 0,
        Apple = 1,
        Wall = 2,
        Portal = 3,
        Dizzy = 4,
        SnakeHead = 10,
        SnakeBody = 11,
        SnakeTail = 12
    }
}
