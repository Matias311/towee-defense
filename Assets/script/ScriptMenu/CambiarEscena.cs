using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Volver con Escape")]
    [Tooltip("Si esta tildado, apretar Escape carga la escena indicada abajo")]
    public bool volverConEscape = false;

    [Tooltip("Nombre de la escena a la que vuelve al apretar Escape (ej: MenuPrincipal)")]
    public string escenaAlPresionarEscape = "MenuPrincipal";

    void Update()
    {
        bool sePresionoVolver = Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);

        if (volverConEscape && sePresionoVolver)
        {
            SceneManager.LoadScene(escenaAlPresionarEscape);
        }
    }

    // Llamar esta funcion desde el boton, pasandole el nombre EXACTO de la escena
    // (tal cual aparece en Assets/Scenes, sin la extension .unity)
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Alternativa: si preferis, podes crear una funcion especifica para cada escena
    public void IrAOpciones()
    {
        SceneManager.LoadScene("MenuJuego");
    }

    public void IrAMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void IrAlJuego()
    {
        SceneManager.LoadScene("MenuJuego"); // o el nombre exacto de tu escena de juego
    }

    public void SalirDelJuego()
    {
        Application.Quit();

        // Esto es solo para que se vea el efecto al probar dentro del Editor de Unity,
        // ya que Application.Quit() no funciona en el modo Play del editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}