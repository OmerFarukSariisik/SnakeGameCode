using System;
using Data;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float gridSize = 1f;
        
        private GridData[,] _gridData;
        private float HalfGridSize => gridSize / 2f;


        public void InitializeGrid(int rowCount, int columnCount)
        {
            _gridData = new GridData[rowCount, columnCount];
        }

        public void SetGridItem(Vector3 position, GridItem item, int row, int column)
        {
            _gridData[row, column] = new GridData();
            _gridData[row, column].Position = position;
            _gridData[row, column].GridItem = item;
            _gridData[row, column].IsPortal = item == GridItem.Portal;
        }

        public void UpdateGridItem(GridItem item, int row, int column)
        {
            _gridData[row, column].GridItem = item;
        }

        public (Vector3 position, int row, int column, bool isBorder, bool isPortal) GetNextGridData(int row,
            int column, Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    if (column == _gridData.GetLength(1) - 1)
                    {
                        var position = _gridData[row, column].Position + Vector3.right * HalfGridSize;
                        column = 0;
                        return (position, row, column, true, false);
                    }
                    column++;
                    break;
                case Direction.Left:
                    if (column == 0)
                    {
                        var position = _gridData[row, column].Position + Vector3.left * HalfGridSize;
                        column =  _gridData.GetLength(1) - 1;
                        return (position, row, column, true, false);
                    }
                    column--;
                    break;
                case Direction.Down:
                    if (row == 0)
                    {
                        var position = _gridData[row, column].Position + Vector3.back * HalfGridSize;
                        row = _gridData.GetLength(0) - 1;
                        return (position, row, column, true, false);
                    }
                    row--;
                    break;
                case Direction.Up:
                    if (row == _gridData.GetLength(0) - 1)
                    {
                        var position = _gridData[row, column].Position + Vector3.forward * HalfGridSize;
                        row = 0;
                        return (position, row, column, true, false);
                    }
                    row++;
                    break;
            }

            var isPortal = _gridData[row, column].IsPortal;
            return (_gridData[row, column].Position, row, column, false, isPortal);
        }

        public (Vector3 movePos, Vector3 borderPos) GetBorderData(Direction direction, int row, int column)
        {
            var borderPos = Vector3.zero;
            switch (direction)
            {
                case Direction.Right:
                    borderPos = _gridData[row, column].Position + Vector3.left * HalfGridSize;
                    break;
                case Direction.Left:
                    borderPos = _gridData[row, column].Position + Vector3.right * HalfGridSize;
                    break;
                case Direction.Down:
                    borderPos = _gridData[row, column].Position + Vector3.forward * HalfGridSize;
                    break;
                case Direction.Up:
                    borderPos = _gridData[row, column].Position + Vector3.back * HalfGridSize;
                    break;
            }

            return (_gridData[row, column].Position, borderPos);
        }

        public (Vector3 position, int row, int column) GetTeleportPosition(int row, int column)
        {
            var position = Vector3.zero;
            var rowIndex = 0;
            var columnIndex = 0;
            for (var i = 0; i < _gridData.GetLength(0); i++)
            {
                for (var j = 0; j < _gridData.GetLength(1); j++)
                {
                    if (!_gridData[i, j].IsPortal)
                        continue;
                    if (i == row && j == column)
                        continue;
                    
                    position += _gridData[i, j].Position;
                    rowIndex = i;
                    columnIndex = j;
                    return (position, rowIndex, columnIndex);
                }
            }

            Debug.LogError("Couldn't find teleport position");
            return (position, rowIndex, columnIndex);
        }

        public GridItem CheckMoveGrid(GridItem currentItem, int row, int column)
        {
            return currentItem != GridItem.SnakeHead ? GridItem.None : _gridData[row, column].GridItem;
        }

        public Vector3 FindRandomEmptyPointForApple()
        {
            var randRow = 0;
            var randColumn = 0;
            
            while (true)
            {
                randRow = UnityEngine.Random.Range(0, _gridData.GetLength(0));
                randColumn = UnityEngine.Random.Range(0, _gridData.GetLength(1));
                if (_gridData[randRow, randColumn].GridItem == GridItem.None)
                    break;
            }
            
            _gridData[randRow, randColumn].GridItem = GridItem.Apple;
            return _gridData[randRow, randColumn].Position;
        }

        public Vector3 GetGridPosition(int row, int column)
        {
            return _gridData[row, column].Position;
        }
    }
}