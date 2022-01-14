using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item //아이템 스크립트에서 정보 받아오기
{

    public abstract override void Use(); //부모 클래스인 item에서 사용한 Use를 override해서 재정의
    public GameObject bulletImpactPrefab;

}
