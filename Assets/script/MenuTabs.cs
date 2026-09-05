using UnityEngine;

public class MenuTabs : MonoBehaviour
{
    [Header("Paneles")]
    [Tooltip("Arrastra aqui el panel que contiene los sliders de Sonido")]
    public GameObject panelSonido;

    [Tooltip("Arrastra aqui el panel que contiene las opciones de Video")]
    public GameObject panelVideo;

    // Llamar esta funcion desde el boton "Sonido" (OnClick)
    public void MostrarSonido()
    {
        panelSonido.SetActive(true);
        panelVideo.SetActive(false);
    }

    // Llamar esta funcion desde el boton "Video" (OnClick)
    public void MostrarVideo()
    {
        panelSonido.SetActive(false);
        panelVideo.SetActive(true);
    }
}