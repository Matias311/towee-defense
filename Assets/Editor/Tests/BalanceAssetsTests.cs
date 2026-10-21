#if UNITY_INCLUDE_TESTS
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;

public class BalanceAssetsTests
{
    [Test]
    public void CuatroTorresTienenRolesYEstadisticasPropias()
    {
        string[] nombres = { "Basica", "Ametralladora", "Francotirador", "Canon" };
        TowerStatsData[] torres = nombres.Select(n => AssetDatabase.LoadAssetAtPath<TowerStatsData>(
            "Assets/Data/Torretas/" + n + ".asset")).ToArray();
        Assert.That(torres.All(t => t != null), Is.True);
        Assert.That(torres.Select(t => t.tipo).Distinct().Count(), Is.EqualTo(4));
        Assert.That(torres[1].tiempoEntreAtaques, Is.LessThan(torres[0].tiempoEntreAtaques));
        Assert.That(torres[2].rango, Is.GreaterThan(torres[0].rango));
        Assert.That(torres[2].penetracion, Is.GreaterThan(torres[1].penetracion));
        Assert.That(torres[3].tipoAtaque, Is.EqualTo(TowerAttackType.Area));
        Assert.That(torres[3].cantidadObjetivos, Is.GreaterThan(1));
    }

    [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
    public void CadaEscenaTieneFichasParaLosCuatroTiposYLosSieteEnemigos(int etapa)
    {
        string escena = File.ReadAllText("Assets/Scenes/etapa" + etapa + ".unity");
        foreach (string carpeta in new[] { "Assets/Data/Torretas", "Assets/Data/Enemigos" })
        {
            string[] rutas = Directory.GetFiles(carpeta, "*.asset");
            Assert.That(rutas.Length, Is.EqualTo(carpeta.EndsWith("Torretas") ? 4 : 7));
            foreach (string ruta in rutas)
            {
                string guid = AssetDatabase.AssetPathToGUID(ruta);
                Assert.That(escena, Does.Contain("estadisticas: {fileID: 11400000, guid: " + guid));
            }
        }
        Assert.That(escena, Does.Not.Contain("guid: b56db9572560a4257b9f62739ec5f313"));
        // Comprueba también que los modelos de las ofertas se importaron con el fileID esperado.
        var modelos = Regex.Matches(escena, @"prefab: \{fileID: 919132149155446097, guid: (\w+), type: 3\}");
        Assert.That(modelos.Count, Is.EqualTo(4));
        foreach (Match modelo in modelos)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(modelo.Groups[1].Value);
            bool encontrado = AssetDatabase.LoadAllAssetsAtPath(ruta).Any(obj =>
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string _, out long id)
                && id == 919132149155446097);
            Assert.That(encontrado, Is.True, ruta);
        }
    }
}
#endif
