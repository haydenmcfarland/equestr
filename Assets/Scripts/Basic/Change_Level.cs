using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Change_Level : MonoBehaviour {

    // main_controller is the game object that has the Fader script
    public GameObject main_controller;
    private IEnumerator transition;

    private const int FADE_IN = -1;
    private const int FADE_OUT = 1;

    IEnumerator FadeOutTransition(string destination)
    {
        // wait for fade to complete
        Cursor.lockState = CursorLockMode.Locked;
        while (!main_controller.GetComponent<Fader>().FadeComplete())
            yield return null;
        SceneManager.LoadScene(destination);
    }

    IEnumerator FadeInTransition()
    {
        // wait for fade to complete
        Cursor.lockState = CursorLockMode.Locked;
        while (!main_controller.GetComponent<Fader>().FadeComplete())
            yield return null;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadScene(string destination)
    {
        // fade-out to load a new scene
        if (main_controller.GetComponent<Fader>().FadeComplete())
        {
            main_controller.GetComponent<Fader>().Fade(FADE_OUT);
            transition = FadeOutTransition(destination);
            StartCoroutine(transition);
        }
    }

    private void Start()
    {
        main_controller.GetComponent<Fader>().Fade(FADE_IN);
        transition = FadeInTransition();
        StartCoroutine(transition);
    }
}
