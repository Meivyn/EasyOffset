using UnityEngine;

namespace EasyOffset {
    internal class SwingAnalyzer {
        #region Constructor

        private readonly WeightedList<Vector3> _tipPositions;
        private readonly WeightedList<Vector3> _pivotPositions;
        private readonly WeightedList<Vector3> _localNormals;

        public SwingAnalyzer(int maximalCapacity) {
            _tipPositions = new WeightedList<Vector3>(maximalCapacity);
            _pivotPositions = new WeightedList<Vector3>(maximalCapacity);
            _localNormals = new WeightedList<Vector3>(maximalCapacity);
        }

        #endregion

        #region Reset

        public void Reset() {
            _tipPositions.Clear();
            _pivotPositions.Clear();
            _localNormals.Clear();
            _hasPreviousTipPosition = false;
        }

        #endregion

        #region Update

        public void Update(
            ReeTransform controllerTransform,
            Vector3 tipWorldPosition,
            Vector3 pivotWorldPosition,
            Vector3 initialNormal,
            out Vector3 planePosition,
            out Quaternion planeRotation,
            out float tipDeviation,
            out float pivotDeviation,
            out float pivotHeight,
            out float minimalSwingAngle,
            out float maximalSwingAngle
        ) {
            _tipPositions.Add(tipWorldPosition, 1.0f);
            _pivotPositions.Add(pivotWorldPosition, 1.0f);

            GetSwingData(
                initialNormal,
                out var planeNormal,
                out planePosition,
                out planeRotation,
                out tipDeviation,
                out pivotDeviation,
                out pivotHeight,
                out minimalSwingAngle,
                out maximalSwingAngle
            );

            UpdateLocalNormals(
                controllerTransform,
                tipWorldPosition,
                planeNormal
            );
        }

        #endregion

        #region WristRotationAxis

        private Vector3 _previousTipPosition = Vector3.zero;
        private bool _hasPreviousTipPosition;

        private void UpdateLocalNormals(
            ReeTransform controllerTransform,
            Vector3 tipWorldPosition,
            Vector3 planeNormal
        ) {
            if (_hasPreviousTipPosition) {
                var localNormal = controllerTransform.WorldToLocalDirection(planeNormal);
                var tipVelocity = (tipWorldPosition - _previousTipPosition).magnitude / Time.deltaTime;
                _localNormals.Add(localNormal, Mathf.Pow(tipVelocity, 2.0f));
            }

            _previousTipPosition = tipWorldPosition;
            _hasPreviousTipPosition = true;
        }

        public Vector3 GetWristRotationAxis() {
            return !_hasPreviousTipPosition ? Vector3.forward : _localNormals.GetAverage();
        }

        #endregion

        #region GetSwingData

        private void GetSwingData(
            Vector3 initialNormal,
            out Vector3 planeNormal,
            out Vector3 planePosition,
            out Quaternion planeRotation,
            out float tipDeviation,
            out float pivotDeviation,
            out float pivotHeight,
            out float minimalSwingAngle,
            out float maximalSwingAngle
        ) {
            var averagePivotPosition = _pivotPositions.GetAverage();
            pivotDeviation = _pivotPositions.GetDeviationFromPoint(averagePivotPosition);
            var swingPlane = _tipPositions.CalculatePlane(initialNormal);
            tipDeviation = _tipPositions.GetDeviationFromPlane(swingPlane);
            pivotHeight = swingPlane.GetDistanceToPoint(averagePivotPosition);

            planeNormal = swingPlane.normal;
            planePosition = swingPlane.ClosestPointOnPlane(averagePivotPosition);
            planeRotation = Quaternion.LookRotation(swingPlane.normal, Vector3.up);

            _tipPositions.GetSwingAngles(
                planePosition,
                planeRotation,
                out minimalSwingAngle,
                out maximalSwingAngle
            );
        }

        #endregion
    }
}