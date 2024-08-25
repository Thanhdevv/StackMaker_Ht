using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float PlayerHeight = 0.15f;
    [SerializeField] private LayerMask brickLayer;
    [SerializeField] private LayerMask BrickPosStart;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask BrickToBuild;
    [SerializeField] private LayerMask BuildBridgeEnd;
    [SerializeField] private LayerMask finishPoint;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isBridgeBrick = true;
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer meshRenderer1;
    private float threshold = 0.0001f;
    private List<GameObject> bricksUnderPlayer = new List<GameObject>();
    private List<GameObject> hiddenBricks = new List<GameObject>();
    private List<GameObject> builtBricks = new List<GameObject>();
    public Transform Model;
    public static int score;
    float rayDistance = 0.5f;
    
    private void Start()
    {
       OnInit();
    }

    public void OnInit()
    {
        ResetAnimator();
        isMoving = false;
        isBridgeBrick = true;
        score = 0;
        UIManager.Instance.HideVictory();
    }
    
    void Update()
    {
        
        if (!isMoving && isBridgeBrick)
        {
            UpdateDirection();
            if (Vector3.Distance(currentDirection, Vector3.zero) > threshold)
            {
                isMoving = true;
            }
        }
        else if (!isBridgeBrick && bricksUnderPlayer.Count == 0)
        {
            UpdateDirection();
        }

        Move();
        DetectBricks();

    }

    private void UpdateDirection()
    {
        if (MobileInput.Instance.swipeLeft)
        {
            currentDirection = Vector3.left;
        }
        else if (MobileInput.Instance.swipeRight)
        {
            currentDirection = Vector3.right;
        }
        else if (MobileInput.Instance.swipeUp)
        {
            currentDirection = Vector3.forward;
        }
        else if (MobileInput.Instance.swipeDow)
        {
            currentDirection = -Vector3.forward;
        }
    }

    public void SetStartPosition(Vector3 position)
    {
        transform.position = new(position.x, position.y + 0.28f, position.z);
        currentDirection = Vector3.zero;
    }

    private void Move()
    {

        if (!isWall() && isBridgeBrick)
        {
            transform.position += currentDirection * speed * Time.deltaTime;
            UpdateBrickPositions();
        }
        else
        {
            isMoving = false;
        }

    }

    private bool isWall()
    {
        RaycastHit hit;
        

        Debug.DrawRay(transform.position, currentDirection * rayDistance, Color.blue);


        return Physics.Raycast(transform.position, currentDirection, out hit, rayDistance, wallLayer);
    }

    private void DetectBricks()
    {
        RaycastHit hit;
        
        Debug.DrawRay(transform.position, currentDirection * rayDistance, Color.red);

        if (Physics.Raycast(transform.position, currentDirection, out hit, rayDistance, brickLayer) ||
            Physics.Raycast(transform.position, Vector3.back, out hit, rayDistance, BrickPosStart))
        {
            ProcessBrick(hit);
        }
        if (Physics.Raycast(transform.position, currentDirection, out hit, rayDistance, BrickToBuild))
        {
            BuildBridge(hit.collider.gameObject);
        }
     
        if (Physics.Raycast(transform.position, currentDirection, out hit, rayDistance, finishPoint))
        {
            FinishPoint();
        }
    }

   private void FinishPoint() {

        isBridgeBrick = false;
        animator.SetTrigger("Victory");
        
        StartCoroutine(NextLevelCoroutine());
       
    }

    IEnumerator NextLevelCoroutine()
    {
        GameManager.ChangeState(GameState.Finish);

        yield return new WaitForSeconds(2f);
    }


    private void BuildBridge(GameObject brickToBuild)
    {
        if (bricksUnderPlayer.Count > 0)
        {
            score++;
            isBridgeBrick = true;
            DeleteBrick();
            MeshRenderer meshRenderer = brickToBuild.GetComponent<MeshRenderer>();
            BoxCollider boxCollider = brickToBuild.GetComponent<BoxCollider>();

            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                builtBricks.Add(brickToBuild); 
            }

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            DecreaseHeight();
        }
        else
        {
            Debug.Log("Không đủ gạch để xây cầu. Quay lại thu thập thêm gạch.");
            currentDirection = Vector3.zero;
            isMoving = false;   
        }
        
    }

    private void AddBrickUnderPlayer()
    {
        Vector3 brickPosition = transform.position + new Vector3(0, 0, 0);
        GameObject newBrick = Instantiate(brickPrefab, brickPosition, Quaternion.identity);

        newBrick.transform.parent = transform;
        newBrick.transform.localRotation = Quaternion.Euler(-90f, 0, -180f);

        bricksUnderPlayer.Add(newBrick);
    }

    private void UpdateBrickPositions()
    {
        
        float yOffset = 0f;
        foreach (var brick in bricksUnderPlayer)
        {
            if (brick != null)
            {
                Vector3 brickPosition = transform.position + new Vector3(0, yOffset, 0);
                brick.transform.position = brickPosition;
                yOffset += PlayerHeight;
            }
        }
    }

    private void ProcessBrick(RaycastHit hit)
    {
        MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
            hiddenBricks.Add(hit.collider.gameObject);
            AddBrickUnderPlayer();
            AddHeight();
            isMoving = true;
        }
    }

    private void DeleteBrick()
    {
        GameObject lastBrick = bricksUnderPlayer[bricksUnderPlayer.Count - 1];
        bricksUnderPlayer.RemoveAt(bricksUnderPlayer.Count - 1);
        Destroy(lastBrick);
    }

    public void ResetLevel()
    {

        foreach (var brick in bricksUnderPlayer)
        {
            Destroy(brick);
        }
        bricksUnderPlayer.Clear();

        foreach (var hiddenBrick in hiddenBricks)
        {
            if (hiddenBrick != null)
            {
                hiddenBrick.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        hiddenBricks.Clear();

        foreach (var builtBrick in builtBricks)
        {
            if (builtBrick != null)
            {
                builtBrick.GetComponent<MeshRenderer>().enabled = false;
                builtBrick.GetComponent<BoxCollider>().enabled = true; 
            }
        }
        builtBricks.Clear();

        Model.localPosition = new Vector3(0, 0, 0);

        currentDirection = Vector3.zero;
        isMoving = false;
        isBridgeBrick = true;
        ResetAnimator();
    }

    private void AddHeight()
    {
        Model.localPosition += new Vector3(0, PlayerHeight, 0);
    }

    private void DecreaseHeight()
    {
        Model.localPosition -= new Vector3(0, PlayerHeight, 0);
    }
    public void ResetAnimator()
    {
        if (animator != null)
        {
            animator.SetTrigger("idle"); 
        }
    }

}
