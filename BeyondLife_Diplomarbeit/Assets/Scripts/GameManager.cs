using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject DeathCanvas;
    public GameObject PauseCanvas;

    [Header("Level Stats")]
    public GameObject[] enemys;
    public PlayerLogic player;
    public Vector2 playerSpawn;
    private bool paused = false;

    [Header("UI Input")]
    public UIControls inputControls;
    public InputAction pauseMenue{ get; private set; }
    private float nextAction = 0f;
    private Vector2 prevMovement = Vector2.zero;

    private void Start()
    {
        this.ResetState();
        this.inputControls = new UIControls();
        this.pauseMenue = this.inputControls.UI.PauseMenue;
        this.pauseMenue.Enable();
    }

    public void Update()
    {
        if (this.pauseMenue.ReadValue<float>() == 1 && Time.time > this.nextAction)
        {
            this.nextAction = Time.time + 0.2f;
            ShowPauseMenue();
        }
    }

    public void StartLevel()
    {
        //ToDO
        /*
        Start Timer
        Display Score

        */

        //Enable all enemys
        for (int i = 0; i < this.enemys.Length; i++)
        {
            this.enemys[i].SetActive(true);
            this.enemys[i].BroadcastMessage("ResetState");      //Reset Enemy without script name
        }

        //Enable player movement
        this.player.InputAllowed = true;
    }

    public void ShowDeathScreen()
    {
        //Disable all enemys
        for (int i = 0; i < this.enemys.Length; i++)
        {
            if (this.enemys[i] != null)
            {
                this.enemys[i].SetActive(false);
            }
        }

        //Disable player movement
        this.player.InputAllowed = false;
        this.player.rigidBody.velocity = Vector2.zero;

        this.DeathCanvas.SetActive(true);
    }

    public void ShowPauseMenue()
    {
        this.paused = !this.paused;
        if (this.paused)
        {
            //Disable all enemys
            for (int i = 0; i < this.enemys.Length; i++)
            {
                if (this.enemys[i] != null)
                {
                    this.enemys[i].SetActive(false);
                }
            }

            //Disable player movement
            this.player.InputAllowed = false;
            this.player.rigidBody.isKinematic = true;
            this.prevMovement = this.player.rigidBody.velocity;
            this.player.rigidBody.velocity = Vector2.zero;

            //Enable Canvas
            this.PauseCanvas.SetActive(true);
        }
        else
        {
            //Enable all enemys
            for (int i = 0; i < this.enemys.Length; i++)
            {
                if (this.enemys[i] != null)
                {
                    this.enemys[i].SetActive(true);
                }
            }

            //Enable player movement
            this.player.InputAllowed = true;
            this.player.rigidBody.isKinematic = false;
            this.player.rigidBody.velocity = this.prevMovement;

            //Disable Canvas
            this.PauseCanvas.SetActive(false);
        }
    }

    public void ResetState()
    {
        //Remove DeathCanvas
        this.DeathCanvas.SetActive(false);
        
        //Enable all enemys
        for (int i = 0; i < this.enemys.Length; i++)
        {
            this.enemys[i].SetActive(true);
            this.enemys[i].BroadcastMessage("ResetState");      //Reset Enemy without script name
        }

        //Reset player
        this.player.ResetState();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        this.pauseMenue.Disable();
    }
}