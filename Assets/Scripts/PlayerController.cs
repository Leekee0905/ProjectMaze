using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float moveSpeed = 5.0f;
    private float r = 0.0f;
    private float rotationSpeed = 500.0f;

    [SerializeField] Item[] items;
    [SerializeField] GameObject cameraHolder;
    public int itemIndex;
    public int previousItemIndex = -1; //기본 아이템 값 없도록

    bool grounded;
    private Transform playerTr;

    Rigidbody rb;
    PhotonView PV;

    PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        playerTr = GetComponent<Transform>();
        if(!PV.IsMine)
        {
            EquipItem(0); // 시작하고 내 포톤뷰면 1번 아이템 끼기 (2번 아이템은 번호상 1이다.)
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
        Move();
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
        if(Input.GetMouseButtonDown(0))//마우스 좌클릭시
        {
            items[itemIndex].Use(); //들고 있던 아이템 사용
        }
    }

    void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");
        //Debug.Log("H: " + h.ToString() + ", V: " + v.ToString());
        playerTr.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
        playerTr.Rotate(new Vector3(0, r, 0) * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "STAR")
        {
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "Flag")
        {
            Destroy(collision.gameObject);
            Debug.Log("축하합니다 1등입니다.");
            Debug.Log("탈출에 성공하였습니다.");
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return; //입력 받은 숫자가 아까 받은 숫자랑 똑같으면 아무일 안함.
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true); // itemIndex번째 아이템 on
        if (previousItemIndex != -1)//만약 초기 상태가 아니라면
        {
            items[previousItemIndex].itemGameObject.SetActive(false);//내가 아까 꼈던 아이템 off
        }
        previousItemIndex = itemIndex; //무한 사이클
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
    }
}
