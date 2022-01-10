using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;

    public void SetUp(Player _player) // 플레이어 이름 받아서 그 사람 이름이 목록에 뜨게 만듦
    {
        player = _player;
        text.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //플레이어가 방을 떠났을때
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() //방 나갔을때
    {
        Destroy(gameObject); //이름표 호출
    }
}
