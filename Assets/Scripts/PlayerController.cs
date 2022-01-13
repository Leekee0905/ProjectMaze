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
    public int previousItemIndex = -1; //�⺻ ������ �� ������

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
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
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
