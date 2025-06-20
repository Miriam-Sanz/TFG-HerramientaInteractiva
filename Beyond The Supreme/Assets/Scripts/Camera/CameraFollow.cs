using UnityEngine;
using Cinemachine;
using System;

public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        //Se obtiene la referencia al componente CinemachineVirtualCamera en el mismo GameObject
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        AssignPlayerToCamera();
    }

    //Método que busca al jugador y lo asigna como objetivo de la cámara
    private void AssignPlayerToCamera()
    {
        GameObject player = FindObjectOfType<PlayerController>()?.gameObject; //Busca un objeto que tenga el componente PlayerController y obtiene su GameObject

        //En caso de que encunetre al jugador
        if (player != null)
        {
            //Asigna su transform como objetivo de seguimiento (Follow) y de mirada (LookAt)
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
        else
        {
            //Si no lo encuentra, se muestra un mensaj de advertencia en consola
            Debug.LogWarning("No se ha encontrado al jugador.");
        }
    }
}