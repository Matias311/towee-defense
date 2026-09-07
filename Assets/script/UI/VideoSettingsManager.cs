using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VideoSettingsManager : MonoBehaviour
{
    [Header("UI - Resolucion")]
    [Tooltip("El Dropdown donde se listan las resoluciones disponibles")]
    public TMPro.TMP_Dropdown dropdownResolucion;

    [Header("UI - Modo de pantalla")]
    [Tooltip("El Dropdown donde se elige: Pantalla completa exclusiva, Pantalla completa con ventana (sin bordes), o Ventana")]
    public TMPro.TMP_Dropdown dropdownModoPantalla;

    private Resolution[] resolucionesDisponibles;
    private List<Resolution> resolucionesFiltradas = new List<Resolution>();

    void Start()
    {
        ConfigurarDropdownResolucion();
        ConfigurarDropdownModoPantalla();
    }

    void ConfigurarDropdownResolucion()
    {
        resolucionesDisponibles = Screen.resolutions;
        resolucionesFiltradas.Clear();
        dropdownResolucion.ClearOptions();

        List<string> opciones = new List<string>();
        int indiceActual = 0;

        for (int i = 0; i < resolucionesDisponibles.Length; i++)
        {
            Resolution res = resolucionesDisponibles[i];
            string texto = res.width + " x " + res.height;

            // Evitar resoluciones duplicadas (a veces Unity repite la misma resolucion
            // con distintas tasas de refresco)
            if (opciones.Contains(texto)) continue;

            opciones.Add(texto);
            resolucionesFiltradas.Add(res);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                indiceActual = resolucionesFiltradas.Count - 1;
            }
        }

        dropdownResolucion.AddOptions(opciones);
        dropdownResolucion.value = indiceActual;
        dropdownResolucion.RefreshShownValue();

        dropdownResolucion.onValueChanged.AddListener(CambiarResolucion);
    }

    void ConfigurarDropdownModoPantalla()
    {
        dropdownModoPantalla.ClearOptions();

        List<string> opciones = new List<string>
        {
            "Pantalla completa exclusiva",
            "Pantalla completa (ventana sin bordes)",
            "Ventana"
        };

        dropdownModoPantalla.AddOptions(opciones);

        // Reflejar el modo actual del juego en el Dropdown
        int indiceActual = 2; // Ventana por defecto
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                indiceActual = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                indiceActual = 1;
                break;
            case FullScreenMode.Windowed:
                indiceActual = 2;
                break;
        }

        dropdownModoPantalla.value = indiceActual;
        dropdownModoPantalla.RefreshShownValue();

        dropdownModoPantalla.onValueChanged.AddListener(CambiarModoPantalla);
    }

    void CambiarResolucion(int indice)
    {
        Resolution res = resolucionesFiltradas[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    void CambiarModoPantalla(int indice)
    {
        FullScreenMode modo = FullScreenMode.Windowed;

        switch (indice)
        {
            case 0:
                modo = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                modo = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                modo = FullScreenMode.Windowed;
                break;
        }

        Screen.fullScreenMode = modo;
    }
}