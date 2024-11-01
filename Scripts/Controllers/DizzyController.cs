using UnityEngine;

namespace Controllers
{
    public class DizzyController : MonoBehaviour
    {
        [SerializeField] private Transform dizzyTransform;
        [SerializeField] private int dizzyRoundCount;

        private int _row;
        private int _column;
        
        public Transform GetDizzyTransform()
        {
            return dizzyTransform;
        }

        public int GetDizzyRoundCount()
        {
            return dizzyRoundCount;
        }

        public void SetRowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public bool IsGridPosMatch(int row, int column)
        {
            return _row == row && _column == column;
        }
    }
}
