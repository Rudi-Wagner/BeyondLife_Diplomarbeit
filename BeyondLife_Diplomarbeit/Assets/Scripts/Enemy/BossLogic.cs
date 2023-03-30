using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLogic : MonoBehaviour
{
    void OnDisable()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
