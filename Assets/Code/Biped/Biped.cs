using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Bipedal
{
    public class Biped : MonoBehaviour
    {
        [SerializeField] Transform head;

        public Transform Head => head;

        public Vector2 LookRotation { get; set; }

        protected virtual void Update ()
        {
            LookRotation = new Vector2(LookRotation.x, Mathf.Clamp(LookRotation.y, -90.0f, 90.0f));

            transform.rotation = Quaternion.Euler(0.0f, LookRotation.x, 0.0f);
            head.rotation = Quaternion.Euler(-LookRotation.y, LookRotation.x, 0.0f);
        }
    }
}
