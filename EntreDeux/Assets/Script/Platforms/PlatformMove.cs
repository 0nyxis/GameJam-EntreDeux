using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;


public class PlatformMove : MonoBehaviour
{
    [Tooltip("Les points de passages ne doivent pas être 'enfant' de la plateforme.")]
    [SerializeField] private List<Transform> points;
    [SerializeField] private bool move = false;
    [SerializeField] private LayerMask playerLayerMask;

    [Range(0.5f, 2f)]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float stopTime = 2f;
    [SerializeField] private bool forward = true;
    private Vector2Int betweenPoint;
    private Collider2D collider2D;
    private bool activated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<Collider2D>();

        betweenPoint.x = 0;
        betweenPoint.y = 1;

        activated = move;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            if (points.Count < 2)
            {
                Debug.LogWarning("Not enough points to move the platform.");
                return;
            }

            float speedMultiplier = 1f;

            if (betweenPoint.x == 0)
            {
                speedMultiplier = Clamp(Vector3.Distance(transform.position, points[betweenPoint.x].position), 0.1f, 1f);
            }
            if (betweenPoint.y == points.Count - 1)
            {
                speedMultiplier = Clamp(Vector3.Distance(transform.position, points[betweenPoint.y].position), 0.1f, 1f);
            }

            if (forward)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[betweenPoint.y].position, speed * Time.deltaTime * speedMultiplier);
                if (Vector3.Distance(transform.position, points[betweenPoint.y].position) < 0.1f)
                {
                    if (betweenPoint.y < points.Count - 1)
                    {
                        betweenPoint.x++;
                        betweenPoint.y++;
                    }
                    else
                    {
                        forward = false;
                        StartCoroutine(Wait());
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, points[betweenPoint.x].position, speed * Time.deltaTime * speedMultiplier);
                if (Vector3.Distance(transform.position, points[betweenPoint.x].position) < 0.1f)
                {
                    if (betweenPoint.x > 0)
                    {
                        betweenPoint.x--;
                        betweenPoint.y--;
                    }
                    else
                    {
                        forward = true;
                        StartCoroutine(Wait());
                    }
                }
            }
        }
        else
        {
            if(!activated && collider2D.IsTouchingLayers(playerLayerMask))
            {
                StartCoroutine(Wait());
            }
        }
    }

    private IEnumerator Wait()
    {
        move = false;

        activated = true;

        //Debug.Log("googoogaga");

        yield return new WaitForSeconds(stopTime);

        move = true;
    }


    //private void OnDrawGizmos()
    //{
    //    if (points != null && points.Count > 0)
    //    {
    //        for (int i = 0; i < points.Count; i++)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(points[i].position, 0.1f);
    //            if (i < points.Count - 1)
    //            {
    //                Gizmos.color = Color.green;
    //                Gizmos.DrawLine(points[i].position, points[i + 1].position);
    //            }
    //        }
    //    }
    //}
}
