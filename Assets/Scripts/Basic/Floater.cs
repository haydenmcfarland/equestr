using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {

    public float speed = 2.0f;
    public float max_deviation;
    private float original_y;
    private Vector3 vec;
    private bool flip = false;

    void Start()
    {
        vec = gameObject.transform.position;
        original_y = vec.y;

    }
            	
	void Update () 
    {
        float incr = Time.deltaTime * speed;

        if (vec.y > original_y - max_deviation && !flip)
        {
            vec.y -= incr;
        }
        else if (vec.y < original_y + max_deviation && flip)
        {
            vec.y += incr;
        }
        else if (vec.y <= original_y - max_deviation || vec.y >= original_y + max_deviation)
            flip = !flip;

        gameObject.transform.position = vec;

    }
}
