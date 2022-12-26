using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Name: Player Controller
/// Description:Handles the player's current weapon loadout
/// Use: Put on the Arms model (child to player object), there can be only one.
/// </summary>

public class PlayerLoadout : MonoBehaviour
{
    int currentSlot;
    int currentWeaponID;
    Animator animator;
    RuntimeAnimatorController defaultAnimator; //unarmed animations, since we'll be switching in overrides frequently.
    [SerializeField] Weapon[] loadout = new Weapon[2];
    [SerializeField] Camera playerCamera;

    /*[SerializeField] int[] viewmodelID;
    [SerializeField] GameObject[] viewmodelObjects;
    public Dictionary<int, GameObject> viewmodels;*/

    int baseDamage;
    float reach;
    bool canAttack;
    int combo;

    private void OnValidate()
    {
        if(loadout.Length != 2)
        {
            Debug.LogWarning("Please do not change the size of the 'loadout' array!");
            Array.Resize(ref loadout, 2);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultAnimator = animator.runtimeAnimatorController;

        /*viewmodels = new Dictionary<int, GameObject>();
        foreach(int i in viewmodelID) viewmodels.Add(i, viewmodelObjects[i]);
        Debug.Log("Creating weapons dictionary...");
        foreach (KeyValuePair<int, GameObject> kvp in viewmodels)
            Debug.Log(kvp);
        Debug.Log("Weapon viewmodel dictionary initialized");*/
    }

    private void Start()
    {
        currentSlot = 0;
        currentWeaponID = -1;
        EvaluateWeapon();
    }

    private void Update()
    {
        if (canAttack)
        {
            if (Input.GetButtonDown("Fire"))
            {
                animator.Play("Attack", 0, 0f);
                animator.SetFloat("Combo", combo);
                combo = (combo + 1) % 4;
            }
        }
    }

    private void EvaluateWeapon()
    {
        if(loadout[currentSlot] == null)
        {
            //we are wielding nothing but our fists, use unarmed combat
            currentWeaponID = -1;
            baseDamage = 10;
            reach = 1f;
            return;
        }

        //check the weapon in the current slot
        currentWeaponID = loadout[currentSlot].weaponID;
        /*if (!viewmodels.ContainsKey(currentWeaponID))
        {
            Debug.LogError("PlayerLoadout.EvaluateWeapon() error: Weapon ID is invalid!");
            currentWeaponID = -1;
            return;
        }*/

        baseDamage = loadout[currentSlot].baseDamage;
        reach = loadout[currentSlot].reach;
    }

    public void SetWeapon(Weapon weapon)
    {
        loadout[currentSlot] = weapon;
    }
    
    public void SetWeapon(Weapon weapon, int slot)
    {
        if(slot != 0 & slot != 1)
        {
            Debug.LogWarning("PlayerLoadout.SetWeapon(Weapon weapon, int slot) error: slot must be 0 or 1");
            return;
        }
        loadout[slot] = weapon;
    }

    public Weapon GetWeapon(int slot)
    {
        if(slot != 0 & slot != 1)
        {
            Debug.LogWarning("PlayerLoadout.GetWeapon(int slot) error: slot must be 0 or 1");
            return null;
        }
        return loadout[slot];
    }

    public bool CompareWeapon(Weapon weapon)
    {
        if (loadout[currentSlot] == weapon) return true;
        return false;
    }

    public bool CompareWeapon(Weapon weapon, bool otherSlot)
    {
        if (loadout[otherSlot == true ? (currentSlot == 0 ? 1 : 0) : currentSlot] == weapon) return true;
        return false;
    }

    public bool CompareWeapon(Weapon weapon, int slot)
    {
        if (slot != 0 & slot != 1)
        {
            Debug.LogWarning("PlayerLoadout.CompareWeapon(Weapon weapon, int slot) error: slot must be 0 or 1");
            return false;
        }
        if (loadout[slot] == weapon) return true;
        return false;
    }

    private void Attack(float damageMultiplier)
    {
        if (loadout[currentSlot] == null || loadout[currentSlot].type == weaponType.Melee)
        {
            canAttack = false;
            RaycastHit[] hits;

            if (loadout[currentSlot] == null)
            {
                //Debug.Log("Attacking unarmed");
                //unarmed cannot deflect bullets and can only hit one target
                hits = Physics.CapsuleCastAll(transform.position, playerCamera.transform.forward, 0.35f, playerCamera.transform.forward, 1f, 6, QueryTriggerInteraction.Ignore);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Player") continue;
                    //apply damage to target, exit after damage is dealt once;
                }
                return;
            }
            //Debug.Log("Attacking with a " + loadout[currentSlot].weaponName);
            if (Physics.CapsuleCast(transform.position, playerCamera.transform.forward * reach, 0.1f, playerCamera.transform.forward, reach, 8, QueryTriggerInteraction.Ignore))
            {
                //deflect boolet
            }
            hits = Physics.CapsuleCastAll(transform.position, playerCamera.transform.forward * reach, 0.35f, playerCamera.transform.forward, reach, 6, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == "Player") continue;
                //apply damage to target;
            }
        }
        else
        {
            //gun
        }
    }

    private void CanAttack()
    {
        //Debug.Log("Can attack again!");
        canAttack = true;
    }

    private void CannotAttack()
    {
        //Debug.Log("Can no longer attack...");
        canAttack = false;
    }

    private void ResetCombo()
    {
        combo = 0;
    }
}
