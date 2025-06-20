using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance; //Singleton para mantener una sola instancia del jugador

    public float moveSpeed; //velocidad a la que se mueve el jugador
    private bool isMoving; //para comprobar si se está moviendo
    private Vector2 input;

    public LayerMask solidObjectsLayer; //Capa con los objetos sólidos para las colisiones
    public LayerMask doors; //capa con las puertas para la interacción

    private Animator animator;

    //Elementos para la confirmación de la puerta
    public GameObject dialogPanel;
    public Button yesButton;
    public Button noButton;

    private bool isDialogActive = false; //para comprobar si hay algún dialogo activo
    private bool isInteracting; //para comprobar si se está interactuando

    public RoomSystem roomSystem;
    private bool isInRoom = false; //para comprobar si el jugador se encuentra en una habitación temática


    //UI del sistema de diálogos
    public GameObject dialogBox; //Cuadro de diálogo
    public TMP_Text dialogText; //Texto dentro del cuadro de diálogo
    public Image characterExpression; //Imagen para expresiones
    public Image characterIllustration; //Imagen para las ilustraciones del personaje
    public Image objectIllustration; //imagen para las ilustraciones de los objetos
    public TMP_Text dialogNameText; // Texto para mostrar el nombre del personaje u objeto

    //private int currentDialogIndex = 0;

    private Queue<CharacterBase.DialogEntry> currentDialog; //Cola de diálogos del personaje
    private CharacterBase currentCharacter; //personaje actual en el diálogo
    private ObjectsBase currentObject; //objeto actual en la interacción
    
    private Queue<ObjectsBase.ObjectEntry> currentObjectDescriptions; //Cola de descripciones de los objetos
    private bool isObjectDialog; //para comprobar si el texto que se muestra es de un objeto

    private void Awake()
    {
        animator = GetComponent<Animator>(); //se obtiene el componente animator del jugador
    }

    void Update()
    {
        if (isDialogActive || isInteracting) //si hay un diálogo en curso o se está realizando una interacción, el personaje no se mueve, se bloquea su movimiento
        {
            animator.SetBool("isMoving", false); // Detiene la animación de movimiento
            if (Input.GetKeyDown(KeyCode.Return)) ShowNextDialog();
            return;
        }

        //si hay un diálogo en curso permite pasar al siguiente diálogo
        if (dialogBox.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextDialog();
        }

        //Solo se detecta una entrada si el perosnaje no se está moviendo
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; // Quita movimiento diagonal
            if (isInRoom) input.y = 0; // Si está en una habitación, solo movimiento horizontal

            //cuando hay una entrada el jugador se mueve
            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position + new Vector3(input.x, input.y, 0);

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);

        //se comprueba si hay alguna interacción
        CheckInteraction();
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        CheckDoors(); //se comprueba si el jugador está cerca de una puerta
    }

    private bool IsWalkable(Vector3 targetPosition)
    {

        //Bloquea el movimieno si choca con algo que pertenece a la capa de objetos sólidos, colisiona
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer) != null) return false;
        return true;
    }

    private void CheckDoors()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.5f, doors) != null)
        {
            Debug.Log("Puerta detectada");
            Dialogs(); //Activa el panel que le pregunta al jugador si desea escoger esa puerta
        }
    }

    private void CheckInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E)) //Si el jugador pulsa "E"
        {
            float interactionRange = 2f; //Distancia para detectar interacción
            Vector2 playerPos = transform.position;

            //Busca el personaje más cercano que tenga RoomSystem
            CharacterBase nearestCharacter = null;
            float nearestCharacterDist = float.MaxValue;

            foreach (CharacterBase character in roomSystem.GetCurrentRoom().Characters)
            {
                float dist = Vector2.Distance(playerPos, character.Position);
                if (dist < interactionRange && dist < nearestCharacterDist)
                {
                    nearestCharacter = character;
                    nearestCharacterDist = dist;
                }
            }

            if (nearestCharacter != null)
            {
                StartDialog(nearestCharacter.Dialogs, nearestCharacter);
                return;
            }

            //Busca el objeto más cercano que tenga RoomSystem
            ObjectsBase nearestObject = null;
            float nearestObjectDist = float.MaxValue;

            foreach (ObjectsBase obj in roomSystem.GetCurrentRoom().Objects)
            {
                float dist = Vector2.Distance(playerPos, obj.Position);
                if (dist < interactionRange && dist < nearestObjectDist)
                {
                    nearestObject = obj;
                    nearestObjectDist = dist;
                }
            }

            if (nearestObject != null)
            {
                StartDialog(null, null, nearestObject);
            }
        }
    }





    public void StartDialog(List<CharacterBase.DialogEntry> dialogEntries, CharacterBase character, ObjectsBase obj = null)
    {
        if ((dialogEntries == null || dialogEntries.Count == 0) && (obj == null || obj.ObjectDescriptions.Count == 0))
        {
            Debug.LogError("No hay diálogos ni descripciones de objeto válidas.");
            return;
        }

        currentCharacter = character;
        //currentDialogIndex = 0;

        //Si hay diálogos de personaje, se almacenan en la cola
        currentDialog = dialogEntries != null ? new Queue<CharacterBase.DialogEntry>(dialogEntries) : new Queue<CharacterBase.DialogEntry>();

        //Si hay un objeto, almacena sus descripciones en la cola
        if (obj != null)
        {
            currentObjectDescriptions = new Queue<ObjectsBase.ObjectEntry>(obj.ObjectDescriptions);
            isObjectDialog = true;
        }
        else
        {
            currentObjectDescriptions = new Queue<ObjectsBase.ObjectEntry>();
            isObjectDialog = false;
        }

        //Se muestra el primer diálogo o descripción
        ShowNextDialog();
        dialogBox.SetActive(true);
        isDialogActive = true;
    }

    //Se muestra el siguiente diálogo de personaje o la siguiente descripción del objeto
    public void ShowNextDialog()
    {
        if (isObjectDialog && currentObjectDescriptions.Count > 0)
        {
            ObjectsBase.ObjectEntry currentEntry = currentObjectDescriptions.Dequeue();

            dialogText.text = currentEntry.description; //Se muestra la descripción del objeto
            dialogNameText.text = currentEntry.title;   //Se muestra el título de la descripción
            objectIllustration.sprite = currentEntry.ilustration; //Se muestra la ilustración
            if (objectIllustration.sprite != null)
            {
                objectIllustration.SetNativeSize(); //Ajusta el RectTransform al tamaño del sprite en píxeles
                objectIllustration.transform.localScale = new Vector3(0.2f, 0.2f, 0.3f);
            }

            objectIllustration.gameObject.SetActive(true);
            characterIllustration.gameObject.SetActive(false);
            characterExpression.gameObject.SetActive(false);
        }
        else if (!isObjectDialog && currentDialog.Count > 0)
        {
            CharacterBase.DialogEntry currentEntry = currentDialog.Dequeue();

            dialogText.text = currentEntry.text; //Se muestra el diálogo
            dialogNameText.text = currentEntry.speakerName; //Se muestra el nombre del personaje
            characterExpression.sprite = currentEntry.expression; //Se muestra la expresión del personaje
            characterIllustration.sprite = currentEntry.dialogIlustration; //Usa la ilustración específica del diálogo
           


            characterIllustration.gameObject.SetActive(true);
            characterExpression.gameObject.SetActive(true);
            objectIllustration.gameObject.SetActive(false);
        }
        else
        {
            EndDialog(); //cuando ya no hay más dialogos o descripciones se finaliza el diálogo
        }
    }







    private void EndDialog()
    {
        dialogBox.SetActive(false);
        characterIllustration.gameObject.SetActive(false); //Se oculta la imagen al terminar el diálogo
        characterExpression.gameObject.SetActive(false);
        objectIllustration.gameObject.SetActive(false);
        dialogNameText.text = ""; //Se oculta el nombre
        isDialogActive = false;
    }


    private void Dialogs()
    {
        isDialogActive = true;
        dialogPanel.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
    }

    private void OnYesPressed()
    {
        isDialogActive = false;
        dialogPanel.SetActive(false);
        isInRoom = true;

        if (roomSystem != null)
        {
            roomSystem.LoadNextRoom();
            roomSystem.ShowSelectionView();
        }
    }

    private void OnNoPressed()
    {
        isDialogActive = false;
        dialogPanel.SetActive(false);
    }
}
