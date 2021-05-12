﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField] IPlayer player;
    [SerializeField] Inventory inventory;
    [SerializeField] UIManager uiManager;
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] HUD hud;

    [Header("Bools")]
    public bool canShoot = true;
    public bool canOpenInv = true;
    public bool canEsc = true;
    public bool canEquipWep = true;
    public bool canMove = true;
    public bool canInteract = true;
    public bool canCollect = true;
    public bool canSkip = false;

    public bool onConversation = false;

    public GameObject checkKey;
    public GameObject medKey;
    public GameObject syringe;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);

        inventory = Inventory.instance;
        cameraManager = CameraManager.instance;
        weaponManager = WeaponManager.instance;
    }
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        player = FindObjectOfType<Player>().GetComponent<IPlayer>();
        uiManager = FindObjectOfType<UIManager>();
    }
    void Update()
    {
        if(canCollect)
            RaycastItem();
        if (canInteract)
        {
            RaycastDoor();
            RaycastSyringe();
            RaycastDNA();
        }

        if (onConversation)
        {
            OnConversation(true);
            if (canSkip)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    GameObject.Find("First Con").GetComponent<NPCDialogue>().DisplayNextSentence();
                }
            }
        }

        if (canOpenInv)
        {
            if (Input.GetButtonDown("Open Inventory"))
            {
                if (!uiManager.GetInventoryVisible())
                {
                    uiManager.ShowInventory(true);
                }
                else
                {
                    uiManager.ShowInventory(false);
                }
            }
            if (Input.GetButtonDown("Cancel"))
            {
                if (uiManager.GetInventoryVisible())
                {
                    uiManager.ShowInventory(false);
                }
            }
        }

        if (canEsc)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (!uiManager.GetMenuVisible())
                {
                    uiManager.ShowMenu(true);
                }
                else if (uiManager.GetMenuVisible())
                {
                    uiManager.ShowMenu(false);
                }
            }
        }

        if (canEquipWep)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeaponInSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeaponInSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeaponInSlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                EquipWeaponInSlot(3);
            }
        }

        if (canShoot)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (player.GetWeapon() != null)
                {
                    weaponManager.Fire();
                }
            }
            else if (Input.GetButton("Fire1"))
            {
                if (player.GetWeapon() != null)
                {
                    if (player.GetWeapon().WeaponType == WeaponEnum.AssaultRifle)
                    {
                        weaponManager.Fire();
                    }
                }
            }
            else if (Input.GetButtonDown("Reload"))
            {
                if (player.GetWeapon() != null)
                    weaponManager.Reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            player.GetHealth().TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.AddItem(syringe.GetComponent<IInventoryItem>(), "Item");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            inventory.AddItem(medKey.GetComponent<IInventoryItem>(), "Item");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.AddItem(checkKey.GetComponent<IInventoryItem>(), "Item");
        }
    }
    public static float GetVerInput()
    {
        return Input.GetAxis("Vertical");
    }
    public static float GetHorInput()
    {
        return Input.GetAxis("Horizontal");
    }
    private void RaycastItem()
    {
        player = FindObjectOfType<Player>();
        RaycastHit hit = CameraManager.GetCameraRaycast(player.GetStats().InteractRange);
        if (hit.transform)
        {
            if (Input.GetButtonDown("Collect"))
            {
                IInventoryItem item = hit.transform.GetComponent<IInventoryItem>();
                if(hit.transform.CompareTag("Ammo"))
                {
                    item.Collected = true;
                    SaveManager.instance.collected[item.Id] = true;
                    if (hit.transform.GetComponent<IInventoryItem>().Name == "Ammo 9mm")
                    {
                        WeaponManager.instance.GetComponent<Pistol>().CurrentSpare += item.Amount;
                    }
                    if (SaveManager.instance.currentWeapon == WeaponEnum.Pistol)
                    {
                        UIManager.instance.UpdateAmmo(WeaponManager.instance.GetComponent<Pistol>().CurrentAmmo, WeaponManager.instance.GetComponent<Pistol>().CurrentSpare);
                    }
                    item.OnPickUp();
                }
                else if (hit.transform.CompareTag("Item"))
                {
                    item.Collected = true;
                    SaveManager.instance.collected[item.Id] = true;
                    inventory.AddItem(item, "Item");
                }
                else if (hit.transform.CompareTag("Weapon"))
                {
                    item.Collected = true;
                    SaveManager.instance.collected[item.Id] = true;
                    inventory.AddItem(item, "Weapon");
                }
            }
        }
    }
    private void RaycastDoor()
    {
        RaycastHit hit = CameraManager.GetCameraRaycast(player.GetStats().InteractRange);
        if (hit.transform)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (hit.transform.CompareTag("Interactable"))
                {
                    Door door = hit.transform.GetComponent<Door>();
                    if (door != null)
                    {
                        if (!door.needKey)
                        {
                            if (door.enterZone)
                                door.OnOpen();
                            else
                                door.OnEnter();
                        }
                        else if (door.needKey)
                        {
                            if (inventory.FindKeyItem(door.keyName) != null)
                            {
                                inventory.RemoveItem(inventory.FindKeyItem(door.keyName));
                                door.needKey = false;
                                SaveManager.instance.UnlockDoor(door.id);
                                if(door.id == 0)
                                {
                                    if (SaveManager.instance.firstTimeEvent[3])
                                    {
                                        FindObjectOfType<EventTrigger>().StartCon3();
                                    }
                                }

                                if (door.enterZone)
                                    door.OnOpen();
                                else
                                    door.OnEnter();
                            }
                            else
                            {
                                uiManager.UpdateSubtitle("You need " + door.keyName + " to enter");
                            }
                        }
                    }
                }
            }
        }
    }
    private void RaycastSyringe()
    {
        player = FindObjectOfType<Player>();
        inventory = Inventory.instance;
        RaycastHit hit = CameraManager.GetCameraRaycast(player.GetStats().InteractRange);
        if (hit.transform)
        {
            if (hit.transform.CompareTag("Dino"))
            {
                float health = hit.transform.GetComponent<Health>().HealthPoint;
                float sedat = hit.transform.GetComponent<SedatPoint>().SedatPoints;

                if (health <= 0 || sedat <= 0)
                {
                    //GameObject text = hit.transform.FindChild("HP Canvas").GetChild(1).gameObject;
                    //hit.transform.GetComponent<GatherSyringe>().ShowUI(text);

                    if (Input.GetButtonDown("Interact"))
                    {
                        IInventoryItem item = inventory.FindKeyItem("Syringe Empty");
                        if (item != null)
                        {
                            print("Getherting DNA");
                            GameObject dna = hit.transform.GetComponent<GatherSyringe>().dna;

                            inventory.RemoveItem(item);
                            inventory.AddItem(dna.GetComponent<IInventoryItem>(), "Item");
                        }
                        else
                        {
                            print("You don't have Syringe");
                        }
                    }
                }
            }
        }
    }
    private void RaycastDNA()
    {
        RaycastHit hit = CameraManager.GetCameraRaycast(player.GetStats().InteractRange);
        if (hit.transform)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (hit.transform.CompareTag("Interactable"))
                {
                    DNAReader gate = hit.transform.GetComponent<DNAReader>();
                    if (gate != null)
                    {
                        if (gate.needDNA)
                        {
                            if (inventory.FindKeyItem(gate.dnaName) != null)
                            {
                                inventory.RemoveItem(inventory.FindKeyItem(gate.dnaName));
                                gate.gate.GetComponent<Door>().needKey = false;
                                gate.needDNA = false;
                                SaveManager.instance.doorNeedKey[gate.gate.GetComponent<Door>().id] = false;
                                SaveManager.instance.gateNeedDNA[gate.id] = false;
                            }
                            else
                            {
                                uiManager.UpdateSubtitle("You need " + gate.dnaName + " to enter");
                            }
                        }
                    }
                }
            }
        }
    }
    public void EquipWeaponInSlot(int slot)
    {
        inventory = Inventory.instance;
        weaponManager = WeaponManager.instance;
        player = FindObjectOfType<Player>();

        IInventoryItem item = inventory.GetPeekItem(slot, "Weapon");
        if (item != null && item.Weapon != WeaponEnum.None)
        {
            player.EquipWeapon(item.Weapon);
            weaponManager.currentSlot = slot;
        }
        else
        {
            Debug.Log("There is no weapon in slot : " + slot);
        }
    }
    public void OnConversation(bool talking)
    {
        onConversation = talking;
        canOpenInv = !talking;
        canShoot = !talking;
        canEquipWep = !talking;
        canMove = !talking;
        canInteract = !talking;
        canCollect = !talking;
        CameraManager.instance.CanLookAround(!talking);
    }
    public void Restart()
    {
        player = FindObjectOfType<Player>().GetComponent<IPlayer>();
        uiManager = FindObjectOfType<UIManager>();
    }
}
