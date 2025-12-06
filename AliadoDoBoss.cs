using UnityEngine;

public class AliadoDoBoss : EnemyMelee
{
    [HideInInspector]
    public BossAliados boss;

    protected override void Die()
    {
        if (boss != null)
        {
            boss.RemoverAliado(this);
        }

        base.Die();
    }
}
