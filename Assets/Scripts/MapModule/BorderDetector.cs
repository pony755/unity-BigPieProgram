using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderDetector : MonoBehaviour
{
    public bool isInvisible;
    private void OnBecameVisible()
    {
        isInvisible = false;
    }
    private void OnBecameInvisible()
    {
        isInvisible = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        isInvisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
