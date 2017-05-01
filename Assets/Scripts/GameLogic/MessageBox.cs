using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageBox : MonoBehaviour {
    /*
     *  This script serves as the primary game logic.
     *  MessageBox was originally only going to be implemented in here,
     *  but most of the game can be controlled through here completely.
     *  May consider breaking things down if I come back to this.
     *  
     *  This script enables the following systems:
     *       Messagebox System
     *          - Horse Portraits
     *          - Typewriter Style Text Display
     *          - .txt script processing
     *          
     *  Some things could be refined, but things work completely fine
     *  and the code is still understandable.
     *  
    */

    // Variables to pass-in before using script
    public GameObject gc;
    public Canvas game_over_screen;
    public Canvas win_screen;
    public Slider slider;
    public Text health_description;
    public TextAsset scene_dialogue;
    public Image hero_image;
    public Image char_op_image;
    public Text name_ui;
    public Text dialogue_ui;
    public Text enter_ui;
    public Text choiceA_ui;
    public Text choiceB_ui;

    // Music and Sound Variables

    public AudioSource bleep;
    public AudioSource okay_music;
    public AudioSource danger_music;
    public AudioSource victory_music;

    // Win Screen Variables
    public Text win_statement_ui;
    public Text score_ui;
    private bool perfect_score = true;

    // Private Variables for Text Processing
    public float speed = 0.025f;
    private string last_named = "";
    private string[] lines;
    private string text_on_screen = "";
    private string full_text = "";
    private float timeElapsed = 0;
    private int i = 0;
    private int incr = 0;

    //HealthBar System
    private int health = 100;

    // Choice System Variables
    private bool choice_state = false;
    private bool win_state = false;
    private bool lose_state = false;
    private bool[] option_values = { false, false };
    private string[] text_options = { "temp", "temp"};
    private int choice = -1;

    // Holds the image corresponding the respective character name (this will be more useful when game is expanded)
    private Dictionary<string, Image> map = new Dictionary<string, Image>();

    private void Start()
    {
        // Initial parameters should be set to false and certains values changed to empty placeholders      
        enter_ui.text = "";
        name_ui.text = "";
        dialogue_ui.text = "";
        choiceA_ui.text = "";
        choiceB_ui.text = "";
        choiceA_ui.enabled = false;
        choiceB_ui.enabled = false;
        choiceA_ui.transform.FindChild("Highlight").gameObject.SetActive(false);
        choiceB_ui.transform.FindChild("Highlight").gameObject.SetActive(false);
        hero_image.enabled = false;
        char_op_image.enabled = false;

        // Set Health Value
        slider.value = health;

        // Fetch lines from level script
        lines = scene_dialogue.ToString().Split('\n');

        // Block other screens
        game_over_screen.gameObject.SetActive(false);
        win_screen.gameObject.SetActive(false);
    }

    void Update()
    {
        if (health > 100)
            health = 100;

        if (!lose_state && !win_state)
        {
            foreach (KeyValuePair<string, Image> entry in map)
            {
                if (entry.Key != last_named)
                    entry.Value.color = new Color(0.3f, 0.3f, 0.3f);
                else
                    entry.Value.color = new Color(1.0f, 1.0f, 1.0f);
            }

            // CHOICE STATE SWITCHES AND KEY CHECKS
            if (choice_state && Input.GetKeyDown("right") && !gc.GetComponent<Pause>().Paused())
            {
                choiceA_ui.transform.FindChild("Highlight").gameObject.SetActive(false);
                choiceB_ui.transform.FindChild("Highlight").gameObject.SetActive(true);
                choice = 1;
            }
            else if (choice_state && Input.GetKeyDown("left") && !gc.GetComponent<Pause>().Paused())
            {
                choiceA_ui.transform.FindChild("Highlight").gameObject.SetActive(true);
                choiceB_ui.transform.FindChild("Highlight").gameObject.SetActive(false);
                choice = 0;
            }
            else if (choice_state && Input.GetKeyDown("return") && !gc.GetComponent<Pause>().Paused())
            {
                if (option_values[choice])
                {
                    if (health != 100)
                    {
                        int val = Random.Range(5, 15);
                        health += val;
                        full_text = "Good move! Gain " + val.ToString() + " ATTRACTION points!";
                    }
                    else
                        full_text = "Good move!";                   
                }
                else
                {
                    if (perfect_score)
                        perfect_score = false;
                    int val = Random.Range(25, 40);
                    health -= val;
                    if (health <= 0)
                        full_text = "You lost all your ATTRACTION points! BAD END.";
                    else
                        full_text = "Bad move! Lose " + val.ToString() + " ATTRACTION points!";
                }

                // reset necessary variables
                incr = 0;
                text_on_screen = "";
                timeElapsed = 0.0f;
                slider.value = health;
                choice_state = false;
                choiceA_ui.enabled = false;
                choiceB_ui.enabled = false;
                option_values[0] = false;
                option_values[1] = false;
                choiceA_ui.transform.FindChild("Highlight").gameObject.SetActive(false);
                choiceB_ui.transform.FindChild("Highlight").gameObject.SetActive(false);

                // increment through lines
                //i += 1;
                lines[i] = text_options[choice];
            }

            // If the text_on_screen has finished, now check if player wants to continue further
            if (text_on_screen == full_text)
            {

                // This option will reveal itself if all fade operations are finished
                if (gc.GetComponent<Fader>().FadeComplete() && !choice_state)
                    enter_ui.text = "[Press Enter]";

                // Pressing 'enter/return' only works if a fading processing is complete
                if (Input.GetKeyDown("return") && gc.GetComponent<Fader>().FadeComplete() && !choice_state && !gc.GetComponent<Pause>().Paused())
                {
                    if (i >= lines.Length)
                        win_state = true;

                    if (health <= 0)
                        lose_state = true;

                    // reset processing parameters
                    if (!win_state && !lose_state)
                    {
                        text_on_screen = "";
                        timeElapsed = 0.0f;
                        enter_ui.text = "";
                        incr = 0;
                        ProcessScene();
                    }
                }
            }

            // grab the time delta
            timeElapsed += Time.deltaTime;

            // modulate the letter display speed
            if (timeElapsed >= speed)
            {
                incr += 1;
                timeElapsed = 0;
            }

            // Check to see how far the text has been processed so far
            if (incr >= full_text.Length)
            {
                text_on_screen = full_text;
                incr = full_text.Length;
            }
            else
            {
                text_on_screen = full_text.Substring(0, incr);
                if (incr % 3 == 0 && !gc.GetComponent<Pause>().Paused())
                    bleep.Play();
            }


            dialogue_ui.text = text_on_screen;

            // Health Bar System Animations and Music Changes
            if (health >= 30 && health <= 70)
            {
                health_description.GetComponent<Floater>().speed = 20.0f;
                if (!okay_music.isPlaying)
                    okay_music.Play();
                if (danger_music.isPlaying)
                    danger_music.Stop();
            }
            else if (health < 30)
            {
                if (!danger_music.isPlaying)
                {
                    okay_music.Stop();
                    danger_music.Play();
                }

                health_description.GetComponent<Floater>().speed = 50.0f;


            }
            else if (health > 70)
            {
                health_description.GetComponent<Floater>().speed = 2.0f;
                if (!okay_music.isPlaying)
                    okay_music.Play();
            }
        }
        else if (lose_state)
        {
            if (!game_over_screen.isActiveAndEnabled)
            {
                gc.GetComponent<Pause>().blocked = true;
                game_over_screen.gameObject.SetActive(true);
            }
        }
        else
        {
            if (perfect_score)
            {
                score_ui.text = "SCORE: PERFECT";
                win_statement_ui.text = "you are a stallion!";
            }
            else if (health <= 100 && health >= 70)
            {
                score_ui.text = "SCORE: GREAT";
                win_statement_ui.text = "you are a clydesdale!";
            }
            else if (health <= 69 && health >= 50)
            {
                score_ui.text = "SCORE: AVERAGE";
                win_statement_ui.text = "you are an equus caballus!";
            }
            else if (health <= 49 && health >= 30)
            {
                score_ui.text = "SCORE: SO-SO";
                win_statement_ui.text = "donkeys need love too!";
            }
            else if (health <= 29)
            {
                score_ui.text = "SCORE: POOR";
                win_statement_ui.text = "no glue factory today!";
            }

            if (okay_music.isPlaying)
                okay_music.Stop();
            if (danger_music.isPlaying)
                danger_music.Stop();
            if (!victory_music.isPlaying)
                victory_music.Play();
            if (!win_screen.isActiveAndEnabled)
            {
                gc.GetComponent<Pause>().blocked = true;
                win_screen.gameObject.SetActive(true);
            }
                
        }            
    }

    // Scene processing
    void ProcessScene()
    {
        // Go through each line to process during update cycles
        if (i < lines.Length)
        {
            if (lines[i][0] != '*')
            {
                // split the process string into name,speech
                string[] to_process = lines[i].Split('&');

                // if the map doesn't contain a character so far, lets add it in and store its image
                if (!map.ContainsKey(to_process[0]))
                {
                    Image image = Instantiate<Image>(hero_image);
                    image.transform.SetParent(hero_image.transform.parent, false);

                    if (to_process[0] != "Horse-Senpai")
                    {
                        image.transform.Rotate(0.0f, -180.0f, 0.0f);
                        image.transform.position = char_op_image.transform.position;
                    }

                    map.Add(to_process[0], image);
                    map[to_process[0]].enabled = true;
                    map[to_process[0]].name = to_process[0] + "_Image";
                }
                // set the new name in the textbox
                name_ui.text = to_process[0];

                if (i < lines.Length)
                    full_text = to_process[1];
                i += 1;

                last_named = to_process[0];
            }
            // Choice-System
            else if (lines[i][0] == '*')
            {
                string[] to_process = lines[i].Substring(1).Split('*');
                if (to_process[0] == "1")
                    option_values[0] = true;
                else
                    option_values[1] = true;

                choiceA_ui.enabled = true;
                choiceB_ui.enabled = true;
                choiceA_ui.transform.FindChild("Highlight").gameObject.SetActive(true);
                choice = 0;
                choiceA_ui.text = to_process[1];
                choiceB_ui.text = to_process[2];
                text_options[0] = to_process[3];
                text_options[1] = to_process[4];
                last_named = "";
                dialogue_ui.text = "";
                name_ui.text = "";
                full_text = "";
                choice_state = true;
            }
        }            
    }
}
