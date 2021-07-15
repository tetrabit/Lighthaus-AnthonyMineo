using UnityEngine;
using Zenject;

public class LighthausInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<ActiveBloodVesselSignal>();
        Container.DeclareSignal<TelemetrySignal>();
        Container.Bind<CellMeasurement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CellMeasurementSpline>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BloodVessel>().FromComponentsInHierarchy().AsCached();
        Container.Bind<OrbitCamera>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SplineCamera>().FromComponentInHierarchy().AsSingle();
    }
}