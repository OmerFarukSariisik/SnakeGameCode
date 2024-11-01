using UnityEngine;

namespace Controllers
{
    public class AppleController : MonoBehaviour
    {
        private int _currentRow;
        private int _currentColumn;

        public void SetGridIndex(int row, int column)
        {
            _currentRow = row;
            _currentColumn = column;
        }

        public (int row, int column) GetGridIndex()
        {
            return (_currentRow, _currentColumn);
        }

        public void SetNewPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
