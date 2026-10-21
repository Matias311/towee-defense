using UnityEngine;

/// <summary>Datos de una torreta comprada y colocada; nunca se agrega a los previews.</summary>
[DisallowMultipleComponent]
public class TorreColocada : MonoBehaviour
{
    public TowerPlacer Colocador { get; private set; }
    public TowerStats Estadisticas { get; private set; }
    public int PrecioPagado { get; private set; }
    public bool Vendida { get; private set; }
    public bool TieneAura { get; private set; }
    public float DanioAura { get; private set; }
    public float AlcanceAura { get; private set; }
    private bool registrada;

    public void Configurar(TowerPlacer colocador, TowerStats estadisticas, int precioPagado)
    {
        if (registrada) return;
        Colocador = colocador;
        Estadisticas = estadisticas;
        PrecioPagado = precioPagado;
        registrada = true;
        SistemaPasivas.RegistrarTorre(this);
    }

    public void ConfigurarAura(float danio, float alcance)
    {
        TieneAura = true;
        DanioAura = danio;
        AlcanceAura = alcance;
    }

    public bool MarcarVendida()
    {
        if (Vendida) return false;
        Vendida = true;
        return true;
    }

    void OnMouseDown()
    {
        if (!registrada || Vendida || Colocador == null || !Colocador.isActiveAndEnabled
            || Colocador.HayColocacionActiva || BloqueoEntradaUI.BloqueaColocacion()) return;
        VentaTorreHUD.Mostrar(this);
    }

    void OnMouseOver()
    {
        if (registrada && !Vendida && Input.GetKeyDown(KeyCode.V)) Colocador.IntentarVender(this);
    }

    void OnEnable()
    {
        if (registrada && Application.isPlaying) SistemaPasivas.RecalcularTorres();
    }

    void OnDisable()
    {
        if (registrada && Application.isPlaying) SistemaPasivas.RecalcularTorres();
    }

    void OnDestroy()
    {
        if (registrada && Application.isPlaying) SistemaPasivas.RetirarTorre(this);
    }
}
