using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoschingMachine.Kinematics
{
    [System.Serializable]
    public class EntityMovement
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float moveForce;

        [Space]
        [SerializeField] float jumpForce;
        [SerializeField] float jumpSpringFreezeTime;

        [Space]
        [SerializeField] float upGravity;
        [SerializeField] float downGravity;

        [Space]
        [SerializeField] float springDistance;
        [SerializeField] float springCheckRadius;
        [SerializeField] float springForce;
        [SerializeField] float springDamper;

        RaycastHit? groundHit;
        float lastJumpTime;
        bool lastJump;

        public void Move (Rigidbody rb, Vector3 moveDirection, bool jump)
        {
            CheckForGround(rb);

            Vector3 target = moveDirection * maxSpeed;
            Vector3 diff = target - rb.velocity;
            diff.y = 0.0f;
            diff = Vector3.ClampMagnitude(diff, maxSpeed);

            Vector3 force = diff * moveForce * rb.mass;
            rb.AddForce(force);

            if (jump && !lastJump) Jump(rb);
            ApplySpring(rb);

            rb.useGravity = false;

            var gScale = jump && GetRelativeVelocity(rb).y > 0.0f ? upGravity : downGravity;
            rb.AddForce(Physics.gravity * gScale, ForceMode.Acceleration);

            lastJump = jump;
        }

        private void CheckForGround(Rigidbody rb)
        {
            Ray ray = new Ray(rb.position + Vector3.up * (springDistance + springCheckRadius), Vector3.down);
            if (Physics.SphereCast(ray, springCheckRadius, out var hit, springDistance))
            {
                groundHit = hit;
            }
            else groundHit = null;
        }

        private void ApplySpring(Rigidbody rb)
        {
            if (!groundHit.HasValue) return;
            if (Time.time < lastJumpTime + jumpSpringFreezeTime) return;

            var relativeVelocity = GetRelativeVelocity(rb);

            var dist = springDistance - groundHit.Value.distance;

            Vector3 force = Vector3.up * dist * springForce;
            force += Vector3.up * -relativeVelocity.y * springDamper * Time.deltaTime;
            force *= rb.mass;

            rb.AddForce(force);
        }

        private void Jump(Rigidbody rb)
        {
            if (!groundHit.HasValue) return;

            var force = Vector3.up * jumpForce * rb.mass;

            rb.AddForce(force, ForceMode.Impulse);

            var relativeVelocity = GetRelativeVelocity(rb);
            var correctionForce = Vector3.up * -relativeVelocity.y * rb.mass;
            rb.AddForce(correctionForce, ForceMode.Impulse);

            lastJumpTime = Time.time;
        }

        public void TryExecuteOnGround (Action<Rigidbody> callback)
        {
            if (groundHit.HasValue)
            {
                if (groundHit.Value.rigidbody)
                {
                    callback(groundHit.Value.rigidbody);
                }
            }
        }

        public Vector3 GetRelativeVelocity(Rigidbody rb)
        {
            Vector3 velocity = rb.velocity;
            TryExecuteOnGround(g => velocity -= g.velocity);
            return velocity;
        }
    }
}
