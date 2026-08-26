using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [Header("Prefab de la torre")]
    public GameObject towerPrefab;

    [Header("Validacion")]
    [Tooltip("Radio de espacio libre que necesita la torre para poder colocarse")]
    public float radioValidacion = 1f;

    [Tooltip("Capas que bloquean la colocacion (camino y estructuras)")]
    public LayerMask capasBloqueadas;

    [Header("Referencias")]
    public Camera camaraPrincipal;

    [Header("Materiales de preview (opcional)")]
    public Material materialValido;
    public Material materialInvalido;

    [Header("Modo de prueba")]
    [Tooltip("Si esta tildado, el modo colocacion arranca activo apenas empieza el juego, sin necesitar boton de UI")]
    public bool activarAlIniciar = true;

    private GameObject previewTorre;
    private bool colocando = false;

    void Start()
    {
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }

        if (activarAlIniciar)
        {
            EmpezarColocacion();
        }
    }

    void Update()
    {
        if (colocando)
        {
            ActualizarPreview();

            if (Input.GetMouseButtonDown(0))
            {
                IntentarColocarTorre();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelarColocacion();
            }
        }
    }

    // Llamar esta funcion desde un boton de UI para empezar a colocar una torre
    public void EmpezarColocacion()
    {
        if (towerPrefab == null) return;

        colocando = true;
        previewTorre = Instantiate(towerPrefab);
        // Desactivar collider del preview para que no interfiera con el raycast
        Collider col = previewTorre.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void ActualizarPreview()
    {
        if (previewTorre == null) return;

        Ray ray = camaraPrincipal.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            previewTorre.transform.position = hit.point;

            bool valido = EsPosicionValida(hit.point);
            PintarPreview(valido);
        }
    }

    bool EsPosicionValida(Vector3 posicion)
    {
        // Si hay algo (camino o estructura) dentro del radio, no es valido
        Collider[] colisiones = Physics.OverlapSphere(posicion, radioValidacion, capasBloqueadas);
        return colisiones.Length == 0;
    }

    void PintarPreview(bool valido)
    {
        Renderer rend = previewTorre.GetComponent<Renderer>();
        if (rend == null) return;

        if (valido && materialValido != null)
        {
            rend.material = materialValido;
        }
        else if (!valido && materialInvalido != null)
        {
            rend.material = materialInvalido;
        }
    }

    void IntentarColocarTorre()
    {
        if (!EsPosicionValida(previewTorre.transform.position))
        {
            // No se puede colocar aca, no hacemos nada (el preview sigue activo)
            return;
        }

        // Confirmar colocacion: instanciar la torre real
        Instantiate(towerPrefab, previewTorre.transform.position, Quaternion.identity);

        // Destruir el preview actual y crear uno nuevo para seguir colocando torres
        Destroy(previewTorre);
        previewTorre = Instantiate(towerPrefab);
        Collider col = previewTorre.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void CancelarColocacion()
    {
        if (previewTorre != null)
        {
            Destroy(previewTorre);
        }
        colocando = false;
    }
}