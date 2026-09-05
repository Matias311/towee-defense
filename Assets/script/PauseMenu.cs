using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel del menu de pausa (todo el overlay)")]
    public GameObject panelPausa;

    [Header("Grupo de botones principales de pausa")]
    [Tooltip("El objeto que agrupa los botones Reanudar, Opciones y Salir (NO el fondo)")]
    public GameObject grupoBotonesPausa;

    [Header("Submenu de opciones (Sonido/Video)")]
    public GameObject panelOpciones;

    [Header("Opciones")]
    public bool pausarTiempo = true;

    private bool juegoPausado = false;

    void Start()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (grupoBotonesPausa != null) grupoBotonesPausa.SetActive(true);
    }

    void Update()
    {
        bool sePresionoVolver = Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);

        if (sePresionoVolver)
        {
            // Si el submenu de opciones esta abierto, volver solo cierra el submenu
            if (panelOpciones != null && panelOpciones.activeSelf)
            {
                CerrarOpciones();
                return;
            }

            // Si no, alterna abrir/cerrar el menu de pausa completo
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        juegoPausado = true;
        panelPausa.SetActive(true);
        grupoBotonesPausa.SetActive(true);
        panelOpciones.SetActive(false);

        if (pausarTiempo) Time.timeScale = 0f;
    }

    // Conectar al boton "Reanudar"
    public void Reanudar()
    {
        juegoPausado = false;
        panelPausa.SetActive(false);

        if (pausarTiempo) Time.timeScale = 1f;
    }

    // Conectar al boton "Opciones" del menu de pausa
    public void AbrirOpciones()
    {
        grupoBotonesPausa.SetActive(false);
        panelOpciones.SetActive(true);
    }

    // Conectar a un boton "Volver" dentro del panel de opciones (opcional),
    // o se llama sola al presionar Escape estando en el submenu
    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        grupoBotonesPausa.SetActive(true);
    }

    // Conectar al boton "Salir" del menu de pausa
    public void SalirDelJuego()
    {
        Time.timeScale = 1f;
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}