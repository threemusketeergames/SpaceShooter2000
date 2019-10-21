using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTubeSpawner : MonoBehaviour
{
    ParticleSystem ps;
    Vector3[] segment;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddSegment(Vector3[] v3s)
    {
        segment = v3s;
        var particles = new ParticleSystem.Particle[v3s.Length];
        for(int i =0; i< v3s.Length; i++)
        {
            particles[i].position = v3s[i];
            particles[i].remainingLifetime = float.PositiveInfinity;
            particles[i].startSize = 1f;
        }
        ps.SetParticles(particles);
    }
}
