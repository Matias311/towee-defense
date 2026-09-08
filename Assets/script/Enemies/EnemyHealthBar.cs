using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image rellenoVida;
    [SerializeField] private Canvas canvasVida;
    [SerializeField] private Camera camara;
    [SerializeField] private bool ocultarConVidaCompleta = true;

    private EnemyStats estadisticas;

    void Awake()
    {
        estadisticas = GetComponentInParent<EnemyStats>();

        if (canvasVida == null)
        {
            canvasVida = GetComponent<Canvas>();
        }
    }

    void Start()
    {
        ActualizarCamara();
        ActualizarBarra();
    }

    void LateUpdate()
    {
        if (estadisticas == null)
        {
            estadisticas = GetComponentInParent<EnemyStats>();
        }

        if (camara == null)
        {
            ActualizarCamara();
        }

        ActualizarBarra();
        GirarHaciaLaCamara();
    }

    void ActualizarCamara()
    {
        if (camara == null)
        {
            camara = Camera.main;
        }
    }

    void ActualizarBarra()
    {
        if (estadisticas == null || rellenoVida == null) return;

        float porcentaje = estadisticas.vidaMaxima > 0f
            ? Mathf.Clamp01(estadisticas.vidaActual / estadisticas.vidaMaxima)
            : 0f;

        rellenoVida.fillAmount = porcentaje;

        if (canvasVida != null && ocultarConVidaCompleta)
        {
            canvasVida.enabled = porcentaje < 1f;
        }
    }

    void GirarHaciaLaCamara()
    {
        if (canvasVida == null || camara == null) return;

        Vector3 direccion = canvasVida.transform.position - camara.transform.position;
        if (direccion.sqrMagnitude > 0.0001f)
        {
            canvasVida.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }
}
