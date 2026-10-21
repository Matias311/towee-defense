// ELEVY · NIVEL 1 · Una chispa bajo la lluvia
// Copiar TODO este archivo en https://strudel.cc/ y pulsar Play.
// 84 BPM · 4/4 · ciclo = un compás · forma de 32 compases (~91 s).
// Síntesis: tecla íntima, cristal de engranadita, bots y lluvia.
// COMBATE: 0 = exploración; 1 = oleada. Cambiar y volver a evaluar.
const COMBATE = 0;
setcps(84 / 60 / 4);

stack(
  // Armonía: Cm(add9), Abmaj7, Ebmaj7, Gsus4 → G.
  note("<[c3,eb3,g3,d4] [c3,eb3,g3,d4] [ab2,c3,eb3,g3] [ab2,c3,eb3,g3] [eb3,g3,bb3,d4] [eb3,g3,bb3,d4] [g2,c3,d3,g3] [g2,b2,d3,g3]>")
    .s("triangle").attack(0.35).decay(0.6).sustain(0.45)
    .release(1.1).lpf(1200).gain(0.11).room(0.35).orbit(1),

  // Tecla de Elevy: el motivo Eb–G–F–D aparece con silencios.
  note("<[eb5@2 ~ g5] [f5@2 d5@2] [c5@3 ~] [~ g4 bb4 c5] [eb5@2 g5 f5] [d5@3 ~] [c5 g4 f4@2] [g4@2 ~ d5]>")
    .s("triangle").fm(0.4).fmh(1)
    .attack(0.008).decay(0.65).sustain(0).release(0.35)
    .lpf(2800).gain(0.26).room(0.4).orbit(2),

  // Engranadita: entra tras cuatro compases y respira al final.
  note("<[~ g5 ~ d6] [~ eb6 d6 ~] [~ c6 ~ g5] [eb6 ~ c6 ~] [~ bb5 ~ g5] [~ d6 f6 ~] [~ g5 ~ c6] [b5 ~ d6 ~]>")
    .s("sine").fm(1.6).fmh(2.01)
    .attack(0.002).decay(0.7).sustain(0).release(0.25)
    .gain(0.095).pan("0.3 0.7").room(0.5).orbit(3)
    .mask("<0@4 1@24 0@4>"),

  // Un bot pequeño: ostinato de madera, ligero y constante.
  note("<[c4 ~ g4 ~ eb4 ~ g4 ~] [c4 ~ g4 ~ d4 ~ g4 ~] [ab3 ~ eb4 ~ c4 ~ eb4 ~] [ab3 ~ eb4 ~ g4 ~ eb4 ~] [eb4 ~ bb4 ~ g4 ~ bb4 ~] [eb4 ~ bb4 ~ d4 ~ bb4 ~] [g3 ~ d4 ~ c4 ~ d4 ~] [g3 ~ d4 ~ b3 ~ d4 ~]>")
    .s("sine").fm(2).fmh(1.5)
    .attack(0.002).decay(0.085).sustain(0).release(0.04)
    .gain(0.11).pan(0.4).room(0.15).orbit(4)
    .mask("<0@8 1@20 0@4>"),

  // Raíces: deja espacio al principio del descenso.
  note("<c2 c2 ab1 ab1 eb2 eb2 g1 g1>")
    .s("sine").attack(0.04).decay(0.5).sustain(0.3)
    .release(0.4).gain(0.19).orbit(5)
    .mask("<0@4 1@28>"),

  // Lluvia estilizada: textura suave, sin grabaciones externas.
  s("pink*8").hpf(2200).lpf(6500)
    .attack(0.035).decay(0.2).sustain(0).release(0.12)
    .gain("0.018 0.009 0.014 0.022 0.01 0.016 0.008 0.02")
    .pan("0.15 0.8 0.35 0.65").room(0.2).orbit(6),

  // Oleada: pulso grave y metal, sin tapar el motivo.
  note("c2 ~ [c2 c2] ~").s("sine")
    .penv(12).pattack(0).pdecay(0.06).psustain(0).attack(0.001)
    .decay(0.17).sustain(0).release(0.05)
    .gain(0.22 * COMBATE).orbit(7),
  s("~ white ~ [white ~ white ~]")
    .bpf(2300).attack(0.001).decay(0.035).sustain(0)
    .release(0.02).gain(0.06 * COMBATE).orbit(7)
).postgain(0.7);
