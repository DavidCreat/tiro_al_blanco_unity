using UnityEngine;
using UnityEditor;

namespace TiroAlBlanco.Editor
{
    public class SetupScene
    {
        [MenuItem("Tools/Setup Scene Lighting")]
        public static void SetupLighting()
        {
            Light directionalLight = GameObject.FindAnyObjectByType<Light>();
            
            if (directionalLight == null || directionalLight.type != LightType.Directional)
            {
                Debug.LogWarning("No hay luz direccional. Creando una...");
                GameObject lightObj = new GameObject("Directional Light");
                directionalLight = lightObj.AddComponent<Light>();
                directionalLight.type = LightType.Directional;
                lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
                Debug.Log("Luz direccional creada!");
            }
            else
            {
                Debug.Log("Luz direccional encontrada: " + directionalLight.gameObject.name);
            }
            
            directionalLight.intensity = 1f;
            directionalLight.color = Color.white;
            
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1f;
            
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.backgroundColor = new Color(0.2f, 0.3f, 0.4f);
                Debug.Log("Camara principal configurada");
            }
            
            Debug.Log("Iluminacion configurada correctamente!");
        }
        
        [MenuItem("Tools/Fix Player Camera")]
        public static void FixPlayerCamera()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError("No se encontro GameObject 'Player'!");
                return;
            }
            
            // Detectar la posicion del escenario (ParedFondo es la pared de atras)
            GameObject paredFondo = GameObject.Find("ParedFondo");
            Vector3 escenarioCentro;
            
            if (paredFondo != null)
            {
                escenarioCentro = paredFondo.transform.position;
                Debug.Log($"ParedFondo encontrado en: {escenarioCentro}");
            }
            else
            {
                GameObject escenario = GameObject.Find("Escenario");
                if (escenario != null)
                {
                    escenarioCentro = escenario.transform.position;
                    Debug.Log($"Escenario encontrado en: {escenarioCentro}");
                }
                else
                {
                    escenarioCentro = Vector3.zero;
                    Debug.LogWarning("No se encontro Escenario ni ParedFondo, usando (0,0,0)");
                }
            }
            
            // Posicionar el Player a 8 unidades del escenario, detras en -Z respecto al escenario
            Vector3 playerPos = new Vector3(escenarioCentro.x, 0f, escenarioCentro.z - 8f);
            player.transform.position = playerPos;
            
            // Rotar el Player para que mire hacia el escenario (asi MouseLook funciona sobre base correcta)
            Vector3 direccion = escenarioCentro - playerPos;
            direccion.y = 0;
            if (direccion.sqrMagnitude > 0.01f)
            {
                player.transform.rotation = Quaternion.LookRotation(direccion);
            }
            Debug.Log($"Player posicionado en: {playerPos}, rotado hacia: {escenarioCentro}");
            Debug.Log($"Player rotacion: {player.transform.rotation.eulerAngles}");
            
            // La camara se resetea localmente, el Player es quien mira al escenario
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.localPosition = new Vector3(0f, 1.6f, 0f);
                cam.transform.localRotation = Quaternion.identity;
                Debug.Log($"Camara mundial - Pos: {cam.transform.position}, Rot: {cam.transform.rotation.eulerAngles}");
            }
            else
            {
                Debug.LogError("No se encontro Camera dentro de Player!");
            }
            
            Debug.Log("Camara del jugador configurada - ahora mira hacia el escenario!");
            
            EditorUtility.SetDirty(player);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
        
        [MenuItem("Tools/Fix Duplicate MouseLook")]
        public static void FixDuplicateMouseLook()
        {
            TiroAlBlanco.Gameplay.MouseLook[] mouseLooks = GameObject.FindObjectsByType<TiroAlBlanco.Gameplay.MouseLook>(FindObjectsInactive.Include);
            Debug.Log($"Total de MouseLook encontrados: {mouseLooks.Length}");
            
            foreach (var ml in mouseLooks)
            {
                string parentName = ml.transform.parent != null ? ml.transform.parent.name : "NONE";
                Debug.Log($"- MouseLook en: {ml.gameObject.name} | Parent: {parentName} | Path: {GetPath(ml.transform)}");
            }
            
            // Eliminar los MouseLook que NO estan dentro de Player
            int eliminados = 0;
            foreach (var ml in mouseLooks)
            {
                bool enPlayer = false;
                Transform t = ml.transform;
                while (t != null)
                {
                    if (t.name == "Player")
                    {
                        enPlayer = true;
                        break;
                    }
                    t = t.parent;
                }
                
                if (!enPlayer)
                {
                    Debug.LogWarning($"Eliminando MouseLook de {ml.gameObject.name} (no esta en Player)");
                    UnityEngine.Object.DestroyImmediate(ml);
                    eliminados++;
                }
            }
            
            Debug.Log($"MouseLook eliminados: {eliminados}. Solo queda el de Player!");
            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
        
        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
        
        [MenuItem("Tools/Inspect Cameras")]
        public static void InspectCameras()
        {
            Camera[] cameras = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include);
            Debug.Log($"Total de camaras en la escena: {cameras.Length}");
            
            foreach (Camera cam in cameras)
            {
                string parentName = cam.transform.parent != null ? cam.transform.parent.name : "NONE";
                Debug.Log($"- {cam.name} | Parent: {parentName} | Pos: {cam.transform.position} | Rot: {cam.transform.rotation.eulerAngles} | LocalPos: {cam.transform.localPosition} | LocalRot: {cam.transform.localRotation.eulerAngles} | Active: {cam.gameObject.activeSelf}");
            }
            
            GameObject cameraHolder = GameObject.Find("CameraHolder");
            if (cameraHolder != null)
            {
                Debug.Log($"CameraHolder | Pos: {cameraHolder.transform.position} | Rot: {cameraHolder.transform.rotation.eulerAngles}");
            }
        }
        
        [MenuItem("Tools/List Scene Objects")]
        public static void ListSceneObjects()
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            Debug.Log($"Total de objetos en la escena: {allObjects.Length}");
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.transform.parent == null)
                {
                    Debug.Log($"- {obj.name} (activo: {obj.activeSelf})");
                }
            }
        }
    }
}
