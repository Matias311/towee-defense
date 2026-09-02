using UnityEngine;

[System.Serializable]
public struct DamageData {
    public float cantidad;
    public float penetracion;
    public DamageType tipo;
    public AlteredDamageType tipoAlterado;
    public float duracion;

    public DamageData(
        float cantidad,
        DamageType tipo = DamageType.Fisico,
        float penetracion = 0f,
        AlteredDamageType tipoAlterado = AlteredDamageType.Veneno,
        float duracion = 3f
    ) {
        this.cantidad = cantidad;
        this.tipo = tipo;
        this.penetracion = penetracion;
        this.tipoAlterado = tipoAlterado;
        this.duracion = duracion;
    }
}
