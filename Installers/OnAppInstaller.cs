using System.Reflection;
using EasyOffset.Configuration;
using JetBrains.Annotations;
using Zenject;

namespace EasyOffset.Installers {
    [UsedImplicitly]
    public class OnAppInstaller : Installer<OnAppInstaller> {
        private static readonly FieldInfo MainSettingsFieldInfo = typeof(PCAppInit).GetField(
            "_mainSettingsModel",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        public static void PreInstall(PCAppInit appInstaller, DiContainer container) {
            var mainSettingsModel = (MainSettingsModelSO)MainSettingsFieldInfo.GetValue(appInstaller);
            container.BindInstance(mainSettingsModel).AsSingle();
            PluginConfig.MainSettingsModel = mainSettingsModel;
        }

        public override void InstallBindings() {
            BindInputManagers();
            BindGizmosManager();
            BindBenchmarkManager();
            BindOffsetManagers();
        }

        private void BindInputManagers() {
            Container.BindInterfacesAndSelfTo<ReeInputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AbominationInputManager>().AsSingle();
        }

        private void BindGizmosManager() {
            Container.BindInterfacesAndSelfTo<GizmosManager>().AsSingle();
            Container.BindExecutionOrder<GizmosManager>(1);
        }

        private void BindBenchmarkManager() {
            Container.BindInterfacesAndSelfTo<SwingBenchmarkManager>().AsSingle();
            Container.BindExecutionOrder<SwingBenchmarkManager>(1);
        }

        private void BindOffsetManagers() {
            Container.BindInterfacesAndSelfTo<BasicAdjustmentModeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PivotOnlyAdjustmentModeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<DirectionOnlyAdjustmentModeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SwingBenchmarkAdjustmentModeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<DirectionAutoAdjustmentModeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<RoomOffsetAdjustmentModeManager>().AsSingle();
        }
    }
}