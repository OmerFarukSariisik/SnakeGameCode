using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ControlManager : MonoBehaviour
    {
        [SerializeField] private Button upButton, downButton, leftButton, rightButton;

        private Direction _lastDirection = Direction.Left;

        private void Awake()
        {
            upButton.onClick.AddListener(() => OnButtonClicked(Direction.Up));
            downButton.onClick.AddListener(() => OnButtonClicked(Direction.Down));
            rightButton.onClick.AddListener(() => OnButtonClicked(Direction.Right));
            leftButton.onClick.AddListener(() => OnButtonClicked(Direction.Left));
        }

        private void OnButtonClicked(Direction direction)
        {
            _lastDirection = direction;
        }

        public Direction GetDirection()
        {
            return _lastDirection;
        }

        public bool IsDirectionPossible(Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Right:
                    return _lastDirection != Direction.Left;
                case Direction.Left:
                    return _lastDirection != Direction.Right;
                case Direction.Down:
                    return _lastDirection != Direction.Up;
                case Direction.Up:
                    return _lastDirection != Direction.Down;
                default:
                    return true;
            }
        }
    }
}