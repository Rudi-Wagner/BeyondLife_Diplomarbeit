using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject DeathCanvas;
    public GameObject PauseCanvas;
    public GameObject toolbar;

    [Header("Level Stats")]
    public GameObject[] enemys;
    public GameObject[] collectibles;
    public PlayerLogic player;
    public Vector2 playerSpawn;
    private bool paused = false;
    private Scene scene;

    [Header("UI Input")]
    public UIControls inputControls;
    public InputAction pauseMenue{ get; private set; }
    private float nextAction = 0f;
    private Vector2 prevMovement = Vector2.zero;

    private void Start()
    {
        this.scene = SceneManager.GetActiveScene();
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

        updateToolbar(this.player.currentSelected);
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
        this.player.animate.speed = 0;

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

    public void MoveToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void updateToolbar(float selection)
    {
        int cnt = 0;
        //Set the color of each toolbar Weapon
        foreach (Transform t in this.toolbar.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name.StartsWith("Weapon_")) 
            {
                //Draw normal Higlight
                if (t.gameObject.name == "Weapon_" + selection)
                {
                    t.gameObject.GetComponent<Image>().color = new Color32(0x41, 0x41, 0x41, 0xFF);
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = new Color32(0x2B, 0x2B, 0x2B, 0xFF);
                }

                //Draw ammunition count
                Transform ammo = t.GetChild(2);
                TextMeshProUGUI tmpAmmunitionCnt = ammo.GetComponent<TextMeshProUGUI>();
                int remainingBullets = this.player.weapons[cnt].ammunition;
                if(remainingBullets == 360)
                {
                    tmpAmmunitionCnt.text = "∞";
                }
                else
                {
                    tmpAmmunitionCnt.text = remainingBullets.ToString();
                    if (remainingBullets <= 0)
                    {
                        //Draw empty Magazine Highlight
                        if (t.gameObject.name == "Weapon_" + selection)
                        {
                            t.gameObject.GetComponent<Image>().color = new Color32(0x96, 0x41, 0x41, 0xFF);
                        }
                        else
                        {
                            t.gameObject.GetComponent<Image>().color = new Color32(0x72, 0x1E, 0x1E, 0xFF);
                        }
                    }
                }
                cnt++;
            }
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

        //Reset Weapons
        for (int i = 0; i < this.player.weapons.Length; i++)
        {
            int resetTo = this.player.weapons[i].maxAmmunition;
            if (resetTo <= 0)
            {
                resetTo = 360;
            }
            this.player.weapons[i].ammunition = resetTo;
        }

        //Spawn Collectibles
        for (int i = 0; i < this.collectibles.Length; i++)
        {
            //TO:DO  Needs to be set in advance (at game start?)
            int collectibleState = PlayerPrefs.GetInt(scene.name + "CollectibleID" + i);
            if (collectibleState == 1)
            {
                this.collectibles[i].SetActive(true);
                PlayerPrefs.SetInt(scene.name + "CollectibleID" + i, collectibleState);
            }
            else
            {
                this.collectibles[i].SetActive(false);
                PlayerPrefs.SetInt(scene.name + "CollectibleID" + i, collectibleState);
            }
        }
    }


    private void OnDisable()
    {
        this.pauseMenue.Disable();
    }
}