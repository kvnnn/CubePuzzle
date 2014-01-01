using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleTester : MonoBehaviour
{
	private ParticleSystem ps;

	void Start() {
		ps = gameObject.GetComponent<ParticleSystem>();
	}

	void Update() {
		if(ps) {
			if(!ps.IsAlive()) {
				Destroy(gameObject);
			}
		}
	}
}
