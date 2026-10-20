using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "Cartas/Definicion de carta")]
public class CartaDatos : ScriptableObject
{
    [SerializeField] private RarezaCarta rareza;
    [SerializeField] private string titulo = "Nueva carta";
    [SerializeField, TextArea(3, 8)] private string descripcion = "Describe el efecto de la carta.";

    public RarezaCarta Rareza => rareza;
    public string Titulo => titulo;
    public string Descripcion => descripcion;
}
