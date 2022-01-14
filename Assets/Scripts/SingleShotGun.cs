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
        //ī�޶�κ��� ������ �߻�
        ray.origin = cam.transform.position;
        //�߻����� ī�޶�κ���
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage); //�������� ���� ��ü�� �������� ���� �� �ִ� ��ü�ΰ�? �´ٸ� ������ ������ �´� ������ �ޱ�
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal); // ���߸� �������� �¾Ҵٰ� �˷��ֱ�
        }
    }

    [PunRPC] //Pun Remote Procedure Call �� ���ڷ� ������� ���� �Լ��� �����Ű�� ���
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation); //hitPosition+hitNormal*0.001f�� ���ָ� �Ѿ� �ڱ��� ��� ǥ�麸�� ���� ��¦ ���� �ְ� �Ǿ ���� ������ �ʴ´�.
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
        //hitnormal�� �´¸��� ���ϴ� ����
        
    }
}
