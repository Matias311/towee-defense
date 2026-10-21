#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PasivasTests
{
    private readonly List<PasivaDatos> temporales = new List<PasivaDatos>();

    private PasivaDatos Nueva()
    {
        var datos = ScriptableObject.CreateInstance<PasivaDatos>();
        temporales.Add(datos);
        return datos;
    }

    [TearDown]
    public void Limpiar()
    {
        foreach (var datos in temporales) Object.DestroyImmediate(datos);
        temporales.Clear();
    }

    [Test]
    public void CatalogoTieneVeintePasivasPositivasPorRareza()
    {
        var configuracion = AssetDatabase.LoadAssetAtPath<RecompensaPasivasDatos>("Assets/Resources/RecompensaPasivas.asset");
        Assert.That(configuracion.catalogo, Has.Count.EqualTo(20));
        Assert.That(configuracion.catalogo.Distinct().Count(), Is.EqualTo(20));
        Assert.That(configuracion.catalogo.Count(p => p.Rareza == RarezaCarta.Normal), Is.EqualTo(6));
        Assert.That(configuracion.catalogo.Count(p => p.Rareza == RarezaCarta.Rara), Is.EqualTo(5));
        Assert.That(configuracion.catalogo.Count(p => p.Rareza == RarezaCarta.Epica), Is.EqualTo(5));
        Assert.That(configuracion.catalogo.Count(p => p.Rareza == RarezaCarta.Legendaria), Is.EqualTo(4));
    }

    [Test]
    public void PenalizacionesNoSeOfrecenComoRecompensaIndependiente()
    {
        var costo = Nueva(); costo.esPenalizacion = true;
        var ofertas = SorteoPasivas.Elegir(new[] { costo, Nueva(), Nueva(), Nueva() }, new List<PasivaDatos>(), n => 0);
        Assert.That(ofertas, Has.Count.EqualTo(3));
        Assert.That(ofertas.Contains(costo), Is.False);
    }

    [Test]
    public void CatalogoAsociaLasSieteNegativasYDejaNormalesSinCosto()
    {
        var configuracion = AssetDatabase.LoadAssetAtPath<RecompensaPasivasDatos>("Assets/Resources/RecompensaPasivas.asset");
        var usadas = new HashSet<PasivaDatos>();
        foreach (var pasiva in configuracion.catalogo)
        {
            if (pasiva.Rareza == RarezaCarta.Normal) Assert.That(pasiva.penalizaciones, Is.Empty);
            foreach (var costo in pasiva.penalizaciones)
            {
                Assert.That(costo.esPenalizacion, Is.True);
                usadas.Add(costo);
            }
        }
        Assert.That(usadas.Count, Is.EqualTo(7));
    }

    [Test]
    public void DescripcionMuestraCostoCompletoSinRecursionPorCiclos()
    {
        var positiva = Nueva();
        positiva.eventos.danioPrimeraTorre = 200;
        var negativa = Nueva();
        negativa.esPenalizacion = true;
        negativa.eventos.preciosDobles = true;
        negativa.eventos.soloUnNivel = true;
        positiva.penalizaciones.Add(negativa);
        negativa.penalizaciones.Add(positiva);
        string descripcion = positiva.Descripcion;
        StringAssert.Contains("200", descripcion);
        StringAssert.Contains("doble de precio", descripcion);
        StringAssert.Contains("siguiente nivel", descripcion);
        Assert.That(descripcion.Length, Is.LessThan(1000));
    }

    [TestCase(0, 0, false)]
    [TestCase(5, .049f, true)]
    [TestCase(5, .05f, false)]
    [TestCase(100, 1, true)]
    public void ProbabilidadRespetaLosLimites(float chance, float tirada, bool esperado)
    {
        Assert.That(PerfilImpactoPasiva.Ocurre(chance, tirada), Is.EqualTo(esperado));
    }

    [Test]
    public void CriticoEInstaKillFuncionanSinEliminarJefesPorDefecto()
    {
        var datos = Nueva();
        datos.combate.probabilidadCritico = 20;
        datos.combate.probabilidadInstaKill = 10;
        var bonos = new BonosPasivas();
        bonos.Agregar(datos);
        var perfil = bonos.Perfil(TowerType.Basica);
        var danio = new DamageData(10, DamageType.Fisico, 5);
        var critico = perfil.Resolver(danio, 500, false, .99f, .1f);
        Assert.That(critico.cantidad, Is.EqualTo(20));
        Assert.That(critico.penetracion, Is.EqualTo(5));
        var instantaneo = perfil.Resolver(danio, 500, false, .05f, .99f);
        Assert.That(instantaneo.cantidad, Is.EqualTo(500));
        Assert.That(instantaneo.tipo, Is.EqualTo(DamageType.Verdadero));
        Assert.That(perfil.Resolver(danio, 500, true, .05f, .99f).cantidad, Is.EqualTo(10));
        Assert.That(danio.cantidad, Is.EqualTo(10));
    }

    [Test]
    public void RiesgosYBonosSeCopianAlAdquirirYSumanEntreCopias()
    {
        var datos = Nueva();
        datos.combate.probabilidadCritico = 20;
        datos.mundo.defensaCasa = 20;
        datos.mundo.ralentizacionEnemigos = 5;
        var bonos = new BonosPasivas();
        bonos.Agregar(datos);
        bonos.Agregar(datos);
        datos.combate.probabilidadCritico = 90;
        datos.mundo.defensaCasa = 90;
        Assert.That(bonos.Perfil(TowerType.Basica).critico, Is.EqualTo(40));
        Assert.That(bonos.DanioCasa(100), Is.EqualTo(60).Within(.001));
        Assert.That(bonos.MultiplicadorVelocidadEnemigos, Is.EqualTo(.9f).Within(.001));
    }

    [Test]
    public void RiesgoDeVelocidadSeCombinaConRalentizacion()
    {
        var datos = Nueva();
        datos.mundo.ralentizacionEnemigos = 5;
        datos.mundo.aumentoVelocidadEnemigos = 30;
        datos.mundo.aumentoFrecuenciaEnemigos = 20;
        datos.mundo.probabilidadExtraRaro = 10;
        datos.mundo.probabilidadEspecialOJefe = 50;
        var bonos = new BonosPasivas();
        bonos.Agregar(datos);
        Assert.That(bonos.MultiplicadorVelocidadEnemigos, Is.EqualTo(1.235f).Within(.001));
        Assert.That(bonos.MultiplicadorFrecuencia, Is.EqualTo(1.2f).Within(.001));
        Assert.That(bonos.ProbabilidadExtraRaro, Is.EqualTo(10));
        Assert.That(bonos.ProbabilidadEspecialOJefe, Is.EqualTo(50));
    }

    [Test]
    public void PoderSangrientoExplicaElCostoDeVida()
    {
        var datos = AssetDatabase.LoadAssetAtPath<PasivaDatos>("Assets/Data/Pasivas/Epicas/PoderSangriento.asset");
        Assert.That(datos.mejoras.Single().porcentaje, Is.EqualTo(20));
        Assert.That(datos.mundo.perdidaVidaActual, Is.EqualTo(15));
        StringAssert.Contains("vida actual", datos.Descripcion);
    }
}
#endif
