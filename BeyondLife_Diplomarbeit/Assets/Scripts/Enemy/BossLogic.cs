using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLogic : MonoBehaviour
{
    public BasicEnemy main;

    void Update()
    {
        if (this.main.health <= 0)
        {
            Invoke("startCredits", 2.0f);
        }
    }

    private void startCredits()
    {
        SceneManager.LoadScene("Settings");
    }
}
