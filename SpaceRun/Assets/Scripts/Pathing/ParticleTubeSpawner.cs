using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleTubeSpawner : MonoBehaviour
{
    public GameObject ParticleTube;
    public float density = 0.1f;
    public int maxParticlesPerSegment = 200;
    public float orbitSpeed = 0.1f;
    public Vector2 MaxMovement = new Vector2(1f, 0.75f);

    private int maxParticles;

    /// <summary>
    /// Path Spawn Manager
    /// </summary>
    PathSpawnManager psm;
    ParticleSystem tubeParticleSystem;
    Queue<SpawnSegmentInfo> newSegments;
    ParticleSystem.Particle[] particles;
    AsteroidOrbitInfo[] orbits;
    public GameObject Player;

    Tuple<Vector3, Vector3>[] lines = new Tuple<Vector3, Vector3>[0];
    Vector3[] points = new Vector3[0];
    int currentParticleIndex = 0;
    bool particleUpdate = false;
    private void Start()
    {
        psm = GetComponent<PathSpawnManager>();
        tubeParticleSystem = ParticleTube.GetComponent<ParticleSystem>();
        maxParticles = maxParticlesPerSegment * (psm.PathManager.NumWaypoints - 1);
        particles = new ParticleSystem.Particle[maxParticles];
        orbits = new AsteroidOrbitInfo[maxParticles];
        var main = tubeParticleSystem.main;
        main.maxParticles = particles.Length;
        newSegments = new Queue<SpawnSegmentInfo>();
        tubeParticleSystem.Emit(maxParticles);
        tubeParticleSystem.Play();
    }


    private void LateUpdate()
    {
        bool update = false;
        while (newSegments.Count > 0)
        {
            if (!update)
            {
                tubeParticleSystem.GetParticles(particles, particles.Length);
                update = true;
            }
            var ssi = newSegments.Dequeue();
            var segment = GetPoints(ssi).GetEnumerator();

            int i = 0;
            while (i < maxParticlesPerSegment && segment.MoveNext())
            {
                i++;
                var par = particles[currentParticleIndex];
                orbits[currentParticleIndex] = new AsteroidOrbitInfo()
                {
                    segment = ssi,
                    direction = new Vector2(Random.Range(-MaxMovement.x, MaxMovement.x), Random.Range(-MaxMovement.y, MaxMovement.y)),
                    radius = Vector3.Cross(ssi.mainSegment.dir, segment.Current - ssi.centerPoint).magnitude
                };

                par.position = segment.Current;
                par.startLifetime = float.MaxValue;
                par.startSize = Random.Range(0.75f, 1.75f);
                par.rotation = Random.Range(0f, 180f);
                particles[currentParticleIndex] = par;

                currentParticleIndex++;
                if (currentParticleIndex >= maxParticles) currentParticleIndex -= maxParticles;
            }
            if (i >= maxParticlesPerSegment)
            {
                Debug.LogWarning($"Attempted to spawn more particles than allowed in maxParticlesPerSegment ({maxParticlesPerSegment}).  Particle count was clamped down.");
            }
            tubeParticleSystem.SetParticles(particles, particles.Length);
        }
        //if (update)
        //{
        //    tubeParticleSystem.GetParticles(particles, particles.Length); //For some reason this extra get/set reminds Unity to update the last segment it has to do.
        //    tubeParticleSystem.SetParticles(particles, particles.Length);
        //}

    }

    private void FixedUpdate()
    {
       

        tubeParticleSystem.GetParticles(particles, particles.Length);
        for (int i = 0; i < orbits.Length; i++)
        {
            if (orbits[i] != null)  //Has this asteroid slot actually been filled with an asteroid? When we're first initializing the game it won't necessarily be
            {
                particles[i].position = GetNextOrbitedPoint(orbits[i], particles[i].position);
            }
        }

        tubeParticleSystem.SetParticles(particles, particles.Length);
        
    }

    Vector3 GetNextOrbitedPoint(AsteroidOrbitInfo aoi, Vector3 currentPos) //This function needs some serious optimization, it's getting called incessantly.
    {
        Vector3 pointOnSegment = Vector3.Project(currentPos - aoi.segment.centerPoint, aoi.segment.mainSegment.dir) + aoi.segment.centerPoint;

        Vector3 bottomPoint = aoi.segment.centerPoint;
        float shear = 0;
        if (aoi.segment.useWedgeAngler)
        {
            shear = Mathf.Tan(aoi.segment.wedgeAngler.wedgeAngle)
                * Mathf.Sin(
                    (90 - Vector3.Angle(
                        aoi.segment.wedgeAngler.wedgePerpFromMain,
                        currentPos - pointOnSegment)) * (float)Math.PI / 180
                    )
                * aoi.radius; //Fun math.


        }

        bottomPoint -= aoi.segment.mainSegment.dir * shear;

        float topPointDist = (pointOnSegment - aoi.segment.newPoint).magnitude;
        float bottomPointDist = (pointOnSegment - bottomPoint).magnitude;

        float segmentLength = (aoi.segment.newPoint - bottomPoint).magnitude;
        if (topPointDist > segmentLength || bottomPointDist > segmentLength) //If we've fallen off the segment, bounce
        {
            float absYSpeed = Mathf.Abs(aoi.direction.y);
            if (topPointDist > bottomPointDist)
            {
                aoi.direction.y = absYSpeed;
            }
            else
            {
                aoi.direction.y = -absYSpeed;
            }

        }
        Vector3 newPos = currentPos + aoi.segment.mainSegment.dir * aoi.direction.y * orbitSpeed;


        //currentPos is copied into the function (because it's a struct), so using it here won't affect the calling function
        Vector3 ihat = (newPos - pointOnSegment).normalized;
        Vector3 jhat = Vector3.Cross(aoi.segment.mainSegment.dir, ihat);
        float squeezeSpeedMultiplier = 1f;
        if (aoi.segment.useWedgeAngler)
        {
            float maxDistanceDifference = Mathf.Tan(aoi.segment.wedgeAngler.wedgeAngle) * psm.outerRadius;
            float segment = aoi.segment.mainSegment.length;
            //float speedChange = Mathf.Lerp(aoi.segment.mainSegment.length - maxDistanceDifference, aoi.segment.mainSegment.length + maxDistanceDifference, shear /);
            squeezeSpeedMultiplier = 1f + Mathf.Lerp(
                (segment + maxDistanceDifference) / segment,
                0f,
                shear / maxDistanceDifference / 2f + 0.5f);
        }

        newPos = GizmosUtil.PointOn3DCircle(pointOnSegment, ihat, jhat, aoi.radius, (aoi.direction.x * orbitSpeed * squeezeSpeedMultiplier) / aoi.radius);

        return newPos;
    }
    private void OnDrawGizmos()
    {
        float colorStep = 1.0f / (points.Length + lines.Length + 1);
        int i = 0;
        foreach (var point in points)
        {
            Gizmos.color = Color.HSVToRGB(colorStep * i, 1, 1);
            Gizmos.DrawSphere(point, 0.25f);
            i++;
        }
        foreach (var line in lines)
        {
            Gizmos.color = Color.HSVToRGB(colorStep * i, 1, 1);
            Gizmos.DrawLine(line.Item1, line.Item2);
            i++;
        }
    }



    // Start is called before the first frame update
    void SpawnForSegment(SpawnSegmentInfo ssi)
    {
        newSegments.Enqueue(ssi);
    }

    void DespawnLastWaypoint()
    {
        //We don't actually do anything here.  Instead of despawning particles behind us, we just wait for them to be used up
        //... when the next segment spawns (our currentParticleIndex just keeps looping, so the old particles will be used up first).
        //... This, at least, is the plan for now.
    }

    IEnumerable<Vector3> GetPoints(SpawnSegmentInfo ssi)
    {
        float outerSphereVolume = 4 / 3 * Mathf.PI * Mathf.Pow(psm.outerRadius, 3);
        float segmentLength = ssi.mainSegment.seg.magnitude;
        float hollowedCircleArea = 2 * Mathf.PI * (Mathf.Pow(psm.outerRadius, 2) - Mathf.Pow(psm.innerRadius, 2)); // Subtract big circle by little circle.  I factored out the 2pi like a total math pro.
        float spawnVolume = hollowedCircleArea * segmentLength;
        int tubeSpawnQuantity = (int)(spawnVolume * density);
        float radiusDiff = psm.outerRadius - psm.innerRadius;


        Vector2 point;
        float dist;
        Vector3 asteroidPoint;
        Vector3 localLinePoint = Vector3.zero;
        float localLineDist;

        int wedgeSpawnQuantity = ssi.useWedgeAngler ?
            (int)(outerSphereVolume * ssi.wedgeAngler.wedgeAngle / Mathf.PI * 0.5f * density)
            : 0;
        int totalSpawnQuantity = tubeSpawnQuantity + wedgeSpawnQuantity;
        float tubeSpawnOdds = (float)tubeSpawnQuantity / totalSpawnQuantity; //Some fancy statistical spawning so we can spawn both the main tube and the wedge at the same time, and if the process is interupted (e.g. by hitting a particle maximum), we'll have a relatively even distribution among what has succeeded to be spawned.
        for (int asteroid = 0; asteroid < totalSpawnQuantity; asteroid++)
        {
            if (Random.Range(0, 1.0f) <= tubeSpawnOdds)
            {
                do
                {
                    point = Random.insideUnitCircle * radiusDiff;
                    point += point.normalized * psm.innerRadius;
                    dist = Random.Range(0, segmentLength);
                    asteroidPoint = ssi.centerPoint + point.x * ssi.mainSegment.rightVector + point.y * ssi.mainSegment.upVector + dist * ssi.mainSegment.dir;
                    if (ssi.useWedgeAngler)
                    {
                        localLinePoint = asteroidPoint - ssi.lastSegment.midpoint;
                    }
                } while (ssi.useWedgeAngler &&  //This while will keep re-guessing points (if wedge angler is being used) until one lands outside of the cutout zone.
                        (localLineDist = Vector3.Dot(localLinePoint, ssi.lastSegment.dir)) < ssi.lastSegment.halfLength &&
                        (Vector3.Distance(asteroidPoint, localLinePoint + localLineDist * ssi.lastSegment.dir) < psm.outerRadius)
                    //find point nearest asteroidPoint on lastSegment line.  If A) within the bounds of that segmentStuffs.segment and B) within outerRadius of that segmentStuffs.segment, try again.
                    );
                yield return asteroidPoint;
            }
            else
            {
                float randihatAngle = Random.Range(0f, ssi.wedgeAngler.wedgeAngle);
                Vector3 randihat = ssi.wedgeAngler.wedgePerpFromLast * Mathf.Cos(randihatAngle) + ssi.lastSegment.dir * Mathf.Sin(randihatAngle);
                asteroidPoint = GizmosUtil.PointOn3DCircle(Vector3.zero, randihat, ssi.wedgeAngler.wedgePlaneNormal, Random.Range(0, radiusDiff), Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f));
                asteroidPoint = ssi.centerPoint + asteroidPoint + asteroidPoint.normalized * psm.innerRadius;
                yield return asteroidPoint;
            }
        }
    }
}

public class AsteroidOrbitInfo
{
    public Vector2 direction;
    public float radius;
    public SpawnSegmentInfo segment;
    //internal Vector3 startPoint;
    //internal float startTime;
}