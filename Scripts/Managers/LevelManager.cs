using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using UI;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SnakeManager snakeManager;
        
        [SerializeField] private AppleCounter appleCounter;
        [SerializeField] private LevelEndPopup levelEndPopup;

        [SerializeField] private GameObject grassDark;
        [SerializeField] private GameObject grassLight;
        [SerializeField] private SnakeController snakeHead;
        [SerializeField] private SnakeController snakeBody;
        [SerializeField] private SnakeController snakeTail;
        [SerializeField] private AppleController apple;
        [SerializeField] private GameObject wall;
        [SerializeField] private GameObject portal;
        [SerializeField] private DizzyController dizzy;

        [SerializeField] private Transform grassParent;
        [SerializeField] private Transform snakeParent;
        [SerializeField] private Transform itemParent;
        
        private AppleController _appleController;
        private List<DizzyController> _dizzyControllers = new();

        private int _appleGoalCount;
        private int _collectedAppleCount;


        private readonly Quaternion _grassRotation = Quaternion.Euler(0, -90, 0);

        private void Awake()
        {
            snakeManager.OnAppleHit += OnAppleHit;
            snakeManager.OnSnakeCrash += OnSnakeCrash;
            snakeManager.OnDizzy += OnGetDizzy;
        }

        private void Start()
        {
            LevelDataLoader.Instance.OnLevelLoaded += OnLevelLoaded;
            var currentLevel = LevelProgressSaver.Instance.GetCurrentLevel();
            LevelDataLoader.Instance.LoadLevelData(currentLevel);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            _appleGoalCount = levelData.goal;
            appleCounter.SetAppleCountText(0, levelData.goal);
            gridManager.InitializeGrid(levelData.rows, levelData.columns);
            for (var i = 0; i < levelData.rows; i++)
            {
                for (var j = 0; j < levelData.columns; j++)
                {
                    var objectToInstantiate = (j + i) % 2 == 0 ? grassDark : grassLight;
                    var xPos = j - (levelData.columns - 1) / 2f;
                    var zPos = i - (levelData.rows - 1) / 2f;

                    var position = new Vector3(xPos, 0, zPos);
                    Instantiate(objectToInstantiate, position, _grassRotation, grassParent);


                    var index = i * levelData.columns + j;
                    var item = (GridItem)levelData.grid[index].item;

                    gridManager.SetGridItem(position, item, i, j);
                    position.y = snakeParent.position.y;

                    switch (item)
                    {
                        case GridItem.SnakeHead:
                            var snakeController3 = Instantiate(snakeTail, position, Quaternion.identity, snakeParent);
                            var snakeController2 = Instantiate(snakeBody, position, Quaternion.identity, snakeParent);
                            var snakeController1 = Instantiate(snakeHead, position, Quaternion.identity, snakeParent);
                            snakeManager.AddInitialSnakeControllers(
                                new[] { snakeController1, snakeController2, snakeController3 }, i, j);
                            break;
                        case GridItem.Apple:
                            _appleController = Instantiate(apple, position, Quaternion.identity, itemParent);
                            _appleController.SetGridIndex(i, j);
                            break;
                        case GridItem.Wall:
                            Instantiate(wall, position, Quaternion.identity, itemParent);
                            break;
                        case GridItem.Portal:
                            Instantiate(portal, position, Quaternion.Euler(90, 0, 0), itemParent);
                            break;
                        case GridItem.Dizzy:
                            var dizzyController = Instantiate(dizzy, position, Quaternion.identity, itemParent);
                            _dizzyControllers.Add(dizzyController);
                            dizzyController.SetRowAndColumn(i, j);
                            break;
                    }
                }
            }
        }
        
        private void OnAppleHit()
        {
            _collectedAppleCount++;
            appleCounter.SetAppleCountText(_collectedAppleCount, _appleGoalCount);
            if (_collectedAppleCount == _appleGoalCount)
            {
                snakeManager.StopGame();
                LevelProgressSaver.Instance.SetLevelCompleted();
                levelEndPopup.ShowPopup(true);
                return;
            }
            
            var newApplePosition = gridManager.FindRandomEmptyPointForApple();
            newApplePosition.y = itemParent.position.y;
            _appleController.SetNewPosition(newApplePosition);

            var gridPosition = snakeManager.GetLastSnakeBodyGridPosition();
            snakeManager.LetTailWaitForNewSnakeBody();
            
            var position = gridManager.GetGridPosition(gridPosition.row, gridPosition.column);
            position.y = snakeParent.position.y;
            var snakeController = Instantiate(snakeBody, position, Quaternion.identity, snakeParent);
            snakeManager.AddSnakeController(snakeController, gridPosition.row, gridPosition.column);
        }
        
        private void OnSnakeCrash()
        {
            snakeManager.StopGame();
            levelEndPopup.ShowPopup(false);
        }
        
        private void OnGetDizzy(int row, int column)
        {
            var dizzyController = _dizzyControllers.First(x => x.IsGridPosMatch(row, column));
            snakeManager.GetDizzy(dizzyController.GetDizzyTransform());
            snakeManager.SetDizzyRoundCount(dizzyController.GetDizzyRoundCount());
        }

        private void OnDestroy()
        {
            snakeManager.OnAppleHit -= OnAppleHit;
            snakeManager.OnSnakeCrash -= OnSnakeCrash;
            snakeManager.OnDizzy -= OnGetDizzy;
        }
    }
}