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
    public int previousItemIndex = -1; //�⺻ ������ �� ������
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
            EquipItem(0); // �����ϰ� �� ������ 1�� ������ ���� (2�� �������� ��ȣ�� 1�̴�.)
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
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f) //���콺 ��ũ�� �����̸�
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
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f) //���콺 ��ũ�� �ݴ�� �����̸�
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
        if(Input.GetMouseButtonDown(0))//���콺 ��Ŭ����
        {
            items[itemIndex].Use(); //��� �ִ� ������ ���
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
            Debug.Log("�����մϴ� 1���Դϴ�.");
            Debug.Log("Ż�⿡ �����Ͽ����ϴ�.");
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return; //�Է� ���� ���ڰ� �Ʊ� ���� ���ڶ� �Ȱ����� �ƹ��� ����.
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true); // itemIndex��° ������ on
        if (previousItemIndex != -1)//���� �ʱ� ���°� �ƴ϶��
        {
            items[previousItemIndex].itemGameObject.SetActive(false);//���� �Ʊ� ���� ������ off
        }
        previousItemIndex = itemIndex; //���� ����Ŭ

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);// hash[itemindex]�� ȣ��Ǹ� ���� ������ ��ȣ�� ȣ��
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //�ٸ� �÷��̾��� ������ ��ȣ �޾Ƶ��̱�
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)//������ �ƴ϶� �ٸ�������϶�
        {
            EquipItem((int)changedProps["itemIndex"]);
            //�����ִ� ������ ���� �޾Ƶ��̱�
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
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage); //���ظ� ���� ����� �ش� �̸����� �Լ��� RpcTarget(������ ��� �÷��̾�)���� ���� �ǵ��� ȣ��
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
        {
            return; //�������� �÷��̾� �ƴϸ� ���� �ȵ�
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
