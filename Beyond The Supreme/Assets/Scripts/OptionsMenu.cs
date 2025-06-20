using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;

    public Slider slider;
    public float sliderValue;
    public Image panelBrillo;  //Imagen que simula el efecto de brillo en pantalla (es una capa encima)

    //Variables de apoyo para calcular el color final
    public float valorBlack;
    public float valorWhite;


    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("Brillo", 0.5f); //Recupera el valor guardado del brillo, en caso de no haber, poner 0.5 por defecto

        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderValue / 3); //Aplica el valor al color del panel
    }

    // Update is called once per frame
    void Update()
    {

        //Calcula cuçanto se tiene que oscurecer o aclarar
        valorBlack = 1 - sliderValue - 0.5f;
        valorWhite = sliderValue - 0.5f;

        //En caso de que el valor se menor que 0.5 se aplica un fondo negro
        if (sliderValue < 0.5f)
        {
            panelBrillo.color = new Color(0, 0, 0, valorBlack);
        }

        //En caso de que el valor se mayor que 0.5 se aplica un fondo blanco
        if (sliderValue > 0.5f)
        {
            panelBrillo.color = new Color(255, 255, 255, valorWhite);
        }
    }
    public void ChangeSlider(float value)
    {
        sliderValue = value; //se actualiza el valor del slider
        PlayerPrefs.SetFloat("Brillo", sliderValue); //se guarda en PlayerPrefs
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderValue / 3);
    }
    public void ChangeVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume); //SE actualiza el volumen en el mixer
    }

}
