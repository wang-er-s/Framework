using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Framework;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int aa;
    
    [Button()]
    void Start()
    {
        foreach (Transform trans in transform)
        {
            print(trans.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
