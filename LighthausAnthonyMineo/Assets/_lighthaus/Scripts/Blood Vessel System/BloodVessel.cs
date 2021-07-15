using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class BloodVessel : MonoBehaviour
{
    [SerializeField]
    private BloodVesselName _bloodVesselName;
    [SerializeField]
    private SplineComputer _splineComputer;
    [SerializeField]
    private ParticleManager _particleManager;

    public BloodVesselName BloodVesselName { get => _bloodVesselName; set => _bloodVesselName = value; }
    public SplineComputer SplineComputer { get => _splineComputer; set => _splineComputer = value; }
    public ParticleManager ParticleManager { get => _particleManager; set => _particleManager = value; }
}
