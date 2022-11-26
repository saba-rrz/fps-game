using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSun : MonoBehaviour
{
    // Start is called before the first frame update
    public Animation _animation;
    // Update is called once per frame
    void Update()
    {
        _animation.Play();
    }
}
