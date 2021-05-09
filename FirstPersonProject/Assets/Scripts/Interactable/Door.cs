﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    IPlayer player;
    public int id = 1;
    public bool needKey = false;
    public string keyName;

    [Header("Enter Zone")]
    public bool enterZone = false;
    public GameObject gate;

    [Header("Change Scene")]
    public string from;
    public string goTo;
    /*
     * Door id
     * 
     * Checkpoint Door = 0
     * SecurityRoom = 1
     * SecurityRoom_Out = 2
     * MedicalRoom = 3
     * MedicalRoom_Out = 4
     * 
     */
    void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<IPlayer>();

        needKey = SaveManager.instance.doorNeedKey[id];
    }

    void Update()
    {
        if (CameraManager.camera != null)
        {
            RaycastHit hit = CameraManager.GetCameraRaycast(player.GetStats().InteractRange);
            if (this.transform != hit.transform)
            {
                ShowUI(false);
            }
            else
            {
                ShowUI(true);
            }
        }
    }
    public void ShowUI(bool show)
    {
        transform.GetChild(0).gameObject.SetActive(show);
    }
    public void OnEnter()
    {
        print("Entering " + gameObject.name);
        if (from == "Level01" && goTo == "MedicalRoom")
        {
             GameManager.instance.LoadMedRoom();
        }
        else if (from == "MedicalRoom" && goTo == "Level01")
        {
            GameManager.instance.LoadMedToLevel01();
        }
        else if (from == "Level01" && goTo == "SecurityRoom")
        {
            GameManager.instance.LoadSecRoom();
        }
        else if(from == "SecurityRoom" && goTo == "Level01")
        {
            GameManager.instance.LoadSecToLevel01();
        }
    }
    public void OnOpen()
    {
        print("Openning " + gameObject.name);
        gate.GetComponent<Animator>().Play("Open");
    }
}
