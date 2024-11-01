using System;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class SnakeController : MonoBehaviour
    {
        [SerializeField] private GridItem gridItem;
        [SerializeField] private Transform dizzyPositioner;

        private Transform _dizzyTransform;
        
        private Direction _lastDirection = Direction.None;
        private Direction _tailLastDirectionBeforeNewBody = Direction.None;
        private int _rowIndex = 0;
        private int _columnIndex = 0;

        public void SetLastDirection(Direction direction)
        {
            _lastDirection = direction;
        }
        
        public Direction GetLastDirection()
        {
            return _lastDirection;
        }

        public bool HasLastDirection()
        {
            return _lastDirection != Direction.None;
        }

        public void SetGridPosition(int rowIndex, int columnIndex)
        {
            _rowIndex = rowIndex;
            _columnIndex = columnIndex;
        }

        public GridItem GetGridItem()
        {
            return gridItem;
        }

        public (int rowIndex, int columnIndex) GetGridPosition()
        {
            return (_rowIndex, _columnIndex);
        }

        public async UniTask MoveToPositionAsync(Vector3 position, float duration, bool isTeleport)
        {
            duration = isTeleport ? duration / 2f : duration;
            position.y = transform.position.y;
            SetRotation();
            transform.DOKill();
            transform.DOMove(position, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
        }

        public void TeleportToPosition(Vector3 position)
        {
            transform.DOKill();
            position.y = transform.position.y;
            transform.position = position;
        }

        private void SetRotation()
        {
            switch (_lastDirection)
            {
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(0, 90f, 0);
                    break;
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(0, -90f, 0);
                    break;
                case Direction.Down:
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case Direction.Up:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        public void LetTailWaitForNewSnakeBody()
        {
            _tailLastDirectionBeforeNewBody = _lastDirection;
            _lastDirection = Direction.None;
        }

        public void LetTailMoveAgain()
        {
            _lastDirection = _tailLastDirectionBeforeNewBody;
        }

        public void SetDizzyToPosition(Transform dizzyTransform)
        {
            _dizzyTransform = dizzyTransform;
            _dizzyTransform.parent = dizzyPositioner;
            _dizzyTransform.localPosition = Vector3.zero;
        }

        public void RemoveDizzy()
        {
            Destroy(_dizzyTransform.gameObject);
        }
    }
}
