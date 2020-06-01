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
    class MoveDimension {
        [SerializeField]
        MainCharacterControls Controller;
        [SerializeField]
        float MaxSpeed;
        [SerializeField]
        float StartAcceleration;
        [SerializeField]
        float StopAcceleration;
        [SerializeField]
        float ResetGravity;

        internal float Velocity;

        internal void UpdateMovement(float Coefficient) {
            if (Coefficient == 0 && Velocity != 0) {
                if (Mathf.Sign(Velocity + -Mathf.Sign(Velocity) * StopAcceleration * Time.deltaTime) != Mathf.Sign(Velocity)) {
                    Velocity = 0;
                }
                else {
                    Velocity += -Mathf.Sign(Velocity) * StopAcceleration * Time.deltaTime;
                }

            }
            else {
                Velocity += Coefficient * Time.deltaTime *
                ((Mathf.Sign(Coefficient) != Mathf.Sign(Velocity)) ? StopAcceleration : StartAcceleration);
            }
            if (MaxSpeed != 0) {
                Velocity = Mathf.Clamp(Velocity, -MaxSpeed, MaxSpeed);
            }

        }

        internal void Move(Vector3 Direction) {
            //Controller.Move(Direction * Velocity * Time.deltaTime);
        }
        internal void ResetVelocity() {
            Velocity = ResetGravity;
        }
    }



}