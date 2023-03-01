using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthbar : MonoBehaviour
{
    public GameObject[] hearts;
    private float maxHealth;
    private float devider;

    void Start()
    {
        PlayerLogic player = GameObject.Find("Player").GetComponent<PlayerLogic>();
        maxHealth = player.maxHealth;
        devider = maxHealth / hearts.Length;
    }

    public void handle(float health)
    {
        foreach(GameObject heart in hearts )
        {
            if(health - devider >= 0)
            {
                heart.SetActive(true);
                health -= devider;
            }
            else
            {
                heart.SetActive(false);
            }
        }
    }
}
