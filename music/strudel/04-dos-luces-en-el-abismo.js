// ELEVY · ETAPA 4 · Dos luces en el abismo
// Copiar TODO este archivo en https://strudel.cc/ y pulsar Play.
// Propuesta: primer descenso acompañada por el bot legendario.
// 104 BPM · 4/4 · ciclo = un compás · 32 compases (~74 s).
// COMBATE: 0 = exploración; 1 = oleada. Admite valores intermedios.
const COMBATE = 0;
setcps(104 / 60 / 4);

stack(
  // El cráter se abre: calidez en el centro y gravedad en los bordes.
  note("<[c3,eb3,g3,d4] [ab2,c3,eb3,g3] [f2,ab2,c3,eb3] [g2,bb2,d3,f3] [c3,eb3,g3,bb3] [bb2,d3,f3,ab3] [ab2,c3,eb3,g3] [g2,b2,d3,f3]>")
    .s("triangle").attack(0.45).decay(0.6).sustain(0.4)
    .release(0.9).lpf(1300).gain(0.11).room(0.45).orbit(1),

  // Elevy pregunta. Su motivo reaparece al comenzar cada frase.
  note("<[eb5 g5 f5 d5] [eb5@2 c5@2] [~ c5 eb5 f5] [d5@3 ~] [eb5@2 g5 f5] [d5@2 bb4@2] [c5 eb5 g5@2] [f5 d5 b4@2]>")
    .s("triangle").fm(0.5).fmh(1)
    .attack(0.006).decay(0.6).sustain(0).release(0.3)
    .lpf(2700).gain(0.23).pan(0.4).room(0.4).orbit(2)
    .mask("<0@4 1@8 0@4 1@12 0@4>"),

  // El compañero responde en un registro más grave, con voz propia.
  // En los compases 17–28 ambas voces se acompañan.
  note("<[~ ~ g4 c5] [eb4@2 g4@2] [ab4 g4 f4@2] [g4@3 ~] [g4@2 eb4 g4] [f4@2 d4 f4] [eb4@2 c5@2] [d5 b4 g4@2]>")
    .s("sine").fm(1.2).fmh(1)
    .attack(0.025).decay(0.4).sustain(0.25).release(0.2)
    .gain(0.19).pan(0.65).room(0.35).orbit(3)
    .mask("<0@12 1@16 0@4>"),

  // Pasos y engranajes: una maquinaria que ahora avanza con decisión.
  note("<[c4 ~ g4 eb4 ~ g4 d4 ~] [ab3 ~ eb4 c4 ~ eb4 g4 ~] [f3 ~ c4 ab3 ~ c4 eb4 ~] [g3 ~ d4 bb3 ~ d4 f4 ~] [c4 ~ g4 eb4 ~ g4 bb4 ~] [bb3 ~ f4 d4 ~ f4 ab4 ~] [ab3 ~ eb4 c4 ~ eb4 g4 ~] [g3 ~ d4 b3 ~ d4 f4 ~]>")
    .s("triangle").fm(1.3).fmh(1.5)
    .attack(0.002).decay(0.11).sustain(0).release(0.04)
    .lpf(2200).gain(0.095 + COMBATE * 0.035)
    .pan(0.3).room(0.15).orbit(4)
    .mask("<0@4 1@24 0@4>"),

  note("<[c2 ~ g2 ~] [ab1 ~ eb2 ~] [f1 ~ c2 ~] [g1 ~ d2 ~] [c2 ~ g2 ~] [bb1 ~ f2 ~] [ab1 ~ eb2 ~] [g1 ~ d2 ~]>")
    .s("sine").attack(0.015).decay(0.35).sustain(0.2)
    .release(0.15).gain(0.19).orbit(5),

  // Ecos minerales: quedan al descubierto al respirar el arreglo.
  note("<[~ g5 ~ d6] [~ eb6 ~ c6] [~ c6 ~ ab5] [~ d6 ~ bb5] [~ eb6 ~ g6] [~ f6 ~ d6] [~ eb6 ~ c6] [~ d6 ~ b5]>")
    .s("sine").fm(1.6).fmh(2.01)
    .attack(0.002).decay(0.65).sustain(0).release(0.25)
    .gain(0.075).pan("0.25 0.75").room(0.55).orbit(6)
    .mask("<1@8 0@8 1@16>"),

  // Aire del abismo, distinto de la lluvia del primer nivel.
  s("brown*2").hpf(160).lpf(700)
    .attack(0.3).decay(0.5).sustain(0).release(0.3)
    .gain("0.025 0.018").pan("0.7 0.3").room(0.5).orbit(7),

  // Oleada: refuerzos graves y metal sobre el paso de los bots.
  note("c2 ~ [c2 c2] ~").s("sine")
    .penv(12).pattack(0).pdecay(0.06).psustain(0)
    .attack(0.001).decay(0.16).sustain(0).release(0.04)
    .gain(0.22 * COMBATE).orbit(8),
  s("~ white ~ [white ~ white ~]").bpf(2100)
    .attack(0.001).decay(0.045).sustain(0).release(0.02)
    .gain(0.065 * COMBATE).orbit(8)
).postgain(0.7);
