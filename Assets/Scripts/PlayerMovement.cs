using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public CharacterController characterController;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public PhotonView photonView;
    
    // Network variables
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 velocity;
    private bool isGround;

    void Awake()
    {
        // Get components
        if (photonView == null) photonView = GetComponent<PhotonView>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
        
        // Initialize network variables
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void Update()
    {
        if (photonView == null) return;
        
        if (photonView.IsMine)
        {
            HandleLocalPlayer();
        }
        else
        {
            // Smooth interpolation for other players
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }
    
    void HandleLocalPlayer()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGround && velocity.y < 0 )
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move=transform.right * x + transform.forward * z;
        characterController.Move(move*speed*Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity*Time.deltaTime);
        
        if(gameObject.transform.position.y<=-10)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Lobby");
            }
        }    
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
