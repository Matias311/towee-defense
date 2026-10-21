#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityEngine;

public class EventosPasivasTests
{
    [Test]
    public void PrimeraColocacionConsumeElBonoUnaSolaVez()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { danioPrimeraTorre = 200 });
        Assert.That(estado.DanioPrimeraPendiente, Is.EqualTo(200));
        Assert.That(estado.Colocar(50, 1, 1).danio, Is.EqualTo(200));
        estado.IniciarNivel();
        Assert.That(estado.Colocar(50, 1, 1).danio, Is.Zero);
    }

    [Test]
    public void FrenesiDuraCincoSegundosSinRefrescarse()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { probabilidadFrenesi = 25, duracionFrenesi = 5 });
        estado.RecibirDanio(() => .24f);
        Assert.That(estado.MultiplicadorCadencia, Is.EqualTo(2));
        estado.Actualizar(2, false);
        estado.RecibirDanio(() => 0);
        estado.Actualizar(3, false);
        Assert.That(estado.MultiplicadorCadencia, Is.EqualTo(1));
    }

    [Test]
    public void PulsoSeActivaCadaDiezBajasYExpira()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { bajasParaPulso = 10, ralentizacionPulso = 80, duracionPulso = 5 });
        for (int i = 0; i < 9; i++) estado.RegistrarBaja(false, () => 1);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(1));
        estado.RegistrarBaja(false, () => 1);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(.2f).Within(.001));
        estado.Actualizar(5, false);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(1));
    }

    [Test]
    public void ReembolsoYVentaNoSeConfundenConUnaNuevaCompra()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { probabilidadReembolso = 15 });
        Assert.That(estado.Colocar(100, 1, .14f).reembolso, Is.EqualTo(100));
        Assert.That(estado.Colocar(100, 1, .15f).reembolso, Is.Zero);
        Assert.That(estado.DevolucionVenta(100, 50, .14f), Is.EqualTo(100));
        Assert.That(estado.DevolucionVenta(100, 50, .15f), Is.EqualTo(50));
        estado.Agregar(new EventosPasiva { ventaSinDevolucion = true });
        Assert.That(estado.DevolucionVenta(100, 50, 0), Is.Zero);
    }

    [Test]
    public void CuracionTieneLimitePorNivelYElDanioReiniciaLaEspera()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { segundosParaCurar = 30, maxCuracionPorNivel = 5 });
        estado.Actualizar(29, true);
        estado.RecibirDanio(() => 1);
        Assert.That(estado.Actualizar(29, true), Is.Zero);
        Assert.That(estado.Actualizar(1, true), Is.EqualTo(1));
        int curacion = 1;
        for (int i = 0; i < 8; i++) curacion += estado.Actualizar(30, true);
        Assert.That(curacion, Is.EqualTo(5));
        estado.IniciarNivel();
        Assert.That(estado.Actualizar(30, true), Is.EqualTo(1));
    }

    [Test]
    public void PreciosDoblesCubrenElSiguienteNivelYDespuesExpiran()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { preciosDobles = true, soloUnNivel = true });
        estado.IniciarNivel();
        Assert.That(estado.MultiplicadorPrecio, Is.EqualTo(2));
        estado.IniciarNivel();
        Assert.That(estado.MultiplicadorPrecio, Is.EqualTo(1));
    }

    [Test]
    public void EnfurecimientoNoSeAcumulaYSeReiniciaPorNivel()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { bajasParaEnfurecer = 25, probabilidadEnfurecer = 50 });
        for (int i = 0; i < 24; i++) estado.RegistrarBaja(false, () => .49f);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(1));
        estado.RegistrarBaja(false, () => .49f);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(2));
        for (int i = 0; i < 25; i++) estado.RegistrarBaja(false, () => 0);
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(2));
        estado.IniciarNivel();
        Assert.That(estado.MultiplicadorVelocidad, Is.EqualTo(1));
    }

    [Test]
    public void EspecialActivaCadenciaReducidaDuranteDiezSegundos()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { probabilidadLentoAlMatarEspecial = 75, factorCadenciaLenta = .75f, duracionCadenciaLenta = 10 });
        estado.RegistrarBaja(false, () => 0);
        Assert.That(estado.MultiplicadorCadencia, Is.EqualTo(1));
        estado.RegistrarBaja(true, () => .74f);
        Assert.That(estado.MultiplicadorCadencia, Is.EqualTo(.75f));
        estado.Actualizar(10, false);
        Assert.That(estado.MultiplicadorCadencia, Is.EqualTo(1));
    }

    [Test]
    public void AuraEscudoYDanioDobleRespetanSusProbabilidades()
    {
        var estado = new EstadoEventosPasivas();
        estado.Agregar(new EventosPasiva { probabilidadAura = 5, probabilidadEscudo = 20, probabilidadDobleDanioCasa = 25 });
        Assert.That(estado.Colocar(50, .049f, 1).aura, Is.True);
        Assert.That(estado.Colocar(50, .05f, 1).aura, Is.False);
        Assert.That(estado.EscudosIniciales(.19f), Is.EqualTo(1));
        Assert.That(estado.EscudosIniciales(.2f), Is.Zero);
        Assert.That(estado.FactorDanioCasa(.24f), Is.EqualTo(2));
        Assert.That(estado.FactorDanioCasa(.25f), Is.EqualTo(1));
    }

    [Test]
    public void PrecioPagadoSeConservaAunqueCambieLaTienda()
    {
        var carta = new CartaTorreta(null, TowerType.Basica, RarezaCarta.Normal, 1, 50);
        int saldo = 100;
        Assert.That(carta.Comprar(ref saldo, 100), Is.True);
        Assert.That(carta.PrecioPagado, Is.EqualTo(100));
        Assert.That(carta.Comprar(ref saldo, 200), Is.True);
        Assert.That(saldo, Is.Zero);
        Assert.That(carta.PrecioPagado, Is.EqualTo(100));
    }

    [Test]
    public void AuraNoSeAplicaAlEmisorNiFueraDelRadioBase()
    {
        Assert.That(AlcanceAuraPasiva.Afecta(0, 5, true), Is.False);
        Assert.That(AlcanceAuraPasiva.Afecta(25, 5, false), Is.True);
        Assert.That(AlcanceAuraPasiva.Afecta(26, 5, false), Is.False);
    }

    [Test]
    public void EscudoBloqueaUnImpactoYMiniJefeNoRecibeInstaKill()
    {
        var objeto = new GameObject("Minijefe de prueba");
        try
        {
            var enemigo = objeto.AddComponent<EnemyStats>();
            enemigo.vidaActual = 100;
            enemigo.defensa = 0;
            enemigo.esMiniJefe = true;
            enemigo.PrepararEscudoPasiva(1);
            var perfil = new PerfilImpactoPasiva { instaKill = 100 };
            SistemaPasivas.AplicarImpacto(enemigo, new DamageData(10), perfil, true);
            Assert.That(enemigo.vidaActual, Is.EqualTo(100));
            SistemaPasivas.AplicarImpacto(enemigo, new DamageData(10), perfil, true);
            Assert.That(enemigo.vidaActual, Is.EqualTo(90));
            enemigo.PrepararEscudoPasiva(1);
            Assert.That(enemigo.escudosProyectil, Is.Zero);
        }
        finally { Object.DestroyImmediate(objeto); }
    }

    [Test]
    public void SangradoNoExcedeElTotalYEscudoConsumeSoloUnProyectil()
    {
        var objeto = new GameObject("Prueba de enemigo");
        try
        {
            var enemigo = objeto.AddComponent<EnemyStats>();
            enemigo.vidaActual = 100;
            enemigo.PrepararEscudoPasiva(1);
            Assert.That(enemigo.ConsumirEscudoProyectil(), Is.True);
            Assert.That(enemigo.ConsumirEscudoProyectil(), Is.False);
            enemigo.AplicarSangrado(20, 4);
            enemigo.ActualizarSangrado(3);
            Assert.That(enemigo.vidaActual, Is.EqualTo(85).Within(.001));
            enemigo.ActualizarSangrado(3);
            Assert.That(enemigo.vidaActual, Is.EqualTo(80).Within(.001));
        }
        finally { Object.DestroyImmediate(objeto); }
    }
}
#endif
