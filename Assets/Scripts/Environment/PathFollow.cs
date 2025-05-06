using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : MonoBehaviour
{    
    [Header("Settings")] 
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float minDistanceToPoint = 0.01f;
    [SerializeField] private List<Vector3> points = new List<Vector3>();  // Get the Point list

    private BoxCollider2D boxCollider;
    private bool _playing;
    private bool _moved;
    private int _currentPoint = 0;
    private Vector3 _currentPosition;
    private int counter;
    private void Start()
    {
        _playing = true;
        _currentPosition = transform.position;
        transform.position = _currentPosition + points[0];
    }

    private void OnValidate()
    {
        if (points.Count < 2)
        {
            points.Add(new Vector3());
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Set first position
        if (!_moved)
        {
            transform.position = _currentPosition + points[0];
            _currentPoint++;
            _moved = true;
        } 
        
        // Move to next point
        transform.position = Vector3.MoveTowards(transform.position, _currentPosition + points[_currentPoint], Time.fixedDeltaTime * moveSpeed);
        
        // Evaluate move to next point
        float distanceToNextPoint = Vector3.Distance(_currentPosition + points[_currentPoint], transform.position);
        if (distanceToNextPoint < minDistanceToPoint)
        {            
            _currentPoint++;
        }
        
        // If we are on the last point, reset our position to the first one
        if (_currentPoint == points.Count)
        {
            _currentPoint = 0;
        }
    }
    
    private void OnDrawGizmos()
    {  
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        if (transform.hasChanged && !_playing)
        {
            _currentPosition = boxCollider.bounds.center;
        }
      
        if (points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count)
                {
                    // Draw points
                    Gizmos.color = Color.red;

                    if (boxCollider != null)
                    {
                        Gizmos.DrawWireCube(_currentPosition + points[i],boxCollider.size*gameObject.transform.localScale);

                    }

                  
                    // Draw lines
                    Gizmos.color = Color.black;
                    if (i < points.Count - 1)
                    {
                        Gizmos.DrawLine(_currentPosition + points[i], _currentPosition + points[i + 1]);
                    }
                    
                    // Draw line from last point to first point
                    if (i == points.Count - 1)
                    {
                        Gizmos.DrawLine(_currentPosition + points[i], _currentPosition + points[0]);
                    }
                }
            }
        }
    }

}
