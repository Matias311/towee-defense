using System;
using System.Collections.Generic;
using UnityEngine;

public enum EstadisticaPasiva { Danio, Alcance, VelocidadAtaque, Defensa, Penetracion, RadioArea, VelocidadProyectil }

[Serializable]
public class MejoraPasiva
{
    public EstadisticaPasiva estadistica;
    [Min(0), Tooltip("20 significa +20%. Las copias suman sus porcentajes.")]
    public float porcentaje;
    [Min(0), Tooltip("Se suma a la estadística antes del porcentaje. Velocidad de ataque usa ataques por segundo.")]
    public float cantidadFija;
}

[CreateAssetMenu(fileName = "NuevaPasiva", menuName = "Tower Defense/Pasiva")]
public class PasivaDatos : CartaDatos
{
    [Tooltip("Las penalizaciones se asocian a otra pasiva y no aparecen solas entre las recompensas.")]
    public bool esPenalizacion;
    public bool todasLasTorretas = true;
    public TowerType tipoTorreta;
    public bool descripcionAutomatica = true;
    public List<MejoraPasiva> mejoras = new List<MejoraPasiva>();
    public CombatePasiva combate = new CombatePasiva();
    public MundoPasiva mundo = new MundoPasiva();
    public EventosPasiva eventos = new EventosPasiva();
    [Tooltip("Costos que se activan al adquirir esta pasiva. Los ciclos se ignoran.")]
    public List<PasivaDatos> penalizaciones = new List<PasivaDatos>();

    public override string Descripcion => CrearDescripcion(new HashSet<PasivaDatos>());

    private string CrearDescripcion(HashSet<PasivaDatos> visitadas)
    {
        if (!visitadas.Add(this)) return string.Empty;
        if (!descripcionAutomatica) return base.Descripcion;
        var lineas = new List<string>();
        foreach (var mejora in mejoras)
        {
            if (mejora == null) continue;
            string nombre = NombreEstadistica(mejora.estadistica);
            if (mejora.cantidadFija > 0) lineas.Add("+" + mejora.cantidadFija.ToString("0.##") + " " + nombre);
            if (mejora.porcentaje > 0) lineas.Add("+" + mejora.porcentaje.ToString("0.##") + "% " + nombre);
        }
        if (combate != null)
        {
            if (combate.probabilidadCritico > 0) lineas.Add(combate.probabilidadCritico + "% crítico (×" + combate.multiplicadorCritico + ").");
            if (combate.probabilidadSangrado > 0) lineas.Add(combate.probabilidadSangrado + "% sangrado: " + combate.danioSangrado + "% del golpe en " + combate.duracionSangrado + " s.");
            if (combate.probabilidadInstaKill > 0) lineas.Add(combate.probabilidadInstaKill + "% eliminación instantánea" + (combate.instaKillAfectaJefes ? "." : " (sin jefes)."));
        }
        if (mundo != null)
        {
            if (mundo.ralentizacionEnemigos > 0) lineas.Add("-" + mundo.ralentizacionEnemigos + "% velocidad enemiga.");
            if (mundo.defensaCasa > 0) lineas.Add("-" + mundo.defensaCasa + "% daño recibido por la casa.");
            if (mundo.aumentoVelocidadEnemigos > 0) lineas.Add("Enemigos +" + mundo.aumentoVelocidadEnemigos + "% velocidad.");
            if (mundo.aumentoFrecuenciaEnemigos > 0) lineas.Add("+" + mundo.aumentoFrecuenciaEnemigos + "% frecuencia de aparición.");
            if (mundo.probabilidadExtraRaro > 0) lineas.Add(mundo.probabilidadExtraRaro + "% de un raro extra por aparición.");
            if (mundo.probabilidadEspecialOJefe > 0) lineas.Add(mundo.probabilidadEspecialOJefe + "% de especial o jefe extra por oleada.");
            if (mundo.perdidaVidaActual > 0) lineas.Add("Al elegir: pierde " + mundo.perdidaVidaActual + "% de la vida actual de la casa.");
            if (mundo.aumentoDanioRecibidoCasa > 0) lineas.Add("Casa recibe +" + mundo.aumentoDanioRecibidoCasa + "% daño.");
        }
        if (eventos != null) lineas.AddRange(eventos.Describir());
        if (penalizaciones != null) foreach (var costo in penalizaciones)
        {
            if (costo == null) continue;
            string detalle = costo.CrearDescripcion(visitadas);
            if (!string.IsNullOrEmpty(detalle)) lineas.Add("Penalización: " + detalle.Replace("\n", " "));
        }
        if (mejoras.Count > 0 || (combate != null && (combate.probabilidadCritico > 0 || combate.probabilidadInstaKill > 0 || combate.probabilidadSangrado > 0)))
            lineas.Add(todasLasTorretas ? "Todas las torretas." : "Solo torretas: " + tipoTorreta + ".");
        return string.Join("\n", lineas);
    }

    public static string NombreEstadistica(EstadisticaPasiva estadistica)
    {
        switch (estadistica)
        {
            case EstadisticaPasiva.Danio: return "daño";
            case EstadisticaPasiva.Alcance: return "alcance";
            case EstadisticaPasiva.VelocidadAtaque: return "velocidad de ataque";
            case EstadisticaPasiva.Defensa: return "defensa";
            case EstadisticaPasiva.Penetracion: return "penetración";
            case EstadisticaPasiva.RadioArea: return "radio de área";
            default: return "velocidad de proyectil";
        }
    }
}
