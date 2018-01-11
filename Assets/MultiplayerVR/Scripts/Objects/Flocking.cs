using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerVR
{
    [RequireComponent(typeof(Rigidbody))]
    [System.Serializable]
    public class Flocking : MonoBehaviour
    {
        public FlockingController _controller;
        FlockingController.Param _p { get { return _controller.param; } }
        Rigidbody _rigidbody;

        public bool drawDebug;

        void Start()
        {
            _controller = GameObject.Find("Flocking").GetComponent<FlockingController>();

            _rigidbody = GetComponent<Rigidbody>();

            //if (_controller == null) _controller = GetComponentInParent<FlockingController>();

            _controller.MustNotBeNull();
            _controller.Add(this);
        }

        public void Update()
        {
            //this.transform.rotation = Quaternion.Euler(0, 0, 0);

            var sep = Separate();
            var ali = Align();
            var coh = Cohesion();
            var fol = Follow();

            //Arbitrarily weight these forces
            sep *= _p.sRate;
            ali *= _p.aRate;
            coh *= _p.cRate;
            fol *= _p.fRate;

            //Add the force vectors to acceleration
            var accelerator = (
                   sep
                   //+ avo
                   + ali
                   + coh
                   + fol
                   );

            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity + accelerator * Time.deltaTime, _p.maxSpeed);
            //if (_rigidbody.velocity.sqrMagnitude > 0f)
            //{
            //    transform.forward = _rigidbody.velocity.normalized;
            //}

            if (drawDebug)
            {
                var scale = 10f;
                var pos = transform.position;

                Debug.DrawRay(pos, sep * scale, Color.cyan);
                Debug.DrawRay(pos, ali * scale, Color.green);
                Debug.DrawRay(pos, coh * scale, Color.blue);
                Debug.DrawRay(pos, fol * scale, Color.yellow);
                Debug.DrawRay(pos, _rigidbody.velocity);
            }
        }

        public void OnDrawGizmosSelected()
        {
            var color = Gizmos.color;
            var pos = transform.position;

            System.Action<float, Color> DrawSphere = (radius, col) =>
            {
                Gizmos.color = col;
                Gizmos.DrawWireSphere(pos, radius);
            };

            if (_controller != null)
            {
                DrawSphere(_p.separateDist, Color.cyan);
                DrawSphere(_p.alignDist, Color.green);
                DrawSphere(_p.cohesionDist, Color.blue);
            }

            Gizmos.color = color;
        }

        Vector3 _DirToSteer(Vector3 dir)
        {
            var steer = Vector3.zero;
            if (dir.sqrMagnitude > 0)
            {
                steer = dir.normalized * _p.maxSpeed;
                steer -= _rigidbody.velocity;
                steer = Vector3.ClampMagnitude(steer, _p.maxForce);
            }
            return steer;
        }


        Vector3 Separate()
        {
            return _DirToSteer(_controller.GetNearData(this).separate_dir);
        }
        
        Vector3 Align()
        {
            var dir = _controller.GetNearData(this).align_dir;
            dir = dir.normalized * _p.maxSpeed;
            var steer = dir - _rigidbody.velocity;
            steer = Vector3.ClampMagnitude(steer, _p.maxForce);

            return steer;
        }

        Vector3 Cohesion()
        {
            return _DirToSteer(_controller.GetNearData(this).cohesion_dir);
        }

        Vector3 Follow()
	    {
            return _DirToSteer(_controller.GetNearData(this).follow_dir);
	    }
    }
}