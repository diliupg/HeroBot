using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAI : EnemyAI
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
