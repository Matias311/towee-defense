using System;
using System.Collections.Generic;
using UnityEngine;

public static class SorteoPasivas
{
    /// <summary>Hasta tres definiciones únicas, priorizando las todavía no adquiridas.</summary>
    public static List<PasivaDatos> Elegir(IReadOnlyList<PasivaDatos> catalogo,
        ICollection<PasivaDatos> adquiridas, Func<int, int> sortearIndice = null)
    {
        var nuevas = new List<PasivaDatos>();
        foreach (var datos in catalogo)
            if (datos != null && !datos.esPenalizacion && !nuevas.Contains(datos) && !adquiridas.Contains(datos)) nuevas.Add(datos);
        var repetidas = new List<PasivaDatos>();
        foreach (var datos in catalogo)
            if (datos != null && !datos.esPenalizacion && adquiridas.Contains(datos) && !repetidas.Contains(datos)) repetidas.Add(datos);
        var resultado = new List<PasivaDatos>();
        while ((nuevas.Count > 0 || repetidas.Count > 0) && resultado.Count < 3)
        {
            var grupo = nuevas.Count > 0 ? nuevas : repetidas;
            int indice = sortearIndice != null ? sortearIndice(grupo.Count) : UnityEngine.Random.Range(0, grupo.Count);
            resultado.Add(grupo[indice]);
            grupo.RemoveAt(indice);
        }
        return resultado;
    }
}
