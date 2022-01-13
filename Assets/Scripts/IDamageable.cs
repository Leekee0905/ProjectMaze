using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //인터페이스는 추상 클래스와 같이 정보를 저장하는 용
{
    void TakeDamage(float damage);
}
