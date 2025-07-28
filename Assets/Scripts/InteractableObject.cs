using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class InteractableObject : MonoBehaviourPunCallbacks, IPunObservable
{
    public string itemName;
    public bool playerInRange;
    public PhotonView photonView;
    public string GetItemName(){ return itemName; }
    
    void Awake()
    {
        // Get PhotonView component
        if (photonView == null) photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (photonView == null) return;
        if (!photonView.IsMine) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selectedGameObject==gameObject)
        {
            //if the inventory is not full
            if(!InventorySystem.instance.CheckIfFull())
            {
                // Call RPC to sync pickup across network
                photonView.RPC("PickupItemRPC", RpcTarget.All);
            }   
            else
            {
                Debug.Log("inventory is full");
            }    
            
        }
    }
    
    [PunRPC]
    void PickupItemRPC()
    {
        InventorySystem.instance.AddToInventory(itemName);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send object state
            stream.SendNext(playerInRange);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receive object state
            playerInRange = (bool)stream.ReceiveNext();
            Vector3 position = (Vector3)stream.ReceiveNext();
            Quaternion rotation = (Quaternion)stream.ReceiveNext();
            
            // Update transform
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
