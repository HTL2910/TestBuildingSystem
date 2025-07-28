using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class AIMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    Animator animator;
    public PhotonView photonView;

    public float moveSpeed = 0.2f;
    private Vector3 stopPosition;
    private float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;
    int WalkDirection;
    public bool isWalking;
    
    // Network variables
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        // Get PhotonView component
        if (photonView == null) photonView = GetComponent<PhotonView>();
        
        // Initialize network variables
        networkPosition = transform.position;
        networkRotation = transform.rotation;
        
        if (photonView.IsMine)
        {
            walkTime = Random.Range(3, 6);
            waitTime = Random.Range(5, 7);

            waitCounter = waitTime;
            walkCounter = walkTime;

            ChooseDirection();
        }
    }
    private void Update()
    {
        if (photonView == null) return;
        
        if (photonView.IsMine)
        {
            UpdateAI();
        }
        else
        {
            // Smooth interpolation for other clients
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }
    
    void UpdateAI()
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true);
            walkCounter -= Time.deltaTime;
            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;

                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }
            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, 0f, transform.position.z);
                isWalking = false;
                transform.position = stopPosition;
                animator.SetBool("isRunning", false);
                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send AI state
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isWalking);
            stream.SendNext(WalkDirection);
        }
        else
        {
            // Receive AI state
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            isWalking = (bool)stream.ReceiveNext();
            WalkDirection = (int)stream.ReceiveNext();
            
            // Update animations for remote clients
            if (animator != null)
            {
                animator.SetBool("isRunning", isWalking);
            }
        }
    }
}


