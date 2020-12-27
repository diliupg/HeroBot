using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LalaName : MonoBehaviour
{
    string name1;
    string name2;
    void Start()
    {
        name1 = gameObject.name;
        name2 = gameObject.tag;
        print ( name1 + " " + name2 );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
