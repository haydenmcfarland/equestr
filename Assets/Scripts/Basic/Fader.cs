using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is used for fading in the screen/audio between scene transitions */
public class Fader : MonoBehaviour {

    public Texture2D fade_out_texture;
    // Placeholder for music (need array)
    // public AudioSource music;
    public float fade_speed = 0.8f;
    private int draw_depth = 0;
    private float alpha = 1.0f;
    public bool fade_in = true;
    private bool fade_complete = false;
    private bool fade_enabled = false;

    /* Every udate cycle, update the alpha value */
    void Update()
    {
        if (!fade_complete && fade_enabled)
        {
            float incr = Time.deltaTime * fade_speed;

            if (fade_in)
                alpha -= incr;
            else
                alpha += incr;

            if (fade_in && alpha <= 0)
                fade_complete = true;
            if (!fade_in && alpha >= 1)
                fade_complete = true;
        }
    }

    /* Draw the black screen and change the audio via the alpha value */
    private void OnGUI()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = draw_depth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fade_out_texture);
        // music.volume = 1 - alpha;
    }

    /* Simple get function for fade_complete */
	public bool FadeComplete()
    {
        return fade_complete;
    }

    /* Initiate fade, -1 for fade_in, 1 for fade_out */
    public void Fade(int dir)
    {
        if (dir == -1)
        {
            fade_in = true;
            alpha = 1.0f;
        }

        else
        {
            fade_in = false;
            alpha = 0.0f;
        }

        fade_complete = false;
        fade_enabled = true;
    }
}
