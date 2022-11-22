using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class LevelEndBox : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player) && GameManager.GameState != Enums.GameState.GameEnded)
            {
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Success);
                PlayerEvents.OnWin?.Invoke();
            }
        }
    }
}
