using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

/// <summary>Actualiza el puntero de etapa del arco exterior. El reloj interior es decorativo.</summary>
[DisallowMultipleComponent]
public class RelojOleadasHUD : MonoBehaviour
{
    [SerializeField] private EnemySpawner generador;
    [FormerlySerializedAs("punteroOleadas")]
    [SerializeField] private RectTransform punteroEtapa;
    private float anguloPrimeraEtapa;
    private bool poseCapturada;
    private Vector3 angulosIniciales;
    [FormerlySerializedAs("gradosPorOleada")]
    [SerializeField] private float gradosPorEtapa = 22.5f;
    [Tooltip("Grosor adicional del puntero, sin cambiar su tamaño ni su pivote.")]
    [SerializeField, Range(0, 3)] private float grosorPuntero = 1f;
    private EnemySpawner conectado;
    private float destinoEtapa;
    private bool inicializado;

    // También conecta escenas abiertas que aún no tengan guardado el componente del reloj.
    public static void AsegurarConexion(Canvas canvas)
    {
        if (canvas == null) return;
        Transform reloj = canvas.rootCanvas.transform.Find("Reloj");
        if (reloj == null) return;
        var controlador = reloj.GetComponent<RelojOleadasHUD>();
        if (controlador == null) controlador = reloj.gameObject.AddComponent<RelojOleadasHUD>();
        controlador.Conectar();
    }

    private void OnEnable() => Conectar();

    private void Conectar()
    {
        // Resolver por el sprite del puntero exterior; no usar Horas/Minutos del reloj.
        if (punteroEtapa == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                foreach (Image imagen in canvas.rootCanvas.GetComponentsInChildren<Image>(true))
                    if (imagen.sprite != null && imagen.sprite.name == "manecillaoleadas")
                    {
                        punteroEtapa = imagen.rectTransform;
                        break;
                    }
        }
        if (punteroEtapa != null && !poseCapturada)
        {
            // La escena define tamaño, posición, pivote, apariencia y ángulo de la etapa 1.
            // Capturar una sola vez para no acumular la rotación al reconectar o reactivar.
            angulosIniciales = punteroEtapa.localEulerAngles;
            anguloPrimeraEtapa = angulosIniciales.z;
            poseCapturada = true;
            Image imagen = punteroEtapa.GetComponent<Image>();
            if (imagen != null)
            {
                Outline borde = imagen.GetComponent<Outline>();
                if (borde == null) borde = imagen.gameObject.AddComponent<Outline>();
                borde.effectColor = new Color(0, 0, 0, 1);
                borde.effectDistance = new Vector2(grosorPuntero, -grosorPuntero);
                borde.useGraphicAlpha = true;
            }
        }
        if (generador == null || !generador.isActiveAndEnabled)
        {
            generador = null;
            foreach (EnemySpawner candidato in FindObjectsByType<EnemySpawner>())
                if (candidato.gameObject.scene == gameObject.scene && candidato.isActiveAndEnabled)
                {
                    generador = candidato;
                    break;
                }
        }
        if (conectado != generador)
        {
            if (conectado != null) conectado.AlCambiarProgreso -= RecibirProgreso;
            conectado = generador;
            if (conectado != null) conectado.AlCambiarProgreso += RecibirProgreso;
        }
        if (conectado != null) RecibirProgreso(conectado.nivelActual, conectado.oleadaActual);
    }

    private void OnDisable()
    {
        if (conectado != null) conectado.AlCambiarProgreso -= RecibirProgreso;
        conectado = null;
        inicializado = false;
    }

    private void RecibirProgreso(int etapa, int oleada)
    {
        destinoEtapa = anguloPrimeraEtapa + (Mathf.Max(1, etapa) - 1) * gradosPorEtapa;
        if (!inicializado && punteroEtapa != null)
        {
            punteroEtapa.localRotation = Quaternion.Euler(angulosIniciales.x, angulosIniciales.y, destinoEtapa);
            inicializado = true;
        }
    }

    private void LateUpdate()
    {
        if (conectado == null || !conectado.isActiveAndEnabled) Conectar();
        if (conectado == null) return;
        // Respaldo para cambios desde el Inspector; no depende de una caché de números.
        RecibirProgreso(conectado.nivelActual, conectado.oleadaActual);
        Girar(punteroEtapa, destinoEtapa);
    }

    private void Girar(RectTransform manilla, float destino)
    {
        if (manilla == null) return;
        float angulo = Mathf.MoveTowardsAngle(manilla.localEulerAngles.z, destino, 180f * Time.deltaTime);
        manilla.localRotation = Quaternion.Euler(angulosIniciales.x, angulosIniciales.y, angulo);
    }
}
