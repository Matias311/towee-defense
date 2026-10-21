#if UNITY_INCLUDE_TESTS
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class AparienciasTorretasTests
{
    [TestCase(RarezaCarta.Normal, "CommonBot")]
    [TestCase(RarezaCarta.Rara, "UncommonBot")]
    [TestCase(RarezaCarta.Epica, "RareBot")]
    [TestCase(RarezaCarta.Legendaria, "LegendaryBot")]
    public void CadaRarezaCreaElBotCorrespondiente(RarezaCarta rareza, string nombre)
    {
        var catalogo = AssetDatabase.LoadAssetAtPath<AparienciasTorretas>("Assets/Resources/AparienciasTorretas.asset");
        GameObject esperado = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Personajes_con_huesos/" + nombre + ".psb");
        Assert.That(esperado, Is.Not.Null, "El PSB debe importar su personaje completo como GameObject.");
        Assert.That(catalogo.Obtener(rareza), Is.EqualTo(esperado));
        GameObject torre = catalogo.Crear(rareza, "Prueba");
        try
        {
            Assert.That(torre.transform.GetChild(0).name, Is.EqualTo(esperado.name));
            Assert.That(torre.GetComponentsInChildren<SpriteRenderer>().Length,
                Is.EqualTo(esperado.GetComponentsInChildren<SpriteRenderer>().Length));
            Assert.That(torre.GetComponentInChildren<Billboard>(), Is.Not.Null);
            Assert.That(torre.GetComponentsInChildren<TorreColocada>(), Is.Empty);
            var partes = torre.GetComponentsInChildren<SpriteRenderer>().Where(p => p.enabled && p.sprite != null).ToArray();
            Assert.That(partes.Length, Is.GreaterThan(0));
            Bounds limites = partes[0].bounds;
            foreach (var parte in partes.Skip(1)) limites.Encapsulate(parte.bounds);
            Assert.That(limites.size.y, Is.EqualTo(catalogo.altura).Within(.02f));
            Assert.That(limites.min.y, Is.EqualTo(0).Within(.02f));
        }
        finally { Object.DestroyImmediate(torre); }
    }
}
#endif
