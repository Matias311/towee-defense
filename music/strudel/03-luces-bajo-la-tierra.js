// ELEVY · NIVEL 3 · Luces bajo la tierra
// Copiar TODO este archivo en https://strudel.cc/ y pulsar Play.
// 96 BPM · 4/4 · forma de 32 compases (~80 s).
// COMPASION: 0 = primera visita; 1 = regreso tras el reloj.
// COMBATE: 0 = exploración; 1 = defensa. Funciona en ambas rutas.
const COMPASION = 0;
const COMBATE = 0;
setcps(96 / 60 / 4);

stack(
  // Una ciudad viva: el mismo hogar armónico en ambas visitas.
  note("<[eb3,g3,bb3] [bb2,d3,f3] [c3,eb3,g3] [ab2,c3,eb3] [eb3,g3,bb3] [f3,ab3,c4] [bb2,eb3,f3] [bb2,d3,f3]>")
    .s("triangle").attack(0.35).decay(0.5).sustain(0.4)
    .release(0.8).lpf(1400 + COMPASION * 700)
    .gain(0.12).room(0.35).orbit(1),

  // Tema de la comunidad: flauta sintetizada, escondida al principio.
  note("<[g5@2 bb5 g5] [f5@2 d5@2] [eb5 g5 c6@2] [bb5 ab5 g5 eb5] [g5@2 f5 eb5] [f5 ab5 c6@2] [bb5@2 g5 f5] [d5@2 f5@2]>")
    .s("sine").attack(0.07).decay(0.25).sustain(0.6)
    .release(0.18).vib(4).vibmod(0.08)
    .gain(0.09 + COMPASION * 0.1).room(0.4).orbit(2)
    .mask(COMPASION ? "<0@4 1@28>" : "<0@8 1@8 0@8 1@8>"),

  // Talleres y habitantes: cuerda pulsada de timbre sintético.
  note("<[eb4 bb4 g4 bb4] [bb3 f4 d4 f4] [c4 g4 eb4 g4] [ab3 eb4 c4 eb4] [eb4 bb4 g4 bb4] [f4 c5 ab4 c5] [bb3 f4 eb4 f4] [bb3 f4 d4 f4]>")
    .s("triangle").fm(0.8).fmh(1)
    .attack(0.003).decay(0.18).sustain(0).release(0.1)
    .gain(0.12).pan(0.35).room(0.2).orbit(3),

  note("<eb2 bb1 c2 ab1 eb2 f2 bb1 bb1>")
    .s("sine").attack(0.02).decay(0.4).sustain(0.25)
    .release(0.25).gain(0.18).orbit(4),

  // Pequeños pasos y madera. No es una marcha enemiga.
  note("eb3 ~ bb3 [~ eb4]").s("sine").fm(2).fmh(1.5)
    .attack(0.001).decay(0.065).sustain(0).release(0.03)
    .gain(0.09).pan(0.65).room(0.15).orbit(5)
    .mask("<0@4 1@24 0@4>"),

  // Primera visita: la lectura amenazante cubre parte de la ciudad.
  note("<[c3,g3] [bb2,f3] [c3,g3] [ab2,eb3] [eb3,bb3] [f3,c4] [bb2,f3] [bb2,f3]>")
    .s("sawtooth").struct("x ~ x ~ x ~ ~ x")
    .attack(0.015).decay(0.14).sustain(0).release(0.08)
    .lpf(850).gain(0.07 * (1 - COMPASION)).orbit(6)
    .mask("<0@8 1@16 0@8>"),

  // Regreso: Elevy responde a la ciudad. La frase encuentra reposo.
  note("<[eb5@2 g5@2] [f5@2 d5@2] [eb5@3 ~] [c5@2 eb5@2] [eb5 g5 f5 d5] [c5@2 ab4@2] [g4 bb4 eb5@2] [f5 d5 eb5@2]>")
    .s("sine").fm(1.4).fmh(2)
    .attack(0.003).decay(0.7).sustain(0).release(0.3)
    .gain(0.15 * COMPASION).pan(0.65).room(0.4).orbit(7)
    .mask("<0@16 1@16>"),

  // Pulso de defensa para cualquiera de las dos visitas.
  note("c2 ~ c2 [~ c2]").s("sine").penv(10).pattack(0).pdecay(0.06).psustain(0)
    .attack(0.001).decay(0.14).sustain(0).release(0.04)
    .gain(0.2 * COMBATE).orbit(8),
  s("~ pink ~ pink").hpf(1500).lpf(4500)
    .attack(0.002).decay(0.06).sustain(0).release(0.02)
    .gain(0.055 * COMBATE).orbit(8)
).postgain(0.7);
