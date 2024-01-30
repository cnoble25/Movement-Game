using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var outline = gameObject.AddComponent<Outline>();

        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
