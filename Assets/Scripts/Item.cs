using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{//추상 클래스로 선언
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();
}
