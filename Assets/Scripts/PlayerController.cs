using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    AudioSource audioPlayer;
    public AudioClip pointsSound, jumpSound, deadSound;
    public Text timeText, lifesText, finalLifesText, starsText, finalStarsText;
    float movVertical, movHorizontal;
    int stars = 0; int lifes = 3;
    public bool isJump = false;
    bool pause = false;
    bool endgame = false;
    public float velocidad = 1.0f;
    public float altitud = 100.0f;
    float totalTime = 120f;
    public MenuManager menuManager;
    public GameObject startPoint, panelGameOver, panelCongratulations, uiMobile;
    public bl_Joystick joystick;
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
            uiMobile.SetActive(true);
        #endif
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pause)
        {
            CountDown();
        }

        #if UNITY_ANDROID
            movVertical = joystick.Vertical * 0.12f;
            movHorizontal = joystick.Horizontal * 0.12f;
        #else
            // Obtengo los input del teclado
            movVertical = Input.GetAxis("Vertical");
            movHorizontal = Input.GetAxis("Horizontal");
        #endif

        // Creo mi vector de movimiento para mi player
        Vector3 movimiento = new Vector3(movHorizontal, 0.0f, movVertical);

        // Agregamos fuerza al cuerpo rigido
        rb.AddForce(movimiento * velocidad);

        if (Input.GetKey(KeyCode.Space) && !isJump)
        {
            Jump();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Floor" || collision.gameObject.name == "Wood")
        {
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "Star")
        {
            Destroy(collider.gameObject);
            stars += 1;
            starsText.text = "0" + stars.ToString();
            GetComponent<AudioSource>().clip = pointsSound;
            GetComponent<AudioSource>().Play();
        }

        if (collider.gameObject.name == "DeadZone" || collider.gameObject.name == "Axe")
        {
            transform.position = startPoint.transform.position;
            lifes -= 1;
            lifesText.text = "0" + lifes.ToString();
            GetComponent<AudioSource>().clip = deadSound;
            GetComponent<AudioSource>().Play();

            if (lifes == 0)
            {
                GameOver();
            }
        }

        if (collider.gameObject.name == "Final" && !endgame)
        {
            FinishedGame();
        }
    }

    void CountDown()
    {
        totalTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime - (minutes * 60));
        timeText.text = string.Format("{0:0}:{01:00}", minutes, seconds);

        if (minutes == 0 && seconds == 0)
        {
            GameOver();
        }
    }

    public void PauseGame()
    {
        pause = !pause;
        rb.isKinematic = pause;
    }

    public void RestartGame()
    {
        transform.position = startPoint.transform.position;
        totalTime = 120f;
        lifes = 3;
        stars = 0;
        lifesText.text = "03";
        starsText.text = "00";
        rb.isKinematic = false;
        pause = false;
        endgame = false;
    }

    void FinishedGame()
    {
        finalLifesText.text = "0" + lifes.ToString();
        finalStarsText.text = "0" + stars.ToString();
        menuManager.GoToMenu(panelCongratulations);
        endgame = true;
        PauseGame();
    }

    void GameOver()
    {
        menuManager.GoToMenu(panelGameOver);
        PauseGame();
    }

    public void Jump()
    {
        if (!isJump)
        {
            Vector3 salto = new Vector3(0, altitud, 0);
            rb.AddForce(salto * velocidad);
            isJump = true;
            GetComponent<AudioSource>().clip = jumpSound;
            GetComponent<AudioSource>().Play();
        }
    }
}
