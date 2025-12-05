using UnityEngine;

public class AliadoDoBoss : EnemyMelee
{
    [HideInInspector]
    public BossAliados chefe;

    protected override void Morrer()
    {
        // Avisa o chefe antes de morrer
        chefe.AliadoMorreu(this);

        base.Morrer();
    }
}