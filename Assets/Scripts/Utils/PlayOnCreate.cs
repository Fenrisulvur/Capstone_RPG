using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCreate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
     GetComponent<AudioRandomizer>().PlayRandomClip();   
    }
}
