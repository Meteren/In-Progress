using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MainTrap, ITrap
{
    [SerializeField] private List<Transform> wayPoints;
    [SerializeField] private BoxCollider2D frame;
    int currentIndex = 0;
    int increment;
    float speed = 3f;
    float distance = 0.3f;


    private void Start()
    {
        foreach(var transform in wayPoints)
        {
            float x = Random.Range((frame.bounds.center.x - (frame.bounds.size.x / 2)), frame.bounds.center.x + (frame.bounds.size.x / 2));
            float y = Random.Range((frame.bounds.center.y - (frame.bounds.size.y / 2)), frame.bounds.center.y + (frame.bounds.size.y / 2));

            transform.position = new Vector2(x, y);

        }
    }

    void Update()
    {
        TrapLogic();

    }

    public void TrapLogic()
    {
        Debug.Log(wayPoints.Count);
        if (currentIndex >= wayPoints.Count)
        {
            currentIndex = 0;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentIndex].position, Time.deltaTime * speed);

        if (Vector2.Distance(transform.position, wayPoints[currentIndex].position) < distance)
        {
            currentIndex++;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            controller.knockBack = true;
        }
    }
}
