using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTools : MonoBehaviour
{
    public void KillAllSkeletons()
    {
        var skeletons = FindObjectsOfType<EnemyEntity>();
        foreach (var skeleton in skeletons)
        {
            if (skeleton != null && skeleton.GetCurrentHealth() > 0)
            {
                skeleton.TakeDamage(skeleton.GetCurrentHealth());
            }
        }
        var bosses = FindObjectsOfType<BossEntity>();
        foreach (var boss in bosses)
        {
            if (boss != null && boss.GetCurrentHealth() > 0)
            {
                boss.TakeDamage(boss.GetCurrentHealth());
            }
        }
    }
}