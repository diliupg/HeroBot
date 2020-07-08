using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerDamage

{

    int lives { get; set; }

    /* The knockback function will exucute ONLY if the  knockBack bool is true
     *
     */

    void Damage ( float damage, bool knockBack);

    void KnockBack ( );

}
