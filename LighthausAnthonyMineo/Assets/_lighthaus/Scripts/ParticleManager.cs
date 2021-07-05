using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleController))]
public class ParticleManager : MonoBehaviour
{
    [SerializeField, HideInEditorMode]
    private ParticleSystem _particleSystem;
    [SerializeField, HideInEditorMode]
    private ParticleController _particleController;
    [SerializeField]
    private Gradient _gradient;
    [SerializeField, Range(0.01f, 100f)]
    private float _speed = 1.0f;
    [SerializeField]
    [Tooltip("Pick either FollowForward or FollowBackward")]
    private ParticleController.MotionType _motionType = ParticleController.MotionType.FollowForward;

    public ParticleSystem ParticleSystem { get => _particleSystem; set => _particleSystem = value; }
    public ParticleController ParticleController { get => _particleController; set => _particleController = value; }

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        ParticleSystem = GetComponent<ParticleSystem>();
        ParticleController = GetComponent<ParticleController>(); ParticleSystem.MainModule main = ParticleSystem.main;
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ParticleSystem.colorOverLifetime;
        colorOverLifetime.color = _gradient;
        main.simulationSpeed = _speed;
        _particleController.motionType = _motionType;
    }
}
