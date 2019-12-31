using System.Collections;
using System.Collections.Generic;
using KEngine;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var loader = KResourceModule.LoadBundleAsync("Cube.prefab");
        Object.Instantiate(loader.AsyncResult as Object);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
