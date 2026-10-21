using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Copia los modificadores de los assets; conserva el balance de cada adquisición.</summary>
public sealed class BonosPasivas
{
    private struct Bono
    {
        public EstadisticaPasiva estadistica;
        public bool todas;
        public TowerType tipo;
        public float porcentaje, fijo;
    }
    private readonly List<Bono> bonos = new List<Bono>();
    private struct CombateActivo { public bool todas; public TowerType tipo; public PerfilImpactoPasiva perfil; }
    private readonly List<CombateActivo> combates = new List<CombateActivo>();
    private float ralentizacion, defensaCasa, velocidadEnemigos, frecuenciaEnemigos, extraRaro, especialOJefe, danioCasa;
    public float MultiplicadorVelocidadEnemigos => (1 + velocidadEnemigos / 100f) * (1 - Mathf.Clamp(ralentizacion, 0, 90) / 100f);
    public float MultiplicadorFrecuencia => 1 + frecuenciaEnemigos / 100f;
    public float ProbabilidadExtraRaro => Mathf.Clamp(extraRaro, 0, 100);
    public float ProbabilidadEspecialOJefe => Mathf.Clamp(especialOJefe, 0, 100);
    public float DanioCasa(float cantidad) => cantidad * (1 + danioCasa / 100f) * (1 - Mathf.Clamp(defensaCasa, 0, 90) / 100f);

    public PerfilImpactoPasiva Perfil(TowerType tipo)
    {
        var resultado = new PerfilImpactoPasiva { multiplicadorCritico = 1 };
        foreach (var combate in combates)
        {
            if (!combate.todas && combate.tipo != tipo) continue;
            resultado.critico += combate.perfil.critico;
            resultado.instaKill += combate.perfil.instaKill;
            resultado.instaKillJefes += combate.perfil.instaKillJefes;
            resultado.sangrado += combate.perfil.sangrado;
            if (combate.perfil.sangrado > 0)
            {
                resultado.danioSangrado = Mathf.Max(resultado.danioSangrado, combate.perfil.danioSangrado);
                resultado.duracionSangrado = Mathf.Max(resultado.duracionSangrado, combate.perfil.duracionSangrado);
            }
            if (combate.perfil.critico > 0)
                resultado.multiplicadorCritico = Mathf.Max(resultado.multiplicadorCritico, combate.perfil.multiplicadorCritico);
        }
        resultado.sangrado = Mathf.Clamp(resultado.sangrado, 0, 100);
        resultado.critico = Mathf.Clamp(resultado.critico, 0, 100);
        resultado.instaKill = Mathf.Clamp(resultado.instaKill, 0, 100);
        resultado.instaKillJefes = Mathf.Clamp(resultado.instaKillJefes, 0, 100);
        return resultado;
    }

    public void Agregar(PasivaDatos datos)
    {
        if (datos.combate != null)
        {
            var c = datos.combate;
            combates.Add(new CombateActivo { todas = datos.todasLasTorretas, tipo = datos.tipoTorreta,
                perfil = new PerfilImpactoPasiva { critico = Mathf.Max(0, c.probabilidadCritico), multiplicadorCritico = Mathf.Max(1, c.multiplicadorCritico),
                    instaKill = Mathf.Max(0, c.probabilidadInstaKill), instaKillJefes = c.instaKillAfectaJefes ? Mathf.Max(0, c.probabilidadInstaKill) : 0,
                    sangrado = Mathf.Max(0, c.probabilidadSangrado), danioSangrado = Mathf.Max(0, c.danioSangrado), duracionSangrado = Mathf.Max(.1f, c.duracionSangrado) } });
        }
        if (datos.mundo != null)
        {
            var m = datos.mundo;
            ralentizacion += Mathf.Max(0, m.ralentizacionEnemigos);
            defensaCasa += Mathf.Max(0, m.defensaCasa);
            velocidadEnemigos += Mathf.Max(0, m.aumentoVelocidadEnemigos);
            frecuenciaEnemigos += Mathf.Max(0, m.aumentoFrecuenciaEnemigos);
            extraRaro += Mathf.Max(0, m.probabilidadExtraRaro);
            especialOJefe += Mathf.Max(0, m.probabilidadEspecialOJefe);
            danioCasa += Mathf.Max(0, m.aumentoDanioRecibidoCasa);
        }
        foreach (var mejora in datos.mejoras)
        {
            if (mejora == null) continue;
            bonos.Add(new Bono { estadistica = mejora.estadistica, todas = datos.todasLasTorretas,
                tipo = datos.tipoTorreta, porcentaje = Mathf.Max(0, mejora.porcentaje), fijo = Mathf.Max(0, mejora.cantidadFija) });
        }
    }

    public float Calcular(EstadisticaPasiva estadistica, float valorBase, TowerType tipo)
    {
        float porcentaje = 0, fijo = 0;
        foreach (var bono in bonos)
        {
            if (bono.estadistica != estadistica || (!bono.todas && bono.tipo != tipo)) continue;
            porcentaje += bono.porcentaje;
            fijo += bono.fijo;
        }
        return (valorBase + fijo) * (1f + porcentaje / 100f);
    }
}

public static class SistemaPasivas
{
    private static BonosPasivas bonos = new BonosPasivas();
    public static EstadoEventosPasivas Eventos { get; private set; } = new EstadoEventosPasivas();
    private static readonly List<TorreColocada> torres = new List<TorreColocada>();
    private static int ultimoFrameActualizado = -1;
    private static readonly List<PasivaDatos> adquiridas = new List<PasivaDatos>();
    public static IReadOnlyList<PasivaDatos> Adquiridas => adquiridas;
    private static Scene escenaPreparada;
    private static string escenaContinuacion;

    public static void PrepararContinuacion(string nombreEscena) => escenaContinuacion = nombreEscena;

    public static void PrepararEscena(Scene escena)
    {
        if (escena == escenaPreparada) return;
        if (escena.name != escenaContinuacion) ReiniciarPartida();
        escenaContinuacion = null;
        escenaPreparada = escena;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        bonos = new BonosPasivas();
        Eventos = new EstadoEventosPasivas();
        torres.Clear();
        adquiridas.Clear();
        escenaPreparada = default;
        escenaContinuacion = null;
        ultimoFrameActualizado = -1;
    }

    public static float MultiplicadorVelocidadEnemigos => bonos.MultiplicadorVelocidadEnemigos * Eventos.MultiplicadorVelocidad;
    public static float MultiplicadorFrecuencia => bonos.MultiplicadorFrecuencia;
    public static float ProbabilidadExtraRaro => bonos.ProbabilidadExtraRaro;
    public static float ProbabilidadEspecialOJefe => bonos.ProbabilidadEspecialOJefe;
    public static float DanioCasa(float cantidad) => bonos.DanioCasa(cantidad);
    public static PerfilImpactoPasiva Perfil(TowerType tipo) => bonos.Perfil(tipo);

    public static void AplicarImpacto(EnemyStats objetivo, DamageData danio, PerfilImpactoPasiva perfil, bool esProyectil = false)
    {
        AplicarImpactoConResultado(objetivo, danio, perfil, esProyectil);
    }

    public static bool AplicarImpactoConResultado(EnemyStats objetivo, DamageData danio, PerfilImpactoPasiva perfil, bool esProyectil = false)
    {
        if (objetivo == null || objetivo.vidaActual <= 0) return false;
        if (esProyectil && objetivo.ConsumirEscudoProyectil()) return false;
        bool jefe = objetivo.esMiniJefe || (objetivo.tipo >= EnemyType.BossL1 && objetivo.tipo <= EnemyType.BossL4);
        DamageData resuelto = perfil.Resolver(danio, objetivo.vidaActual, jefe, Random.value, Random.value);
        float vidaAnterior = objetivo.vidaActual;
        objetivo.RecibirDanio(resuelto);
        if (objetivo.vidaActual > 0 && PerfilImpactoPasiva.Ocurre(perfil.sangrado, Random.value))
            objetivo.AplicarSangrado(resuelto.cantidad * perfil.danioSangrado / 100f, perfil.duracionSangrado);
        return objetivo.vidaActual < vidaAnterior;
    }

    public static float Calcular(EstadisticaPasiva estadistica, float valorBase, TowerType tipo) =>
        bonos.Calcular(estadistica, valorBase, tipo);

    public static void ReiniciarPartida()
    {
        Reiniciar();
        RecalcularTorres();
    }

    public static void IniciarNivel()
    {
        Eventos.IniciarNivel();
        foreach (var colocador in Object.FindObjectsByType<TowerPlacer>()) colocador.RefrescarTienda();
    }

    public static void Actualizar(float delta)
    {
        if (delta <= 0 || ultimoFrameActualizado == Time.frameCount) return;
        ultimoFrameActualizado = Time.frameCount;
        var casa = Eventos.HayRegeneracion ? Object.FindAnyObjectByType<FantasyHouseHealth>() : null;
        int curacion = Eventos.Actualizar(delta, casa != null && casa.VidaActual < casa.VidaMaxima);
        if (casa != null && curacion > 0) casa.Curar(curacion);
    }

    public static void RegistrarBaja(EnemyType tipo) =>
        Eventos.RegistrarBaja(tipo == EnemyType.Special || tipo == EnemyType.UltraPro, () => Random.value);

    public static void RegistrarDanioCasa() => Eventos.RecibirDanio(() => Random.value);

    public static void RecalcularTorres()
    {
        var actuales = Object.FindObjectsByType<TowerStats>(FindObjectsInactive.Include);
        // Resolver radios sin auras primero evita realimentación entre emisores.
        foreach (var torre in actuales) torre.RecalcularPasivas(true);
        foreach (var torre in actuales) torre.RecalcularPasivas();
    }

    public static Vector2 BonoAura(TowerStats objetivo)
    {
        Vector2 resultado = Vector2.zero;
        foreach (var emisor in torres)
        {
            if (emisor == null || !emisor.isActiveAndEnabled || !emisor.TieneAura || emisor.Estadisticas == null) continue;
            if (!AlcanceAuraPasiva.Afecta(Vector3.SqrMagnitude(emisor.transform.position - objetivo.transform.position),
                emisor.Estadisticas.RangoSinAura, emisor.Estadisticas == objetivo)) continue;
            resultado.x += emisor.DanioAura;
            resultado.y += emisor.AlcanceAura;
        }
        return resultado;
    }

    public static void RegistrarTorre(TorreColocada torre)
    {
        if (torres.Contains(torre)) return;
        torres.Add(torre);
        var efecto = Eventos.Colocar(torre.PrecioPagado, Random.value, Random.value);
        torre.Estadisticas.AgregarBonoColocacion(efecto.danio);
        if (efecto.aura) torre.ConfigurarAura(efecto.danioAura, efecto.alcanceAura);
        if (efecto.reembolso > 0) torre.Colocador.RegistrarPuntos(efecto.reembolso);
        RecalcularTorres();
    }

    public static void RetirarTorre(TorreColocada torre)
    {
        if (torres.Remove(torre)) RecalcularTorres();
    }

    public static void ConfigurarEscudo(EnemyStats enemigo)
    {
        if (Eventos.HayEscudo) enemigo.PrepararEscudoPasiva(Eventos.EscudosIniciales(Random.value));
    }

    public static void Adquirir(PasivaDatos datos)
    {
        if (datos == null) return;
        adquiridas.Add(datos);
        AdquirirInterna(datos, new HashSet<PasivaDatos>());
        RecalcularTorres();
        foreach (var enemigo in Object.FindObjectsByType<EnemyStats>()) ConfigurarEscudo(enemigo);
        foreach (var colocador in Object.FindObjectsByType<TowerPlacer>()) colocador.RefrescarTienda();
    }

    private static void AdquirirInterna(PasivaDatos datos, HashSet<PasivaDatos> visitadas)
    {
        if (datos == null || !visitadas.Add(datos)) return;
        bonos.Agregar(datos);
        Eventos.Agregar(datos.eventos);
        if (datos.mundo != null && datos.mundo.perdidaVidaActual > 0)
            foreach (var casa in Object.FindObjectsByType<FantasyHouseHealth>())
                casa.PagarVidaPorPasiva(datos.mundo.perdidaVidaActual);
        if (datos.penalizaciones != null)
            foreach (var costo in datos.penalizaciones) AdquirirInterna(costo, visitadas);
    }
}
