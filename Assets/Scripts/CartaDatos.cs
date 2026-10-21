using UnityEngine;

public enum EfectoCarta { Ninguno, RefuerzoTorres }

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "Cartas/Definicion de carta")]
public class CartaDatos : ScriptableObject
{
    [SerializeField] private RarezaCarta rareza;
    [SerializeField] private string titulo = "Nueva carta";
    [SerializeField, TextArea(3, 8)] private string descripcion = "Describe el efecto de la carta.";

    [SerializeField] private EfectoCarta efecto;
    [SerializeField, Min(0)] private float porcentaje = 20f;

    public EfectoCarta Efecto => efecto;
    public float Porcentaje => porcentaje;
    public RarezaCarta Rareza => rareza;
    public string Titulo => titulo;
    public virtual string Descripcion => descripcion;
}
