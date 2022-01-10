using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public RoomInfo info; // 포톤 리얼타임의 방정보 기능

    public void SetUp(RoomInfo _info)//방 정보 가져오기
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info); // 런처스크립트 메서드로 JoinRoom 실행
    }
}
