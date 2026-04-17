using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

namespace TiroAlBlanco.Editor
{
    public class SetupPlayButton : MonoBehaviour
    {
        [MenuItem("Tools/Setup Play Button")]
        public static void ConfigurePlayButton()
        {
            Debug.Log("Buscando PlayButton en la escena...");
            
            Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
            Button playButton = null;
            GameObject playButtonObj = null;
            
            foreach (Button btn in allButtons)
            {
                if (btn.gameObject.name == "PlayButton" && btn.gameObject.scene.isLoaded)
                {
                    playButton = btn;
                    playButtonObj = btn.gameObject;
                    Debug.Log("PlayButton encontrado!");
                    break;
                }
            }
            
            if (playButton == null)
            {
                Debug.LogError("No se encontro el GameObject 'PlayButton' en la escena");
                return;
            }

            Debug.Log("Buscando UIManager en la escena...");
            TiroAlBlanco.UI.UIManager[] allManagers = Resources.FindObjectsOfTypeAll<TiroAlBlanco.UI.UIManager>();
            TiroAlBlanco.UI.UIManager uiManager = null;
            
            foreach (var manager in allManagers)
            {
                if (manager.gameObject.scene.isLoaded)
                {
                    uiManager = manager;
                    Debug.Log("UIManager encontrado!");
                    break;
                }
            }
            
            if (uiManager == null)
            {
                Debug.LogError("No se encontro UIManager en la escena");
                return;
            }

            Debug.Log("Configurando evento del boton...");
            playButton.onClick.RemoveAllListeners();
            
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                playButton.onClick,
                uiManager.OnPlayButtonPressed
            );

            EditorUtility.SetDirty(playButtonObj);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log("Boton de jugar configurado correctamente!");
        }

        [MenuItem("Tools/Verify EventSystem")]
        public static void VerifyEventSystem()
        {
            UnityEngine.EventSystems.EventSystem eventSystem = GameObject.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            
            if (eventSystem == null)
            {
                Debug.LogWarning("No hay EventSystem en la escena. Creando uno...");
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                Debug.Log("EventSystem creado con InputSystemUIInputModule!");
            }
            else
            {
                Debug.Log("EventSystem encontrado: " + eventSystem.gameObject.name);
                var oldModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (oldModule != null)
                {
                    Debug.LogWarning("EventSystem tiene StandaloneInputModule obsoleto. Reemplazando con InputSystemUIInputModule...");
                    UnityEngine.Object.DestroyImmediate(oldModule);
                    eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    Debug.Log("InputSystemUIInputModule agregado!");
                }
            }

            Canvas canvas = GameObject.FindAnyObjectByType<Canvas>();
            if (canvas != null)
            {
                UnityEngine.UI.GraphicRaycaster raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
                if (raycaster == null)
                {
                    Debug.LogWarning("Canvas no tiene GraphicRaycaster. Agregando...");
                    canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    Debug.Log("GraphicRaycaster agregado!");
                }
                else
                {
                    Debug.Log("GraphicRaycaster encontrado en Canvas");
                }
            }
        }
    }
}
