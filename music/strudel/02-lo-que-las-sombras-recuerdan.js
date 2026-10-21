// ELEVY · NIVEL 2 · Lo que las sombras recuerdan
// Copiar TODO este archivo en https://strudel.cc/ y pulsar Play.
// 84 BPM · 4/4 · 32 compases (~91 s). Reinicia la reproducción
// para escuchar la forma desde el inicio: refugio, sombras, runa.
// COMBATE: 0 = tensión; 1 = defensa del sueño.
const COMBATE = 0;
setcps(84 / 60 / 4);

stack(
  // Recuerdo del refugio: acordes largos y oscuros.
  note("<[c3,eb3,g3] [c3,eb3,g3] [ab2,c3,eb3] [ab2,c3,eb3] [f2,ab2,c3] [f2,ab2,c3] [g2,b2,d3] [g2,b2,d3]>")
    .s("triangle").attack(0.7).decay(0.8).sustain(0.5)
    .release(1.2).lpf(650).gain(0.12).room(0.5).orbit(1)
    .mask("<1@24 0@8>"),

  // Eb–G–F–D deshecho: las sombras dejan huecos en la frase.
  note("<[eb5@2 ~ ~] [~ g5 ~ ~] [f5 ~ ~ d5] [~ ~ c5 ~] [~ g4 ~ eb5] [d5@2 ~ ~] [~ f5 ~ d5] [b4 ~ ~ ~]>")
    .s("triangle").fm(0.7).fmh(1.01)
    .attack(0.01).decay(0.5).sustain(0).release(0.8)
    .lpf(1700).gain(0.21).room(0.6).orbit(2)
    .mask("<1@24 0@8>"),

  // Ecos graves: una sombra cromática que no pertenece al refugio.
  note("<[c2,db3] ~ [ab1,a2] ~ [f2,gb3] ~ [g1,ab2] ~>")
    .s("sawtooth").attack(0.7).decay(0.5).sustain(0.25)
    .release(1).lpf(420).gain(0.055 + COMBATE * 0.035)
    .room(0.55).orbit(3).mask("<0@8 1@16 0@8>"),

  // El mecanismo todavía no encuentra un paso regular.
  note("[c6 ~ ~ g5 ~] [~ d6 ~ ~]")
    .s("sine").fm(3).fmh(1.414)
    .attack(0.001).decay(0.045).sustain(0).release(0.03)
    .gain(0.075).pan("0.2 0.8").room(0.25).orbit(4)
    .mask("<0@4 1@20 0@8>"),

  // Latido y roce de las sombras; la defensa aumenta su presencia.
  note("c2 [~ c2] ~ ~").s("sine").penv(9).pattack(0).pdecay(0.08).psustain(0)
    .attack(0.002).decay(0.2).sustain(0).release(0.05)
    .gain(0.1 + COMBATE * 0.12).orbit(5)
    .mask("<0@4 1@20 0@8>"),
  s("~ pink ~ [~ pink]").bpf(1700)
    .attack(0.12).decay(0.3).sustain(0).release(0.3)
    .gain(0.025 + COMBATE * 0.025).pan("0.75 0.25")
    .room(0.5).orbit(6).mask("<0@8 1@16 0@8>"),

  // Compases 25–32: la runa ordena el pulso y aparece calidez.
  note("<[eb3,g3,bb3] [eb3,g3,bb3] [ab2,c3,eb3] [ab2,c3,eb3] [c3,eb3,g3] [c3,eb3,g3] [bb2,d3,f3] [bb2,d3,f3]>")
    .s("triangle").attack(0.4).decay(0.6).sustain(0.4)
    .release(0.9).lpf(1800).gain(0.12).room(0.5).orbit(1)
    .mask("<0@24 1@8>"),
  note("<[eb5@2 g5@2] [f5 d5 eb5@2] [c5@2 eb5@2] [g5@3 ~] [eb5 g5 f5 d5] [eb5@3 ~] [d5@2 f5@2] [bb4@2 d5@2]>")
    .s("sine").fm(1).fmh(2)
    .attack(0.004).decay(0.85).sustain(0).release(0.4)
    .gain(0.2).room(0.5).orbit(7).mask("<0@24 1@8>"),
  note("c6*4").s("sine").fm(1.5).fmh(2.01)
    .attack(0.001).decay(0.04).sustain(0).release(0.02)
    .gain("0.035 0.02 0.028 0.02").pan("0.4 0.6")
    .orbit(4).mask("<0@24 1@8>")
).postgain(0.7);
