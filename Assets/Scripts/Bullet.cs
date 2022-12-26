using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Name: Bullet
/// Description: Controls behavior of bullets.
/// 
/// TODO:
/// -!!!design with object pooling in mind!!!
/// -ricochets?
/// -deal damage to hit targets, once that can be done
/// -make sure the player can't run into and get damaged by
///     their own bullets immediately after firing them
/// -keep list of characters hit so one character isn't hit several times(?)
/// </summary>

public class Bullet : MonoBehaviour
{
    [SerializeField] int baseDamage;
    [SerializeField] float speed;
    [SerializeField] int penetration;

    BoxCollider collBox;
    BoxCollider hurtBox;
    Rigidbody rb;

    private void Awake()
    {
        collBox = GetComponent<BoxCollider>();
        hurtBox = GetComponentInChildren<BoxCollider>();
        rb = collBox.attachedRigidbody;
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        rb.velocity = Vector3.zero; //so that speed doesn't add up for some reason.
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            //do damage
        }
    }
}
