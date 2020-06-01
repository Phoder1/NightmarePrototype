using System;
using UnityEngine;
namespace MoveTools {
    [System.Serializable]
    class RotationDimension {
        [SerializeField]
        internal Transform Transform;
        [SerializeField]
        internal float Coefficient;

        internal float Angle;

        [SerializeField]
        internal float MinDegree;
        [SerializeField]
        internal float MaxDegree;
        internal void UpdateAngle(float value) {
            Angle += value * Coefficient * Time.deltaTime;
            if (MinDegree != 0 || MaxDegree != 0) {
                Angle = Mathf.Clamp(Angle, MinDegree, MaxDegree);
            }

        }
    }

    [System.Serializable]



}