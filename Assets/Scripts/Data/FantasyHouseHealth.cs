using UnityEngine;
using UnityEngine.UI;

public class FantasyHouseHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float vidaMaxima = 100f;
    [SerializeField] private Image rellenoVida;

    public float VidaActual { get; private set; }

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

        VidaActual = Mathf.Max(0f, VidaActual - cantidad);
        ActualizarBarra();
    }

    void ActualizarBarra()
    {
        if (rellenoVida != null)
            rellenoVida.fillAmount = Mathf.Clamp01(VidaActual / vidaMaxima);
    }
}
