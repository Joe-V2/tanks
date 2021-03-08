using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_V2 : MonoBehaviour
{
    int randomIndex = 0;
    int newRandomIndex = 0;

    [SerializeField] List<GameObject> nodes = new List<GameObject>();

    Vector2 targetNodePos;
    Vector2 nextNode;

    [SerializeField] GameObject currentNode;
    [SerializeField] GameObject target;

    [SerializeField] float timer = 0;
    [SerializeField] float timeTaken = 3.0f;
    [SerializeField] Vector2 AIPos;

    enum AIMovementMode
    {
        smooth,
        instant
    };

    [SerializeField] AIMovementMode movement;
    [SerializeField] bool bCoroutineStart = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] _nodeGameObject = GameObject.FindGameObjectsWithTag("Node");

        for (int i = 0; i < _nodeGameObject.Length; i++)
        {
            nodes.Add(_nodeGameObject[i]);
        }

        currentNode = nodes[0];
        CalculateNextNode();

        for(int i = 0; i < nodes.Count; i++)
        {
            if (Vector2.Distance(nodes[i].transform.position, this.transform.position) < Vector2.Distance(nextNode, this.transform.position))
            {
                nextNode = nodes[i].transform.position;
                timer = 0;
                AIPos = this.transform.position;
            }
        }
        targetNodePos = nodes[randomIndex].transform.position;
    }

    IEnumerator MoveAI()
    {
        while(bCoroutineStart)
        {
            this.transform.position = nextNode;
            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(movement == AIMovementMode.instant)
        {
            StartMovement();
        }
        else if(movement == AIMovementMode.smooth)
        {
            bCoroutineStart = false;
            MoveAISmooth();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Node"))
        {
            this.transform.position = other.transform.position;

            currentNode = other.gameObject;

            if(other.transform.position.x == targetNodePos.x && other.transform.position.y == targetNodePos.y)
            {
                while(newRandomIndex == randomIndex)
                {
                    randomIndex = Random.Range(0, nodes.Count);
                }
                newRandomIndex = randomIndex;

                targetNodePos = nodes[randomIndex].transform.position;
            }
            CalculateNextNode();
        }
    }

    void CalculateNextNode()
    {
        for (int i = 0; i < currentNode.GetComponent<CurrentNode>().accessibleNodes.Count; i++)
        {
            if (Vector2.Distance(currentNode.GetComponent<CurrentNode>().accessibleNodes[i].transform.position, targetNodePos) < Vector2.Distance(nextNode, targetNodePos))
            {
                nextNode = currentNode.GetComponent<CurrentNode>().accessibleNodes[i].transform.position;
                timer = 0;
                AIPos = transform.position;
            }
        }
    }

    void MoveAISmooth()
    {
        timer += Time.deltaTime;

        if(timer > timeTaken)
        {
            CalculateNextNode();
        }
        else
        {
            float speed = timer / timeTaken;

            transform.position = Vector2.Lerp(AIPos, nextNode, speed);
        }
    }

    void StartMovement()
    {
        if (!bCoroutineStart)
        {
            bCoroutineStart = true;
            StartCoroutine(MoveAI());
        }
    }
}
