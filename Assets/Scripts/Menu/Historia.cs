using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HistoriaManager : MonoBehaviour
{
    [System.Serializable]
    public class Pagina
    {
        [Tooltip("Imagen (Sprite) que se muestra en esta pagina. Si lo dejas vacio se mantiene la imagen anterior")]
        public Sprite imagen;

        [Tooltip("Objeto padre con los textos como hijos (ej: Historia1). Cada hijo es un texto que se muestra en orden")]
        public GameObject contenedorTextos;
    }

    [Header("Paginas (en orden)")]
    public Pagina[] paginas;

    [Header("Referencias")]
    [Tooltip("El componente Image del objeto 'Image' (fondo de la historia)")]
    public Image imagenFondo;

    [Header("Avance")]
    [Tooltip("0 = solo avanza con Enter o click. Mayor a 0 = avanza solo tras esos segundos")]
    public float segundosAutomatico = 0f;
    [Tooltip("Letras por segundo. 0 = sin efecto de escritura")]
    public float efectoEscrituraVelocidad = 40f;
    [Tooltip("Si esta activo, los textos anteriores de la misma pagina se quedan visibles")]
    public bool acumularTextos = false;

    [Header("Escenas")]
    public string escenaSiguiente = "etapa2";

    int paginaActual;
    int textoActual;
    TMP_Text[] textosPagina;
    TMP_Text textoVisible;
    float letrasMostradas;
    float tiempoTexto;
    bool cargando;

    void Start()
    {
        Time.timeScale = 1f;

        if (paginas == null || paginas.Length == 0)
        {
            Debug.LogError("HistoriaManager: la lista 'Paginas' esta vacia. Ponle tamaño 4 y arrastra Historia1..Historia4 en 'Contenedor Textos'.");
            return;
        }

        // Apagar todos los textos al inicio
        foreach (var p in paginas)
        {
            if (p.contenedorTextos == null) continue;
            p.contenedorTextos.SetActive(false);
        }

        MostrarPagina(0);
    }

    void Update()
    {
        if (cargando) return;

        // Saltar toda la historia
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IrAlJuego();
            return;
        }

        // Si no hay paginas configuradas, no hacer nada
        if (paginas == null || paginas.Length == 0 || textosPagina == null) return;

        // Efecto maquina de escribir
        if (textoVisible != null && textoVisible.maxVisibleCharacters < textoVisible.textInfo.characterCount)
        {
            letrasMostradas += efectoEscrituraVelocidad * Time.deltaTime;
            textoVisible.maxVisibleCharacters = (int)letrasMostradas;
        }

        bool escribiendo = EstaEscribiendo();
        if (!escribiendo) tiempoTexto += Time.deltaTime;

        bool presiono = Input.GetKeyDown(KeyCode.Return)
                     || Input.GetKeyDown(KeyCode.KeypadEnter)
                     || Input.GetMouseButtonDown(0);

        if (presiono)
        {
            if (escribiendo) CompletarTexto();   // primer click completa el texto
            else Siguiente();                    // segundo click avanza
        }
        else if (segundosAutomatico > 0f && !escribiendo && tiempoTexto >= segundosAutomatico)
        {
            Siguiente();
        }
    }

    bool EstaEscribiendo()
    {
        if (textoVisible == null || efectoEscrituraVelocidad <= 0f) return false;
        textoVisible.ForceMeshUpdate();
        return textoVisible.maxVisibleCharacters < textoVisible.textInfo.characterCount;
    }

    void CompletarTexto()
    {
        textoVisible.maxVisibleCharacters = int.MaxValue;
    }

    void Siguiente()
    {
        textoActual++;
        if (textoActual < textosPagina.Length)
        {
            MostrarTexto(textoActual);
        }
        else
        {
            paginaActual++;
            if (paginaActual >= paginas.Length) IrAlJuego();
            else MostrarPagina(paginaActual);
        }
    }

    void MostrarPagina(int i)
    {
        // Apagar la pagina anterior
        foreach (var p in paginas)
            if (p.contenedorTextos != null) p.contenedorTextos.SetActive(false);

        var pagina = paginas[i];

        if (pagina.imagen != null && imagenFondo != null)
            imagenFondo.sprite = pagina.imagen;

        if (pagina.contenedorTextos == null)
        {
            textosPagina = new TMP_Text[0];
            Siguiente();
            return;
        }

        pagina.contenedorTextos.SetActive(true);

        // Tomar los textos hijos (incluso los desactivados)
        textosPagina = pagina.contenedorTextos.GetComponentsInChildren<TMP_Text>(true);
        foreach (var t in textosPagina) t.gameObject.SetActive(false);

        textoActual = 0;
        MostrarTexto(0);
    }

    void MostrarTexto(int i)
    {
        if (textosPagina.Length == 0) return;

        if (!acumularTextos)
            foreach (var t in textosPagina) t.gameObject.SetActive(false);

        textoVisible = textosPagina[i];
        textoVisible.gameObject.SetActive(true);
        textoVisible.ForceMeshUpdate();

        if (efectoEscrituraVelocidad > 0f)
        {
            textoVisible.maxVisibleCharacters = 0;
            letrasMostradas = 0f;
        }
        else
        {
            textoVisible.maxVisibleCharacters = int.MaxValue;
        }

        tiempoTexto = 0f;
    }

    void IrAlJuego()
    {
        cargando = true;
        SceneManager.LoadScene(escenaSiguiente);
    }
}