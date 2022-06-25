using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject DeathCanvas;

    public GameObject[] enemys;
    public PlayerLogic player;
    public Vector2 playerSpawn;

    private void Start()
    {
        this.ResetState();
    }

    public void Update()
    {
        
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
        }

        //Enable player movement
        this.player.InputAllowed = true;
    }

    public void ShowDeathScreen()
    {
        //Disable all enemys
        for (int i = 0; i < this.enemys.Length; i++)
        {
            this.enemys[i].SetActive(false);
        }

        //Disable player movement
        this.player.InputAllowed = false;

        this.DeathCanvas.SetActive(true);
    }

    public void ResetState()
    {
        //Remove DeathCanvas
        this.DeathCanvas.SetActive(false);
        
        //Enable all enemys
        for (int i = 0; i < this.enemys.Length; i++)
        {
            this.enemys[i].SetActive(true);
        }

        //Reset player
        this.player.ResetState();
    }
}