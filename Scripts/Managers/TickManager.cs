using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class TickManager : MonoBehaviour
    {
        [SerializeField] public float tickInterval = 2f;
        
        public int blockFlag;

        private readonly CancellationTokenSource _lifetimeCts = new();
        private bool _isEnabled = true;
        public event Action OnTick;

        private async void Start()
        {
            await StartTicking();
        }

        private async UniTask StartTicking()
        {
            while (_isEnabled)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(tickInterval), cancellationToken: _lifetimeCts.Token);
                await UniTask.WaitUntil(() => blockFlag == 0, cancellationToken: _lifetimeCts.Token);
                OnTick?.Invoke();
            }
        }

        public void SetEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }

        private void OnDestroy()
        {
            _lifetimeCts?.Cancel();
        }
    }
}
