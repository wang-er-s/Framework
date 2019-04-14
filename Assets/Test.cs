using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        print(GetComponent<RectTransform>().rect.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
