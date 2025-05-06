using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "Player Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]

    public float MaxWalkSpeed = 12.5f;
    public float GroundAcceleration = 5f;
    public float GroundDeceleration = 20f;
    public float AirAcceleration = 5f;
    public float AirDeceleration = 5f;

    [Header("Run")]
    public float MaxRunSpeed = 20f;

    [Header("Collision Checks")]
    public LayerMask GroundLayer;
    public LayerMask HeadLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    public float WallDetectionRayLength = 0.1f;
    public float Width = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    public float TimeTillJumpApex = 0.35f;
    public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    public float TimeForUpwardsCancel = 0.027f;
    
    [Header("Jump Apex")]
    public float ApexThreshold = 0.97f;
    public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    public float JumpBufferTime = 0.0125f;

    [Header("Jump Coyote Time")]
    public float JumpCoyoteTime = 0.1f;
    
    [Header("Dash")]
    public float DashDistance = 6f;
    public float TimeTillCompleteDash = 0.1f;

    public AnimationCurve dashCurve;
    [Header("Hop")]
    public float HopHeight = 6.5f;
    public float TimeTillHopApex = 0.35f;

    public float HopGravity  { get; private set;}   
    public float InitialHopVelocity  { get; private set;}   

    [Header("Float")]
    public float FloatMaxFallSpeed = 10f;

    [Header("Shoot")]
    public GameObject BulletPrefab;
    public float BulletSpeed = 5f;
    public float BulletDespawnTime = 3f;

    [Header("Teleport")]
    public float TeleportToTimeRatio = 1f;
    
    
    [Header("Gizmos")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;
    public bool DebugShowWallCheck;

    

    public float Gravity { get; private set;}
    public float InitialJumpVelocity {get; private set;}

    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        Gravity = -(2f *JumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f); //Gravity (distance = 1/2 * a * t^2) where a is gravity and t is time

        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex; //Velocity ( v = a * t) where v is initial velocity, a is gravity and t is time
        
        HopGravity = -(2f *HopHeight) / Mathf.Pow(TimeTillHopApex, 2f); //Gravity (distance = 1/2 * a * t^2) where a is gravity and t is time

        InitialHopVelocity = Mathf.Abs(HopGravity) * TimeTillHopApex; //Velocity ( v = a * t) where v is initial velocity, a is gravity and t is time


    }
}
