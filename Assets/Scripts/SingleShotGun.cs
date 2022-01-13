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
        //카메라로부터 레이저 발사
        ray.origin = cam.transform.position;
        //발사지점 카메라로부터
        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("We hit " + hit.collider.gameObject.name); //레이저에 뭔가 맞으면 디버그
        }
    }
}
