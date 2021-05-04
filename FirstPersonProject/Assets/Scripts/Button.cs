﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadCutscene(int scene)
    {
        GameManager.instance.LoadCutscene(scene);
    }
    public void LoadGame(int scene)
    {
        GameManager.instance.LoadGame(scene);
    }
}
