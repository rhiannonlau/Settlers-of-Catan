using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    public GameObject Canvas;
    public PlayerManager PlayerManager;

    private bool isDragging = false;
    private bool isDraggable = true;
    private GameObject startParent;
    private Vector2 startPosition;

    private GameObject dropZone;
    private bool isOverDropZone;

    private GameObject playerArea;
    private bool isOverPlayerArea;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        
        if (!isOwned)
        {
            isDraggable = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
        // add a collider to the playerarea
        // check which collider is being collided with using
        // if(other.gameObject.name.Equals("Bird1"))
        // or 
        // other.gameObject.CompareTag("Bird");
        // https://stackoverflow.com/questions/71927904/how-to-detect-which-gameobject-is-being-collided-with
    }


    // // might have to checkmark "is trigger" in DropZone's "box collider" for this to work?
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Entered collision with " + other.gameObject.name);
    //     isOverDropZone = true;
    //     dropZone = other.gameObject;
    // }

    // private void OnTriggerStay(Collider other)
    // {
    //     Debug.Log("Colliding with " + other.gameObject.name);
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     Debug.Log("Exited collision with " + other.gameObject.name);
    //     isOverDropZone = false;
    //     dropZone = null;
    // }

    public void BeginDrag()
    {
        if (!isDraggable) return;

        isDragging = true;
        // find the gameObject corresponding to the parent of the transform
        // and set it to startParent
        // in this case, startParent = PlayerArea
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
    }

    public void EndDrag()
    {
        if (!isDraggable) return; // for edge cases

        isDragging = false;

        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            // isDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.PlayCard(gameObject);
        }
        
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            // set world position to true so that the position is not changed, it just
            // follows what you previously set, i.e. the mouse
            transform.SetParent(Canvas.transform, true);
        }
    }
}
