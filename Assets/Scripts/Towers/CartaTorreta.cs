using UnityEngine;

/// <summary>Una carta consumible que representa una torreta concreta.</summary>
public sealed class CartaTorreta
{
    public GameObject Prefab { get; }
    public TowerType Tipo { get; }
    public RarezaCarta Rareza { get; }
    public float MultiplicadorDanio { get; }
    public int Precio { get; }
    public bool Comprada { get; private set; }
    public int PrecioPagado { get; private set; }

    public bool Comprar(ref int saldo) => Comprar(ref saldo, Precio);

    public bool Comprar(ref int saldo, int precioActual)
    {
        if (Comprada) return true;
        precioActual = Mathf.Max(1, precioActual);
        if (saldo < precioActual) return false;
        saldo -= precioActual;
        PrecioPagado = precioActual;
        Comprada = true;
        return true;
    }
    public string Nombre => "Torreta " + (Tipo == TowerType.Basica ? "básica"
        : Tipo == TowerType.Ametralladora ? "ametralladora"
        : Tipo == TowerType.Francotirador ? "francotirador" : "cañón");

    public CartaTorreta(GameObject prefab, TowerType tipo, RarezaCarta rareza)
        : this(prefab, tipo, rareza, new BonosDanioTorreta().Multiplicador(rareza)) { }

    public CartaTorreta(GameObject prefab, TowerType tipo, RarezaCarta rareza, float multiplicadorDanio, int precio = 50)
    {
        Precio = Mathf.Max(1, precio);
        MultiplicadorDanio = Mathf.Max(0, multiplicadorDanio);
        Prefab = prefab;
        Tipo = tipo;
        Rareza = rareza;
    }

    // Compatibilidad para consumidores que usen el balance predeterminado.
    public static RarezaCarta SortearRareza(int oleada, float valor) =>
        new ConfiguracionTorretas().SortearRareza(oleada, valor);
}
