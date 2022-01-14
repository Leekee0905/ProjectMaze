using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;//포톤 기능 사용
using TMPro; // 텍스트 매쉬 프로 사용
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks//다른 포톤 반응 받아들이기
{
    public static Launcher Instance; //Launcher 스크립트를 메서드로 사용하기 위함

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();//설정한 포톤 서버에 때라 마스터 서버에 연결
    }

    public override void OnConnectedToMaster()//마스터서버에 연결시 작동됨
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();//마스터 서버 연결시 로비로 연결
        PhotonNetwork.AutomaticallySyncScene = true; //자동으로 모든 사람들의 씬을 통일 시켜줌
    }

    public override void OnJoinedLobby()//로비에 연결시 작동
    {
        MenuManager.Instance.OpenMenu("title");//로비에 들어오면 타이틀 메뉴 키기
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000"); //들어온 사람 이름 랜덤으로 숫자 붙여주기
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return; //방 이름이 빈 값이면 안만들어짐
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text); //포톤 네트워크 기능으로 roomNameInputField.text의 이름으로 방만듦
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom() // 방에 들어갔을 때 작동
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; //들어간 방 이름 표시
        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject); //방에 들어가면 전에 있던 이름들 삭제
        }
        for(int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]); //내가 방에 들어가면 방에 있는 사람 목록 만큼 이름표 뜨게하기
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); //방장만 게임시작 버튼 누르기 가능
    }

    public override void OnMasterClientSwitched(Player newMasterClient)//방장이 나가서 방장이 바뀌었을때
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //방 만들기 실패시 작동
    {
        errorText.text = ("Room Creation Failed: " +message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // 1인 이유는 빌드에서 scene번호가 1번씩 이기 때문이다. 0번은 초기 씬
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // 방 나가기포톤 네트워크 기능
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name); //포톤 네트워크의 JoinRoom기능 해당 이름을 가진 방으로 접속
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()//방을 떠나면 호출
    {
        MenuManager.Instance.OpenMenu("title"); //방떠나면 타이틀 메뉴호출
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 포톤의 룸 리스트 기능
    {
        foreach(Transform trans in roomListContent) //존재하는 모든 roomListContent
        {
            Destroy(trans.gameObject); //룸리스트 업데이트가 될 때 마다 다 지우기
        }
        for (int i = 0; i < roomList.Count; i++) //방 개수 만큼 반복
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]); //Instantiate로 prefab을 roomListContent위치에 만들어 주고 그 프리펩은 i번째 룸리스트가 된다.
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //다른 플레이어가 방에 들어오면 작동
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer); //Instatiate로 prefab을 playerlistcontent위치에 만들어주고 그 프리펩 이름을 받아서 표시
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}