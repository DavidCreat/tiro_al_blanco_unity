using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TiroAlBlanco.UI
{
    public class ButtonDebugger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            Debug.Log($"ButtonDebugger en {gameObject.name} - Button interactable: {button?.interactable}");
        }

        private void Start()
        {
            if (button != null)
            {
                Debug.Log($"ButtonDebugger.Start - {gameObject.name} - Interactable: {button.interactable}");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Mouse ENTRO en boton: {gameObject.name}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Mouse SALIO del boton: {gameObject.name}");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"CLICK detectado en boton: {gameObject.name}");
        }
    }
}
