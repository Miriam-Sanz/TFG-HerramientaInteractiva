using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Carga la siguiente escena
    }

    public void Exit()
    {
        Debug.Log("Salir...");
        Application.Quit(); //Se cierra el ejecutable
    }
}
