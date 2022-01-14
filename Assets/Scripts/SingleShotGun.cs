using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
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
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage); //레이저에 맞은 물체가 데미지를 입을 수 있는 물체인가? 맞다면 아이템 정보에 맞는 데미지 받기
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal); // 맞추면 상대방한테 맞았다고 알려주기
        }
    }

    [PunRPC] //Pun Remote Procedure Call 의 약자로 원격제어를 통해 함수를 실행시키는 기능
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation); //hitPosition+hitNormal*0.001f를 해주면 총알 자국이 대상 표면보다 아주 살짝 위에 있게 되어서 겹쳐 보이지 않는다.
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        //hitnormal은 맞는면이 향하는 방향
        
    }
}
