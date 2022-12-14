using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D[] gibs = gameObject.GetComponentsInChildren<Rigidbody2D>();

        foreach(Rigidbody2D child in gibs)
        {
            Vector2 randomVector = new Vector2(Random.Range(1f,4f), Random.Range(1f, 3f));
            child.AddForce ( randomVector * 10000f );
        }
    }
}
