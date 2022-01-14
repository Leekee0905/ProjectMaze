using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;//���� ��� ���
using TMPro; // �ؽ�Ʈ �Ž� ���� ���
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks//�ٸ� ���� ���� �޾Ƶ��̱�
{
    public static Launcher Instance; //Launcher ��ũ��Ʈ�� �޼���� ����ϱ� ����

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
        PhotonNetwork.ConnectUsingSettings();//������ ���� ������ ���� ������ ������ ����
    }

    public override void OnConnectedToMaster()//�����ͼ����� ����� �۵���
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();//������ ���� ����� �κ�� ����
        PhotonNetwork.AutomaticallySyncScene = true; //�ڵ����� ��� ������� ���� ���� ������
    }

    public override void OnJoinedLobby()//�κ� ����� �۵�
    {
        MenuManager.Instance.OpenMenu("title");//�κ� ������ Ÿ��Ʋ �޴� Ű��
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000"); //���� ��� �̸� �������� ���� �ٿ��ֱ�
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return; //�� �̸��� �� ���̸� �ȸ������
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text); //���� ��Ʈ��ũ ������� roomNameInputField.text�� �̸����� �游��
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom() // �濡 ���� �� �۵�
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; //�� �� �̸� ǥ��
        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject); //�濡 ���� ���� �ִ� �̸��� ����
        }
        for(int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]); //���� �濡 ���� �濡 �ִ� ��� ��� ��ŭ �̸�ǥ �߰��ϱ�
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); //���常 ���ӽ��� ��ư ������ ����
    }

    public override void OnMasterClientSwitched(Player newMasterClient)//������ ������ ������ �ٲ������
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //�� ����� ���н� �۵�
    {
        errorText.text = ("Room Creation Failed: " +message);
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // 1�� ������ ���忡�� scene��ȣ�� 1���� �̱� �����̴�. 0���� �ʱ� ��
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // �� ���������� ��Ʈ��ũ ���
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name); //���� ��Ʈ��ũ�� JoinRoom��� �ش� �̸��� ���� ������ ����
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()//���� ������ ȣ��
    {
        MenuManager.Instance.OpenMenu("title"); //�涰���� Ÿ��Ʋ �޴�ȣ��
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // ������ �� ����Ʈ ���
    {
        foreach(Transform trans in roomListContent) //�����ϴ� ��� roomListContent
        {
            Destroy(trans.gameObject); //�븮��Ʈ ������Ʈ�� �� �� ���� �� �����
        }
        for (int i = 0; i < roomList.Count; i++) //�� ���� ��ŭ �ݺ�
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]); //Instantiate�� prefab�� roomListContent��ġ�� ����� �ְ� �� �������� i��° �븮��Ʈ�� �ȴ�.
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //�ٸ� �÷��̾ �濡 ������ �۵�
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer); //Instatiate�� prefab�� playerlistcontent��ġ�� ������ְ� �� ������ �̸��� �޾Ƽ� ǥ��
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}