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
    }
}
