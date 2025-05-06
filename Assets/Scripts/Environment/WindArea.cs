using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : Environment
{
    [Header("Settings")]
    [SerializeField] private float windForce;
    [SerializeField] private Vector2 windDirection;

    private float newWindForce;
    private Vector2 newWindDirection;
 
    private void Start()
    {
        unityEvent.AddListener(() => playerMovement.ApplyWind(newWindForce,newWindDirection));

    }
    protected override void PlayerEnterTrigger()
    {
        newWindDirection = windDirection;
        newWindForce = windForce;
        unityEvent.Invoke();
    }
    protected override void PlayerExitTrigger()
    {
        newWindDirection = Vector2.zero;
        newWindForce = 0;
        unityEvent.Invoke();
    }

}
