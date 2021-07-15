using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;

public partial class BloodVesselManager : SerializedMonoBehaviour
{
    private SignalBus _signalbus;
    private Dictionary<BloodVesselName, BloodVessel> _bloodVessels = new Dictionary<BloodVesselName, BloodVessel>();

    [Inject]
    private void Construct(SignalBus signalBus, List<BloodVessel> bloodVessels)
    {
        _signalbus = signalBus;

        foreach (var item in bloodVessels)
            _bloodVessels.Add(item.BloodVesselName, item);
    }

    public BloodVessel GetBloodVessel(BloodVesselName bloodVesselName)
    {
        return _bloodVessels[bloodVesselName];
    }

    private void Awake()
    {
        _signalbus.Fire(new ActiveBloodVesselSignal(_bloodVessels[BloodVesselName.SuperVenaCava]));
    }

}
