using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventosPasiva
{
    [Header("Colocación y economía")]
    [Min(0)] public float danioPrimeraTorre;
    [Range(0, 100)] public float probabilidadReembolso;
    [Range(0, 100)] public float probabilidadAura;
    [Min(0)] public float danioAura = 15;
    [Min(0)] public float alcanceAura = 25;
    [Header("Al recibir daño en la casa")]
    [Range(0, 100)] public float probabilidadFrenesi;
    [Min(.1f)] public float duracionFrenesi = 5;
    [Header("Pulso de ralentización")]
    [Min(0)] public int bajasParaPulso;
    [Range(0, 100)] public float ralentizacionPulso = 80;
    [Min(.1f)] public float duracionPulso = 5;
    [Header("Regeneración sin recibir daño")]
    [Min(0)] public float segundosParaCurar;
    [Min(1)] public int curacionPorIntervalo = 1;
    [Min(1)] public int maxCuracionPorNivel = 5;
    [Header("Penalizaciones")]
    [Tooltip("Conservar durante el siguiente nivel y retirar al comenzar el posterior.")]
    public bool soloUnNivel;
    [Min(0)] public int bajasParaEnfurecer;
    [Range(0, 100)] public float probabilidadEnfurecer = 50;
    [Range(0, 100)] public float probabilidadDobleDanioCasa;
    [Range(0, 100)] public float probabilidadLentoAlMatarEspecial;
    [Range(.01f, 1)] public float factorCadenciaLenta = .75f;
    [Min(.1f)] public float duracionCadenciaLenta = 10;
    public bool preciosDobles;
    public bool ventaSinDevolucion;
    public bool sinDescansoOleadas;
    [Range(0, 100)] public float probabilidadEscudo;
    [Min(1)] public int puntosEscudo = 1;

    public EventosPasiva Copiar() => (EventosPasiva)MemberwiseClone();

    public IEnumerable<string> Describir()
    {
        if (danioPrimeraTorre > 0) yield return "Próxima torreta colocada: +" + danioPrimeraTorre + "% daño (una vez).";
        if (probabilidadReembolso > 0) yield return probabilidadReembolso + "% de recuperar el costo al colocar o duplicar la devolución al vender.";
        if (probabilidadAura > 0) yield return probabilidadAura + "% de crear aura: +" + danioAura + "% daño y +" + alcanceAura + "% alcance a otras torretas.";
        if (probabilidadFrenesi > 0) yield return "Al recibir daño: " + probabilidadFrenesi + "% de ataque ×2 por " + duracionFrenesi + " s.";
        if (bajasParaPulso > 0) yield return "Cada " + bajasParaPulso + " bajas: ralentiza " + ralentizacionPulso + "% por " + duracionPulso + " s.";
        if (segundosParaCurar > 0) yield return "Sin daño por " + segundosParaCurar + " s: cura " + curacionPorIntervalo + " (máx. " + maxCuracionPorNivel + "/nivel).";
        if (bajasParaEnfurecer > 0) yield return "Cada " + bajasParaEnfurecer + " bajas: " + probabilidadEnfurecer + "% de enemigos ×2 velocidad hasta terminar el nivel.";
        if (probabilidadDobleDanioCasa > 0) yield return probabilidadDobleDanioCasa + "% de recibir daño doble en la casa.";
        if (probabilidadLentoAlMatarEspecial > 0) yield return "Matar especial: " + probabilidadLentoAlMatarEspecial + "% de cadencia al " + (factorCadenciaLenta * 100) + "% por " + duracionCadenciaLenta + " s.";
        if (preciosDobles) yield return "Torretas al doble de precio.";
        if (ventaSinDevolucion) yield return "Vender torretas no devuelve engranajes.";
        if (sinDescansoOleadas) yield return "Sin descanso entre oleadas.";
        if (probabilidadEscudo > 0) yield return probabilidadEscudo + "% de enemigos con " + puntosEscudo + " escudo(s) contra proyectiles.";
        if (soloUnNivel) yield return "Dura el siguiente nivel.";
    }
}

public struct ResultadoColocacionPasiva
{
    public float danio, danioAura, alcanceAura;
    public bool aura;
    public int reembolso;
}

/// <summary>Estado de contadores y tiempos; recibe delta de juego para detenerse durante pausa.</summary>
public sealed class EstadoEventosPasivas
{
    private sealed class Entrada
    {
        public EventosPasiva datos;
        public int bajas;
        public bool primeraConsumida, participoEnNivel;
    }
    private readonly List<Entrada> entradas = new List<Entrada>();
    private float reloj, rapidoHasta, lentoHasta, pulsoHasta, fuerzaPulso, factorLento = 1, tiempoSinDanio;
    private bool enfurecidos;
    private int curadoNivel;
    public float MultiplicadorCadencia => (reloj < rapidoHasta ? 2 : 1) * (reloj < lentoHasta ? factorLento : 1);
    public float MultiplicadorVelocidad => (enfurecidos ? 2 : 1) * (reloj < pulsoHasta ? 1 - Mathf.Clamp(fuerzaPulso, 0, 90) / 100f : 1);
    public bool HayRegeneracion => entradas.Exists(e => e.datos.segundosParaCurar > 0);
    public bool HayEscudo => entradas.Exists(e => e.datos.probabilidadEscudo > 0);
    public bool HayDobleDanio => entradas.Exists(e => e.datos.probabilidadDobleDanioCasa > 0);
    public float DanioPrimeraPendiente
    {
        get
        {
            float total = 0;
            foreach (var e in entradas) if (!e.primeraConsumida) total += e.datos.danioPrimeraTorre;
            return total;
        }
    }
    public bool SinDescanso => entradas.Exists(e => e.datos.sinDescansoOleadas);
    public bool VentaSinDevolucion => entradas.Exists(e => e.datos.ventaSinDevolucion);
    public int MultiplicadorPrecio => entradas.Exists(e => e.datos.preciosDobles) ? 2 : 1;

    public void Agregar(EventosPasiva datos)
    {
        if (datos != null) entradas.Add(new Entrada { datos = datos.Copiar() });
    }

    public void IniciarNivel()
    {
        entradas.RemoveAll(e => e.datos.soloUnNivel && e.participoEnNivel);
        foreach (var e in entradas) { e.participoEnNivel = true; e.bajas = 0; }
        reloj = rapidoHasta = lentoHasta = pulsoHasta = tiempoSinDanio = 0;
        factorLento = 1;
        curadoNivel = 0;
        enfurecidos = false;
    }

    public int Actualizar(float delta, bool faltaVida)
    {
        if (delta <= 0) return 0;
        reloj += delta;
        tiempoSinDanio += delta;
        float intervalo = float.MaxValue;
        int cantidad = 0, limite = 0;
        foreach (var e in entradas)
        {
            if (e.datos.segundosParaCurar <= 0) continue;
            intervalo = Mathf.Min(intervalo, e.datos.segundosParaCurar);
            cantidad = Mathf.Max(cantidad, e.datos.curacionPorIntervalo);
            limite = Mathf.Max(limite, e.datos.maxCuracionPorNivel);
        }
        if (!faltaVida || curadoNivel >= limite || tiempoSinDanio < intervalo) return 0;
        int curacion = Mathf.Min(cantidad, limite - curadoNivel);
        curadoNivel += curacion;
        tiempoSinDanio = 0;
        return curacion;
    }

    public void RecibirDanio(Func<float> tirar)
    {
        tiempoSinDanio = 0;
        if (reloj < rapidoHasta) return;
        foreach (var e in entradas)
            if (e.datos.probabilidadFrenesi > 0 && PerfilImpactoPasiva.Ocurre(e.datos.probabilidadFrenesi, tirar()))
                rapidoHasta = Mathf.Max(rapidoHasta, reloj + e.datos.duracionFrenesi);
    }

    public void RegistrarBaja(bool especial, Func<float> tirar)
    {
        foreach (var e in entradas)
        {
            e.bajas++;
            if (e.datos.bajasParaPulso > 0 && e.bajas % e.datos.bajasParaPulso == 0)
            {
                if (reloj >= pulsoHasta) fuerzaPulso = 0;
                fuerzaPulso = Mathf.Max(fuerzaPulso, e.datos.ralentizacionPulso);
                pulsoHasta = Mathf.Max(pulsoHasta, reloj + e.datos.duracionPulso);
            }
            if (!enfurecidos && e.datos.bajasParaEnfurecer > 0 && e.bajas % e.datos.bajasParaEnfurecer == 0
                && PerfilImpactoPasiva.Ocurre(e.datos.probabilidadEnfurecer, tirar())) enfurecidos = true;
            if (especial && reloj >= lentoHasta && e.datos.probabilidadLentoAlMatarEspecial > 0 && PerfilImpactoPasiva.Ocurre(e.datos.probabilidadLentoAlMatarEspecial, tirar()))
            {
                factorLento = e.datos.factorCadenciaLenta;
                lentoHasta = reloj + e.datos.duracionCadenciaLenta;
            }
        }
    }

    public float FactorDanioCasa(float tirada)
    {
        float chance = 0;
        foreach (var e in entradas) chance += e.datos.probabilidadDobleDanioCasa;
        return PerfilImpactoPasiva.Ocurre(chance, tirada) ? 2 : 1;
    }

    private float ChanceEconomia()
    {
        float chance = 0;
        foreach (var e in entradas) chance += e.datos.probabilidadReembolso;
        return chance;
    }

    public ResultadoColocacionPasiva Colocar(int precioPagado, float tiradaAura, float tiradaEconomia)
    {
        var resultado = new ResultadoColocacionPasiva();
        float chanceAura = 0;
        foreach (var e in entradas)
        {
            if (!e.primeraConsumida && e.datos.danioPrimeraTorre > 0)
            { resultado.danio += e.datos.danioPrimeraTorre; e.primeraConsumida = true; }
            chanceAura += e.datos.probabilidadAura;
            if (e.datos.probabilidadAura > 0)
            {
                resultado.danioAura = Mathf.Max(resultado.danioAura, e.datos.danioAura);
                resultado.alcanceAura = Mathf.Max(resultado.alcanceAura, e.datos.alcanceAura);
            }
        }
        resultado.aura = PerfilImpactoPasiva.Ocurre(chanceAura, tiradaAura);
        resultado.reembolso = PerfilImpactoPasiva.Ocurre(ChanceEconomia(), tiradaEconomia) ? precioPagado : 0;
        return resultado;
    }

    public int DevolucionVenta(int precioPagado, float porcentajeVenta, float tirada)
    {
        if (VentaSinDevolucion) return 0;
        int normal = Mathf.FloorToInt(precioPagado * Mathf.Clamp01(porcentajeVenta / 100f));
        return normal * (PerfilImpactoPasiva.Ocurre(ChanceEconomia(), tirada) ? 2 : 1);
    }

    public int EscudosIniciales(float tirada)
    {
        float chance = 0; int puntos = 0;
        foreach (var e in entradas)
        {
            chance += e.datos.probabilidadEscudo;
            if (e.datos.probabilidadEscudo > 0) puntos = Mathf.Max(puntos, e.datos.puntosEscudo);
        }
        return PerfilImpactoPasiva.Ocurre(chance, tirada) ? puntos : 0;
    }
}

public static class AlcanceAuraPasiva
{
    public static bool Afecta(float distanciaCuadrada, float radioSinAura, bool mismaTorre) =>
        !mismaTorre && radioSinAura >= 0 && distanciaCuadrada <= radioSinAura * radioSinAura;
}
