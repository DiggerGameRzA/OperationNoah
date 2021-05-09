﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private const int itemSlots = 5;
    private const int weaponSlots = 2;
    //public List<IInventoryItem> mItems = new List<IInventoryItem>();

    public List<InventorySlot> iSlots = new List<InventorySlot>();
    public List<InventorySlot> wSlots = new List<InventorySlot>();

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemUsed;
    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public event EventHandler<InventoryEventArgs> WeaponAdded;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            print("Destroy myself");
            Destroy(this.gameObject);
        }
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }
    public Inventory()
    {
        for (int i = 0; i < itemSlots; i++)
        {
            iSlots.Add(new InventorySlot(i));
        }
        for (int i = 0; i < weaponSlots; i++)
        {
            wSlots.Add(new InventorySlot(i));
        }
    }

    private InventorySlot FindStackkableSlot(IInventoryItem item, string type)
    {
        if (type == "Item")
        {
            foreach (InventorySlot slot in iSlots)
            {
                if (slot.IsStackable(item))
                    return slot;
            }
        }
        else if (type == "Weapon")
        {
            return null;
            /*
            foreach (InventorySlot slot in wSlots)
            {
                if (slot.IsStackable(item))
                    return slot;
            }
            */
        }
        return null;
    }
    private InventorySlot FindNextEmptySlot(string type)
    {
        if (type == "Item")
        {
            foreach (InventorySlot slot in iSlots)
            {
                if (slot.IsEmpty)
                    return slot;
            }
        }
        if(type == "Weapon")
        {
            foreach (InventorySlot slot in wSlots)
            {
                if (slot.IsEmpty)
                    return slot;
            }
        }
        return null;
    }
    public IInventoryItem ItemTop(string type)
    {
        IInventoryItem item = null;
        if (type == "Item")
        {
            foreach (InventorySlot slot in iSlots)
            {
                if (slot.Count > 0)
                    item = slot.mItemStack.Peek();
            }
        }
        else if (type == "Weapon")
        {
            foreach (InventorySlot slot in wSlots)
            {
                if (slot.Count > 0)
                    item = slot.mItemStack.Peek();
            }
        }
        return item;
    }
    public void AddItem(IInventoryItem item, string type)
    {
        InventorySlot freeSlot = FindStackkableSlot(item, type);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot(type);
        }
        if (freeSlot != null)
        {
            freeSlot.AddItem(item);
            item.OnPickUp();

            if (type == "Item")
            {
                if (ItemAdded != null)
                {
                    ItemAdded(instance, new InventoryEventArgs(item));
                    Debug.Log("Added Item : " + item.Name);
                }
            }
            else if(type == "Weapon")
            {
                if (WeaponAdded != null)
                {
                    WeaponAdded(instance, new InventoryEventArgs(item));
                    Debug.Log("Added Weapon : " + item.Name);
                }
            }
        }
        /*
          if(mItems.Count < SLOTS)
          {
              Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
              if(collider.enabled)
              {
                  collider.enabled = false;
                  mItems.Add(item);
                  item.OnPickup();
                  if(ItemAdded != null)
                  {
                      ItemAdded(this, new InventoryEventArgs(item));
                  }
              }
          }
         */
    }
    internal void UseItem(IInventoryItem item)
    {
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item));
        }
        item.OnUse();
    }
    public void RemoveItem(IInventoryItem item)
    {
        foreach (InventorySlot slot in iSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(instance, new InventoryEventArgs(item));
                }
                break;
            }
        }
    }
    public IInventoryItem GetPeekItem(int slot, string type)
    {
        slot -= 1;
        IInventoryItem item = null;
        if (wSlots[slot].Count != 0 || iSlots[slot].Count != 0)
        {
            if (type == "Item")
            {
                item = iSlots[slot].mItemStack.Peek();
            }
            else if (type == "Weapon")
            {
                item = wSlots[slot].mItemStack.Peek();
            }
        }
        return item;
    }
    public IInventoryItem FindKeyItem(string name)
    {
        
        IInventoryItem item = null;
        for (int i = 0; i < iSlots.Count; i++)
        {
            if (iSlots[i].mItemStack.Count != 0)
            {
                item = iSlots[i].mItemStack.Peek();
            }
            if(item != null)
            {
                item = iSlots[i].mItemStack.Peek();
                if(item.Name == name)
                {
                    break;
                }
                else
                {
                    item = null;
                }
            }
        }
        if(item != null)
        {
            print("Key is found!");
        }
        else
        {
            print("Key is not found");
        }
        return item;
    }
    public void Restart()
    {

    }
}
