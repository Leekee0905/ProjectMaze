using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return; //해당 물체가 player면 무시
        playerController.SetGroundedState(true); //닿으면 true
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false); //닿으면 false
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }
}
