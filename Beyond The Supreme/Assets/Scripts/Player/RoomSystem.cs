using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RoomSystem : MonoBehaviour
{
    [Header("Configuración de habitaciones")]
    [SerializeField] private List<RoomBase> rooms; //Lista de habitaciones predefinidas

    [Header("Referencias en la escena")]
    [SerializeField] private Transform interactableContainer; //Contenedor para objetos interactuables
    [SerializeField] private Transform characterContainer; //Contenedor para personajes
    [SerializeField] private SpriteRenderer backgroundRenderer; //Renderizador del fondo de la habitación
    [SerializeField] private Transform cameraTarget; //Objeto que la cámara seguirá

    [Header("Cámaras")]
    [SerializeField] private Camera selectionCamera; //Cámara inicial
    [SerializeField] private Camera roomCamera; //Cámara de la habitación temática

    private Vector3 playerStartPosition = new Vector3(38.3f, 34.84f, 0f);

    private int currentRoomIndex = -1; //Índice de la habitación actual

    void Start()
    {
        ShowSelectionView();
    }

    //Carga la siguiente habitación en la secuencia predefinida.
    public void LoadNextRoom()
    {
        currentRoomIndex++;

        if (currentRoomIndex >= rooms.Count)
        {
            currentRoomIndex = 0; 
        }

        LoadRoom(currentRoomIndex);
    }

    //Carga una habitación específica por su índice en la lista.
    private void LoadRoom(int index)
    {
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("No hay habitaciones asignadas en RoomSystem.");
            return;
        }

        RoomBase room = rooms[index];

        if (room == null)
        {
            Debug.LogError("Se intentó cargar una habitación nula.");
            return;
        }

        //Se Cambia la imagen de fondo
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = room.Backgrounds;
            backgroundRenderer.transform.position = new Vector3(38.3f, 34.84f, -5.83f);
            backgroundRenderer.transform.localScale = new Vector3(3.98f, 1f, 1f);
        }


        //Se limpia y carga objetos interactuables
        //Se limpia y carga objetos interactuables en la habitación
        ClearContainer(interactableContainer);

        if (room == null || room.Objects == null)
        {
            Debug.LogError("La habitación o su lista de objetos es nula.");
            return;
        }

        foreach (ObjectsBase interactable in room.Objects)
        {
            if (interactable == null)
            {
                Debug.LogError("Objeto interactuable nulo en la habitación.");
                continue;
            }

            //Se imprime el título de la primera descripción del objeto
            if (interactable.ObjectDescriptions.Count > 0)
            {
                Debug.Log($"Cargando objeto: {interactable.Name}");
            }
            else
            {
                Debug.Log($"Cargando objeto: {interactable.Name}");
            }

            GameObject obj = new GameObject(interactable.Name);
            obj.transform.SetParent(interactableContainer);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();

            //Se verifica si la ilustración está asignada
            if (interactable.ObjectDescriptions.Count > 0 && interactable.ObjectDescriptions[0].ilustration != null)
            {
                sr.sprite = interactable.ObjectDescriptions[0].ilustration;
            }
            else
            {
                Debug.LogError($"{interactable.Name} no tiene ilustración asignada.");
                continue;
            }

            sr.sprite = interactable.Ilustration;
            sr.enabled = true;
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 1;

            //Se asigna la posición definida en el ScriptableObject
            obj.transform.position = new Vector3(interactable.Position.x, interactable.Position.y, 0);
            sr.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }



        //Se limpia y carga personajes
        ClearContainer(characterContainer);
        foreach (CharacterBase character in room.Characters)
        {
            if (character == null)
            {
                Debug.LogError("❌ Personaje nulo en la habitación.");
                continue;
            }

            Debug.Log($"🔍 Cargando personaje: {character.Name}");

            GameObject charObj = new GameObject(character.Name);
            charObj.transform.SetParent(characterContainer);

            SpriteRenderer sr = charObj.AddComponent<SpriteRenderer>();

            if (character.Ilustrations == null || character.Ilustrations.Count == 0)
            {
                Debug.LogError($"❌ {character.Name} no tiene ilustraciones asignadas.");
                continue;
            }

            sr.sprite = character.Ilustrations[0];
            sr.enabled = true;
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 1;

            //Se asigna la posición definida en el ScriptableObject
            charObj.transform.position = new Vector3(character.Position.x, character.Position.y, 0);
            sr.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        //Mueve el objetivo de la cámara a la nueva habitación
        if (cameraTarget != null)
        {
            cameraTarget.position = new Vector3(-34.37f, 26.03f, 0f); ;
        }

        //Se cambia a la cámara de la habitación
        ToggleCameras(false);
    }

   


    //Activa la vista de selección de puertas.
    public void ShowSelectionView()
    {
        ToggleCameras(true);
    }

    //Se activa o desactiva las cámaras según la vista actual.
    private void ToggleCameras(bool isSelectionView)
    {
        if (selectionCamera != null)
        {
            selectionCamera.gameObject.SetActive(isSelectionView);
        }
        if (roomCamera != null)
        {
            roomCamera.gameObject.SetActive(!isSelectionView);
        }
    }


    //Elimina todos los hijos de un contenedor.
    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    public RoomBase GetCurrentRoom()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < rooms.Count)
        {
            return rooms[currentRoomIndex]; //Se vuelve a la habitación actual
        }
        return null;
    }
}
