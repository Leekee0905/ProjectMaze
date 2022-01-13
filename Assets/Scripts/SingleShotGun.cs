using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    public override void Use()
    {
        Debug.Log("using gun " + itemInfo.itemName);
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //ī�޶�κ��� ������ �߻�
        ray.origin = cam.transform.position;
        //�߻����� ī�޶�κ���
        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("We hit " + hit.collider.gameObject.name); //�������� ���� ������ �����
        }
    }
}
