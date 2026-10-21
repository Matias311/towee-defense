using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cartas/Recompensa de pasivas")]
public class RecompensaPasivasDatos : ScriptableObject
{
    public CartaFactory fabrica;
    public Sprite icono;
    public List<PasivaDatos> catalogo = new List<PasivaDatos>();
}
