using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellMeasurement : MonoBehaviour
{
    //simple class to help collect data via dictionary to make sure no duplicate data is collected
    public class CalculateParticle
    {
        private Vector3 _previousPosition;
        private Vector3 _currentPosition;
        private bool _calculated = false;
        private bool _counted = false;

        public Vector3 PreviousPosition { get => _previousPosition; set => _previousPosition = value; }
        public Vector3 CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
        public bool Calculated { get => _calculated; set => _calculated = value; }
        public bool Counted { get => _counted; set => _counted = value; }

        public CalculateParticle(Vector3 previousPosition)
        {
            _previousPosition = previousPosition;
        }
    }

    private enum ParticleState
    {
        Entered,
        Exited
    }
    
    private ParticleSystem _particleSystem;
    private float _cumulativeMagnitude = 0;
    private ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[0];
    private int _numParticlesAlive = 0;
    private Collider _collider;
    private Dictionary<int, CalculateParticle> _particlesToCalculate = new Dictionary<int, CalculateParticle>();

    private int _cellCount = 0;
    private Vector3 _averageVelocity;
    private List<Vector3> _velocities = new List<Vector3>();
    private float _averageSpeed;
    private List<ParticleSystem.Particle> _particleEnter = new List<ParticleSystem.Particle>();
    private List<ParticleState> _particleStates = new List<ParticleState>();

    public int CellCount { get => _cellCount; set => _cellCount = value; }
    public List<Vector3> Velocities { get => _velocities; set => _velocities = value; }
    public Vector3 AverageVelocity { get => _averageVelocity; set => _averageVelocity = value; }
    public float AverageSpeed { get => _averageSpeed; set => _averageSpeed = value; }
    public Dictionary<int, CalculateParticle> ParticlesToCalculate { get => _particlesToCalculate; set => _particlesToCalculate = value; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    //When a blood cell is selected make sure the correct particle system is being checked
    //and reset necessary values
    public void UpdateActiveBloodVessel(ParticleSystem particleSystem)
    {
        _particleSystem = particleSystem;
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        _particlesToCalculate.Clear();
        _particleStates.Clear();
        _cellCount = 0;

        for (int i = 0; i < _particles.Length; i++)
            _particleStates.Add(ParticleState.Exited);
    }

    private void Update()
    {
        ManuallyCalculateVelocity();
    }

    private void ManuallyCalculateVelocity()
    {
        //reset if max particles changes
        if (_particles == null || _particles.Length < _particleSystem.main.maxParticles)
        {
            _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        }

        //limit for the ammount of items we need to loop through as not every particle is currently in use
        _numParticlesAlive = _particleSystem.GetParticles(_particles);

        //reset values for this frame
        _cumulativeMagnitude = 0;
        _velocities.Clear();
        _averageVelocity = Vector3.zero;

        //if a particle is marked to be calculated get this frames position to have data to calculate a position delta/vector
        for (int i = 0; i < _particlesToCalculate.Count; i++)
        {
            var item = _particlesToCalculate.ElementAt(i);
            if (item.Value.CurrentPosition == null || item.Value.CurrentPosition == Vector3.zero)
                item.Value.CurrentPosition = _particles[item.Key].position;
        }

        for (int i = 0; i < _numParticlesAlive; i++)
        {
            //check if any particles are in the hitbox of the velocity checker
            if (_collider.bounds.Contains(_particles[i].position))
            {
                //check if that particle is already registered to have its velocity calculated
                if(!_particlesToCalculate.ContainsKey(i))
                {
                    //add new particle to be calculated
                    _particlesToCalculate.Add(i, new CalculateParticle(_particles[i].position));
                }

                //If particle was not already marked as existing in the
                //hitbox mark it as entered and increment the cell counter
                if (_particleStates[i] == ParticleState.Exited)
                {
                    _particleStates[i] = ParticleState.Entered;
                    _cellCount++;
                }
            }
            else
            {
                //once determined that the cell is not longer in the hitbox
                //set its state as exited so it may be counted again later
                if (_particleStates[i] == ParticleState.Entered)
                    _particleStates[i] = ParticleState.Exited;
            }
        }

        for(int i = 0; i < _particlesToCalculate.Count; i++)
        {
            var item = _particlesToCalculate.ElementAt(i);
            //check if particles marked to calculate have the necessary data
            if (item.Value.CurrentPosition != null && item.Value.CurrentPosition != Vector3.zero)
            {
                //get distance for magnitude/speed value
                float distance = Vector3.Distance(item.Value.CurrentPosition, item.Value.PreviousPosition);
                //collect all the magnitueds for average calculation later
                _cumulativeMagnitude += distance / Time.deltaTime;
                //calculate velocity vector
                Vector3 velocity = (item.Value.CurrentPosition - item.Value.PreviousPosition);
                //collect all the velocity vectors for average calculation later
                _velocities.Add(velocity);
                item.Value.Calculated = true;
                _averageVelocity += velocity;
            }
        }

        //calculate average speed in meters/second
        _averageSpeed = _cumulativeMagnitude / _particlesToCalculate.Count;
        //calculate average velocity vector
        _averageVelocity = _averageVelocity / _velocities.Count();

        for(int i = 0; i < _particlesToCalculate.Count; i++)
        {
            var item = _particlesToCalculate.ElementAt(i);
            //if calculated fuse is true remove item from dictionary
            if(item.Value.Calculated)
            {
                _particlesToCalculate.Remove(item.Key);
                i--;
            }
        }
    }
}
