using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAI : EnemyAI
{
    public override void Init ( )
    {
        base.Init ( );
        Health = health;
    }

    public override void Update ( )
    {
        base.Update ( );

    }

}
