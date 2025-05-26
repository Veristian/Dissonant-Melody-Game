using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(BoxCollider2D))]
public class WindParticleController : MonoBehaviour
{
    public Vector2 windDirection = new Vector2(1f, 0f); // Wind direction (x = right, y = up)
    public float windStrength = 5f;                     // Wind speed
    public float particleRate = 100f;                   // Emission rate

    private ParticleSystem ps;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.EmissionModule emissionModule;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        velocityModule = ps.velocityOverLifetime;
        shapeModule = ps.shape;
        emissionModule = ps.emission;
        boxCollider = GetComponent<BoxCollider2D>();
        windDirection = GetComponent<WindArea>().windDirection;
        windStrength = windStrength * GetComponent<WindArea>().windForce;
    }

    void Start()
    {
        ApplyWindSettings();
        ps.Play();
    }

    public void ApplyWindSettings()
    {
        // Normalize and convert wind to 3D
        Vector3 windDir3D = new Vector3(windDirection.x, windDirection.y, 0).normalized;

        // Velocity over lifetime
        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.World;
        velocityModule.x = new ParticleSystem.MinMaxCurve(windDir3D.x * windStrength);
        velocityModule.y = new ParticleSystem.MinMaxCurve(windDir3D.y * windStrength);
        velocityModule.z = new ParticleSystem.MinMaxCurve(0f);

        // Set shape to box matching BoxCollider2D
        shapeModule.enabled = true;
        shapeModule.shapeType = ParticleSystemShapeType.Box;
        shapeModule.scale = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.1f);
        shapeModule.position = boxCollider.offset;

        // Emission rate
        emissionModule.rateOverTime = particleRate;
    }

    public void ActivateWind(Vector2 newWindDirection, float newStrength)
    {
        windDirection = newWindDirection;
        windStrength = newStrength;
        ApplyWindSettings();
        ps.Play();
    }
}
