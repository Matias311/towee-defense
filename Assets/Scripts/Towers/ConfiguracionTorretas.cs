using System;
using UnityEngine;

[Serializable]
public class PesosTiposTorreta
{
    [Range(0, 100)] public float basica = 40;
    [Range(0, 100)] public float ametralladora = 30;
    [Range(0, 100)] public float francotirador = 15;
    [Range(0, 100)] public float canon = 15;

    public float Obtener(TowerType tipo)
    {
        switch (tipo)
        {
            case TowerType.Ametralladora: return Mathf.Max(0, ametralladora);
            case TowerType.Francotirador: return Mathf.Max(0, francotirador);
            case TowerType.Canon: return Mathf.Max(0, canon);
            default: return Mathf.Max(0, basica);
        }
    }
}

[Serializable]
public class PorcentajesRarezaTorreta
{
    [Range(0, 100)] public float normal = 65;
    [Range(0, 100)] public float rara = 25;
    [Range(0, 100)] public float epica = 8;
    [Range(0, 100)] public float legendaria = 2;

    public float Obtener(RarezaCarta rareza)
    {
        switch (rareza)
        {
            case RarezaCarta.Rara: return Mathf.Max(0, rara);
            case RarezaCarta.Epica: return Mathf.Max(0, epica);
            case RarezaCarta.Legendaria: return Mathf.Max(0, legendaria);
            default: return Mathf.Max(0, normal);
        }
    }
}

[Serializable]
public class BonosDanioTorreta
{
    [Min(0)] public float normal;
    [Min(0)] public float rara = 20;
    [Min(0)] public float epica = 50;
    [Min(0)] public float legendaria = 100;

    public float Multiplicador(RarezaCarta rareza)
    {
        float porcentaje = rareza == RarezaCarta.Legendaria ? legendaria
            : rareza == RarezaCarta.Epica ? epica : rareza == RarezaCarta.Rara ? rara : normal;
        return 1f + Mathf.Max(0, porcentaje) / 100f;
    }
}

/// <summary>Balance editable. Las probabilidades se normalizan entre las opciones elegibles.</summary>
[Serializable]
public class ConfiguracionTorretas
{
    [Min(2)] public int oleadaBalanceFinal = 15;
    public PesosTiposTorreta tiposIniciales = new PesosTiposTorreta();
    public PesosTiposTorreta tiposFinales = new PesosTiposTorreta
        { basica = 15, ametralladora = 20, francotirador = 32.5f, canon = 32.5f };
    public PorcentajesRarezaTorreta rarezasIniciales = new PorcentajesRarezaTorreta();
    public PorcentajesRarezaTorreta rarezasFinales = new PorcentajesRarezaTorreta
        { normal = 45, rara = 32, epica = 17, legendaria = 6 };
    [Tooltip("Aumento de daño en porcentaje: 20 significa +20%, es decir, daño ×1.2.")]
    public BonosDanioTorreta bonosDanio = new BonosDanioTorreta();

    private float Progreso(int oleada) => Mathf.Clamp01((oleada - 1f) / (Mathf.Max(2, oleadaBalanceFinal) - 1f));

    public float PesoTipo(TowerType tipo, int oleada) =>
        Mathf.Lerp(tiposIniciales.Obtener(tipo), tiposFinales.Obtener(tipo), Progreso(oleada));

    public RarezaCarta SortearRareza(int oleada, float valor)
    {
        float progreso = Progreso(oleada);
        float total = 0;
        for (int i = 0; i < 4; i++)
            total += Mathf.Lerp(rarezasIniciales.Obtener((RarezaCarta)i), rarezasFinales.Obtener((RarezaCarta)i), progreso);
        // Un perfil vacío entrega normales, sin introducir legendarias accidentalmente.
        if (total <= 0) return RarezaCarta.Normal;
        float tirada = Mathf.Clamp(valor, 0, .9999999f) * total;
        RarezaCarta ultima = RarezaCarta.Normal;
        for (int i = 0; i < 4; i++)
        {
            var rareza = (RarezaCarta)i;
            float peso = Mathf.Lerp(rarezasIniciales.Obtener(rareza), rarezasFinales.Obtener(rareza), progreso);
            if (peso <= 0) continue;
            ultima = rareza;
            if (tirada < peso) return rareza;
            tirada -= peso;
        }
        return ultima;
    }
}

[Serializable]
public class PreciosTorretas
{
    [Min(1)] public int normal = 50;
    [Min(1)] public int rara = 100;
    [Min(1)] public int epica = 150;
    [Min(1)] public int legendaria = 250;

    public int Obtener(RarezaCarta rareza) => Mathf.Max(1,
        rareza == RarezaCarta.Legendaria ? legendaria : rareza == RarezaCarta.Epica ? epica
        : rareza == RarezaCarta.Rara ? rara : normal);
}
