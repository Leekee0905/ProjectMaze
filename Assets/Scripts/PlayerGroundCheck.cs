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
            return; //�ش� ��ü�� player�� ����
        playerController.SetGroundedState(true); //������ true
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false); //������ false
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }
}
