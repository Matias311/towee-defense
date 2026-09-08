using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaPersistente : MonoBehaviour
{
    private static MusicaPersistente instancia;

    [Header("Configuracion")]
    [Tooltip("escenas donde estara sonando la msusica")]
    public string[] escenasPermitidas = { "MenuPrincipal", "MenuJuego" };

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        // Si la escena que se acaba de cargar NO esta en la lista de permitidas,
        // se corta la musica y se destruye este objeto.
        bool permitida = false;
        foreach (string nombre in escenasPermitidas)
        {
            if (escena.name == nombre)
            {
                permitida = true;
                break;
            }
        }

        if (!permitida)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instancia = null;
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Limpieza por si se destruye de otra forma, para no dejar el evento enganchado
        if (instancia == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}