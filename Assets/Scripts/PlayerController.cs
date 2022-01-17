using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using TMPro;


public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float moveSpeed = 5.0f;
    private float r = 0.0f;
    private float y = 0.0f;
    private float rotationSpeed = 500.0f;

    [SerializeField] Item[] items;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    public int itemIndex;
    public int previousItemIndex = -1; //기본 아이템 값 없도록
    public TMP_Text healthText;

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
        healthText.text = ("Current Health: " + currentHealth.ToString());
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f) //마우스 스크롤 움직이면
        {
            if(itemIndex >= items.Length-1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f) //마우스 스크롤 반대로 움직이면
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length-1);
            }
            else
            {
                EquipItem(itemIndex-1);
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
        y = Input.GetAxis("Mouse Y");
        //Debug.Log("H: " + h.ToString() + ", V: " + v.ToString());
        playerTr.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
        playerTr.Rotate(new Vector3(0, r, 0) * rotationSpeed * Time.deltaTime);
        playerTr.Rotate(new Vector3(-y, 0, 0) * rotationSpeed * Time.deltaTime);
        
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

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);// hash[itemindex]가 호출되면 현재 아이템 번호가 호출
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //다른 플레이어의 아이템 번호 받아들이기
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)//내꺼가 아니라 다른사람꺼일때
        {
            EquipItem((int)changedProps["itemIndex"]);
            //끼고있는 아이템 정보 받아들이기
        }
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

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage); //피해를 입힌 사람이 해당 이름가진 함수를 RpcTarget(지금은 모든 플레이어)에게 적용 되도록 호출
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
        {
            return; //피해입은 플레이어 아니면 실행 안됨
        }
        Debug.Log("took damage " + damage);
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
            currentHealth = maxHealth;
            Respawn();
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    [PunRPC]
    void Respawn()
    {
        playerManager.CreateController();
    }
}
