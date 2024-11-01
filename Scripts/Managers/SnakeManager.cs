using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace Managers
{
    public class SnakeManager : MonoBehaviour
    {
        [SerializeField] private ControlManager controlManager;
        [SerializeField] private TickManager tickManager;
        [SerializeField] private GridManager gridManager;

        private List<SnakeController> _snakeControllers = new();
        
        private GridItem _lastHitGridItem;
        private int _dizzyRoundCount = 0;
        private bool _isDizzy = false;

        public event Action OnAppleHit;
        public event Action OnSnakeCrash;
        public event Action<int, int> OnDizzy;

        private void Awake()
        {
            tickManager.OnTick += OnTick;
        }

        private void OnTick()
        {
            _lastHitGridItem = GridItem.None;
            
            for (var i = 0; i < _snakeControllers.Count; i++)
            {
                var snakeController = _snakeControllers[i];
                var gridPosition = snakeController.GetGridPosition();
                var rowIndex = gridPosition.rowIndex;
                var columnIndex = gridPosition.columnIndex;
                var gridItem = snakeController.GetGridItem();

                if (!snakeController.HasLastDirection())
                {
                    gridManager.UpdateGridItem(gridItem, rowIndex, columnIndex);
                    continue;
                }

                if (i == 0 && !IsDizzy())
                    CheckHeadDirection(snakeController);

                CalculateNewPositions(snakeController, rowIndex, columnIndex).Forget();
            }
            
            SetNextDirections();
            if (_lastHitGridItem == GridItem.Apple)
                SetNextDirectionForTailOnly();
        }

        private void CheckHeadDirection(SnakeController snakeController)
        {
            if (controlManager.IsDirectionPossible(snakeController.GetLastDirection()))
                snakeController.SetLastDirection(controlManager.GetDirection());
        }

        private async UniTaskVoid CalculateNewPositions(SnakeController snakeController, int rowIndex, int columnIndex)
        {
            tickManager.blockFlag++;
            var gridItem = snakeController.GetGridItem();
            var lastDirection = snakeController.GetLastDirection();
            var nextGridData = gridManager.GetNextGridData(rowIndex, columnIndex, lastDirection);
            snakeController.SetGridPosition(nextGridData.row, nextGridData.column);
            SaveIfHit(gridItem, nextGridData.row, nextGridData.column);
            gridManager.UpdateGridItem(GridItem.None, rowIndex, columnIndex);
            gridManager.UpdateGridItem(gridItem, nextGridData.row, nextGridData.column);
            
            await snakeController.MoveToPositionAsync(nextGridData.position, tickManager.tickInterval,
                nextGridData.isBorder);
            if (nextGridData.isBorder)
            {
                var borderData = gridManager.GetBorderData(lastDirection, nextGridData.row, nextGridData.column);
                snakeController.TeleportToPosition(borderData.borderPos);
                snakeController.MoveToPositionAsync(borderData.movePos, tickManager.tickInterval, true).Forget();
            }

            if (nextGridData.isPortal)
            {
                var teleportPosition = gridManager.GetTeleportPosition(nextGridData.row, nextGridData.column);
                snakeController.TeleportToPosition(teleportPosition.position);
                
                snakeController.SetGridPosition(teleportPosition.row, teleportPosition.column);
                gridManager.UpdateGridItem(GridItem.None, nextGridData.row, nextGridData.column);
                gridManager.UpdateGridItem(gridItem, teleportPosition.row, teleportPosition.column);
            }
            
            tickManager.blockFlag--;
        }

        private void SaveIfHit(GridItem gridItem, int rowIndex, int columnIndex)
        {
            if (_lastHitGridItem != GridItem.None)
                return;
            _lastHitGridItem = gridManager.CheckMoveGrid(gridItem, rowIndex, columnIndex);
            CheckHit(rowIndex, columnIndex);
        }

        private void CheckHit(int rowIndex, int columnIndex)
        {
            switch (_lastHitGridItem)
            {
                case GridItem.Apple:
                    OnAppleHit?.Invoke();
                    break;
                case GridItem.Wall:
                case GridItem.SnakeBody:
                    OnSnakeCrash?.Invoke();
                    break;
                case GridItem.SnakeTail:
                    if (!_snakeControllers.Last().HasLastDirection())
                        OnSnakeCrash?.Invoke();
                    break;
                case GridItem.Dizzy:
                    OnDizzy?.Invoke(rowIndex, columnIndex);
                    break;
            }
        }

        private void SetNextDirections()
        {
            for (var i = _snakeControllers.Count - 1; i > 0; i--)
                _snakeControllers[i].SetLastDirection(_snakeControllers[i - 1].GetLastDirection());
        }

        private void SetNextDirectionForTailOnly()
        {
             _snakeControllers[^1].LetTailMoveAgain();
        }

        public void AddSnakeController(SnakeController snakeController, int rowIndex, int columnIndex)
        {
            _snakeControllers.Insert(_snakeControllers.Count - 1, snakeController);
            snakeController.SetGridPosition(rowIndex, columnIndex);
        }

        public void AddInitialSnakeControllers(SnakeController[] snakeControllers, int rowIndex, int columnIndex)
        {
            _snakeControllers.AddRange(snakeControllers);
            _snakeControllers.First().SetLastDirection(controlManager.GetDirection());
            foreach (var snakeController in snakeControllers)
                snakeController.SetGridPosition(rowIndex, columnIndex);
        }

        public (int row, int column) GetLastSnakeBodyGridPosition()
        {
            return _snakeControllers[^2].GetGridPosition();
        }

        public void LetTailWaitForNewSnakeBody()
        {
            _snakeControllers.Last().LetTailWaitForNewSnakeBody();
        }

        public void StopGame()
        {
            tickManager.SetEnabled(false);
        }

        public void GetDizzy(Transform dizzyTransform)
        {
            _snakeControllers.First().SetDizzyToPosition(dizzyTransform);
        }

        public void SetDizzyRoundCount(int dizzyRoundCount)
        {
            _isDizzy = true;
            _dizzyRoundCount = dizzyRoundCount;
        }

        private bool IsDizzy()
        {
            if (!_isDizzy) return false;

            _dizzyRoundCount--;
            if (_dizzyRoundCount == 0)
            {
                _snakeControllers.First().RemoveDizzy();
                _isDizzy = false;
            }

            return _dizzyRoundCount > 0;
        }

        private void OnDestroy()
        {
            tickManager.OnTick -= OnTick;
        }
    }
}