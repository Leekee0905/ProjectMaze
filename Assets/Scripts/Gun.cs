using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item //������ ��ũ��Ʈ���� ���� �޾ƿ���
{

    public abstract override void Use(); //�θ� Ŭ������ item���� ����� Use�� override�ؼ� ������
    public GameObject bulletImpactPrefab;

}
