using UnityEngine;
using UnityEngine.UI;

public class FantasyHouseHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float vidaMaxima = 100f;
    [SerializeField] private Image rellenoVida;

    public float VidaActual { get; private set; }
    public float VidaMaxima => vidaMaxima;

    void Awake()
    {
        vidaMaxima = Mathf.Max(1f, vidaMaxima);
        VidaActual = vidaMaxima;
        if (rellenoVida != null)
        {
            rellenoVida.type = Image.Type.Filled;
            rellenoVida.fillMethod = Image.FillMethod.Horizontal;
            rellenoVida.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        ActualizarBarra();
    }

    public void RecibirDanio(float cantidad)
    {
        if (cantidad <= 0f || VidaActual <= 0f) return;

        float factor = SistemaPasivas.Eventos.HayDobleDanio ? SistemaPasivas.Eventos.FactorDanioCasa(Random.value) : 1;
        float recibido = SistemaPasivas.DanioCasa(cantidad) * factor;
        VidaActual = Mathf.Max(0f, VidaActual - recibido);
        if (recibido > 0) SistemaPasivas.RegistrarDanioCasa();
        ActualizarBarra();
    }

    public void Curar(float cantidad)
    {
        if (cantidad <= 0 || VidaActual <= 0) return;
        VidaActual = Mathf.Min(vidaMaxima, VidaActual + cantidad);
        ActualizarBarra();
    }

    public void PagarVidaPorPasiva(float porcentaje)
    {
        if (VidaActual <= 0) return;
        VidaActual *= 1 - Mathf.Clamp01(porcentaje / 100f);
        ActualizarBarra();
    }

    void ActualizarBarra()
    {
        if (rellenoVida != null)
            rellenoVida.fillAmount = Mathf.Clamp01(VidaActual / vidaMaxima);
    }
}
