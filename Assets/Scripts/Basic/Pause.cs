using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    // Pause_Canvas is the pause screen used during game pausing.
    public GameObject Pause_Canvas;

    private bool paused = false;
    public bool blocked = false;

    void Start() {
        Pause_Canvas.gameObject.SetActive(false);
    }

    public bool Paused()
    {
        return paused;
    }
    void Update() {

        if (!blocked)
        {
            bool fade_complete = gameObject.GetComponent<Fader>().FadeComplete();
            if (Input.GetKeyDown("escape") && fade_complete)
            {
                if (paused)
                {
                    Time.timeScale = 1.0f;
                    Pause_Canvas.gameObject.SetActive(false);
                    paused = false;
                }
                else
                {
                    Time.timeScale = 0.0f;
                    Pause_Canvas.gameObject.SetActive(true);
                    paused = true;
                }
            }
        }
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        Pause_Canvas.gameObject.SetActive(false);
        paused = false;
    }

    public void DisablePause()
    {
        blocked = true;
        Pause_Canvas.gameObject.SetActive(false);
    }
}
