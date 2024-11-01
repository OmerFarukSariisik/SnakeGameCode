using TMPro;
using UnityEngine;

namespace UI
{
    public class AppleCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text appleCounterText;

        public void SetAppleCountText(int appleCount, int goal)
        {
            appleCounterText.text = $"{appleCount}/{goal}";
        }
    }
}
