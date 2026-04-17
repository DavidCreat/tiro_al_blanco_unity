using UnityEngine;
using UnityEngine.InputSystem;
using TiroAlBlanco.Core;

namespace TiroAlBlanco.Gameplay
{
    public class PauseController : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current == null) return;
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

            if (GameManager.Instance == null) return;

            if (GameManager.Instance.CurrentState == GameState.Playing)
                GameManager.Instance.PauseGame();
            else if (GameManager.Instance.CurrentState == GameState.Paused)
                GameManager.Instance.ResumeGame();
        }
    }
}
