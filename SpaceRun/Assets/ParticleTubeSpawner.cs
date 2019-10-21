using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleTubeSpawner : MonoBehaviour
{
    public GameObject ParticleTube;
    public float density = 0.1f;
    public int maxParticlesPerSegment = 200;

    /// <summary>
    /// Path Spawn Manager
    /// </summary>
    PathSpawnManager psm; 
    ParticleSystem tubeParticleSystem;
    Vector3[] segment;
    private void Start()
    {
        psm = GetComponent<PathSpawnManager>();
        tubeParticleSystem = ParticleTube.GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void SpawnForSegment(SpawnSegmentInfo ssi)
    {
        segment = GetPoints(ssi).ToArray();
        var particles = new ParticleSystem.Particle[segment.Length];
        for (int i = 0; i < segment.Length; i++)
        {
            particles[i].position = segment[i];
            particles[i].remainingLifetime = float.PositiveInfinity;
            particles[i].startSize = 1f;
        }
        tubeParticleSystem.SetParticles(particles);
    }

    IEnumerable<Vector3> GetPoints(SpawnSegmentInfo ssi)
    {
        float outerSphereVolume = 4 / 3 * Mathf.PI * Mathf.Pow(psm.outerRadius, 3);
        float segmentLength = ssi.mainSegment.segment.magnitude;
        float hollowedCircleArea = 2 * Mathf.PI * (Mathf.Pow(psm.outerRadius, 2) - Mathf.Pow(psm.innerRadius, 2)); // Subtract big circle by little circle.  I factored out the 2pi like a total math pro.
        float spawnVolume = hollowedCircleArea * segmentLength;
        int spawnQuantity = (int)(spawnVolume * density);
        float radiusDiff = psm.outerRadius - psm.innerRadius;

        Vector3? lastSegmentMidpoint = null;
        float? lastSegmentMaxMidpointDist = null;
        if (ssi.useWedgeAngler)
        {
            lastSegmentMidpoint = (ssi.firstPoint.Value + ssi.centerPoint) / 2;
            lastSegmentMaxMidpointDist = Vector3.Distance(ssi.firstPoint.Value, ssi.centerPoint) / 2;
        }

        Vector2 point;
        float dist;
        Vector3 asteroidPoint;
        Vector3 localLinePoint = Vector3.zero;
        float localLineDist;
        for (int asteroid = 0; asteroid < spawnQuantity; asteroid++)
        {
            do
            {
                point = Random.insideUnitCircle * radiusDiff;
                point += point.normalized * psm.innerRadius;
                dist = Random.Range(0, segmentLength);
                asteroidPoint = ssi.centerPoint + point.x * ssi.mainSegment.rightVector + point.y * ssi.mainSegment.upVector + dist * ssi.mainSegment.segmentDir;
                if (ssi.useWedgeAngler)
                {
                    localLinePoint = asteroidPoint - lastSegmentMidpoint.Value;
                }
            } while (ssi.useWedgeAngler &&  //This while will keep re-guessing points (if wedge angler is being used) until one lands outside of the cutout zone.
                    (localLineDist = Vector3.Dot(localLinePoint, ssi.wedgeAngler.lastSegmentDir)) < lastSegmentMaxMidpointDist &&
                    (Vector3.Distance(asteroidPoint, localLinePoint + localLineDist * ssi.wedgeAngler.lastSegmentDir) < psm.outerRadius)
            //find point nearest asteroidPoint on lastSegment line.  If A) within the bounds of that segmentStuffs.segment and B) within outerRadius of that segmentStuffs.segment, try again.
            );
            yield return asteroidPoint;
        }

        if (ssi.useWedgeAngler) //Spawn asteroids in spherical corner wedge (in a shape like a slice of an orange)
        {
            float volume = outerSphereVolume * ssi.wedgeAngler.wedgeAngle / Mathf.PI * 0.5f;
            int wedgeSpawnQuantity = (int)(volume * density);
            for (int asteroid = 0; asteroid < wedgeSpawnQuantity; asteroid++)
            {
                float randihatAngle = Random.Range(0f, ssi.wedgeAngler.wedgeAngle);
                Vector3 randihat = ssi.wedgeAngler.wedgePerpFromLast * Mathf.Cos(randihatAngle) + ssi.wedgeAngler.lastSegmentDir * Mathf.Sin(randihatAngle);
                asteroidPoint = GizmosUtil.PointOn3DCircle(Vector3.zero, randihat, ssi.wedgeAngler.wedgePlaneNormal, Random.Range(0, radiusDiff), Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f));
                asteroidPoint = ssi.centerPoint + asteroidPoint + asteroidPoint.normalized * psm.innerRadius;
                yield return asteroidPoint;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
