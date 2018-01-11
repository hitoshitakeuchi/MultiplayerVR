using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerVR
{
    public class FlockingController : MonoBehaviour
    {
        [System.Serializable]
        public class Param
        {
            public float drag = 0.8f;
            public float maxSpeed = 10f;
            public float maxForce = 0.003f;    // Maximum steering force

            public float separateDist = 1f;
            public float alignDist = 2f;
            public float cohesionDist = 5f;

            public float sRate = 1f;
            public float aRate = 0.5f;
            public float cRate = 0.4f;
            public float fRate = 0.8f;

            public float effectiveDistance
            {
                get { return Mathf.Max(separateDist, alignDist, cohesionDist); }
            }
        }


        public class NearData
        {
            public Vector3 separate_dir;
            public Vector3 align_dir;
            public Vector3 cohesion_dir;

            public Vector3 follow_dir;
        }


        public List<GameObject> _followTargets;
        public Param param;
        

        List<Flocking> _flockings = new List<Flocking>();

        bool _nears_valid;
        Dictionary<Flocking, NearData> _nears = new Dictionary<Flocking, NearData>();

        public void Add(Flocking f)
        {
            _flockings.Add(f);
        }

        bool CreateNears()
        {
            var max_distance = param.effectiveDistance;

            if (_flockings.Any())
            {
                _flockings = _flockings.Where(f => f != null).ToList();

                _flockings.ForEach(f =>
                {
                    if (!_nears.ContainsKey(f)) _nears[f] = new NearData();
                });


                var _pos_dir_list = _flockings.Select(f => new { pos = f.transform.position, dir = f.transform.forward }).ToList();

                _followTargets = _followTargets.Where(f => f != null).ToList();
                var _follow_pos_list = _followTargets.Any() ? _followTargets.Select(f => f.transform.position).ToList() : new List<Vector3>();

                UnityThreading.Parallel.For(0, _pos_dir_list.Count, (idx) =>
                {
                    var me = _flockings[idx];
                    var pos = _pos_dir_list[idx].pos;
                    var near = _nears[me];

                    var separate_dir = Vector3.zero;
                    var align_dir = _pos_dir_list[idx].dir;
                    var cohesion_dir = Vector3.zero;

                    for (var i = 0; i < _flockings.Count; ++i)
                    {
                        if (i != idx)
                        {
                            var other_pos = _pos_dir_list[i].pos;
                            var d = Vector3.Distance(pos, other_pos);
                            if (d <= max_distance)
                            {
                                var to_other = other_pos - pos;

                                if (d <= param.separateDist)
                                {
                                    separate_dir += -to_other.normalized;
                                }

                                if (d <= param.alignDist)
                                {
                                    align_dir += _pos_dir_list[i].dir;
                                }

                                if (d <= param.cohesionDist)
                                {
                                    cohesion_dir += to_other;
                                }
                            }
                        }
                    }

                    var follow_min_dist = float.MaxValue;
                    var follow_dir = Vector3.zero;
                    for(var i=0; i<_follow_pos_list.Count; ++i)
                    {
                        var to_follow = _follow_pos_list[i] - pos;
                        if ( to_follow.magnitude < follow_min_dist)
                        {
                            follow_min_dist = to_follow.magnitude;
                            follow_dir = to_follow;
                        }
                    }

                    near.separate_dir = separate_dir.normalized;
                    near.align_dir = align_dir.normalized;
                    near.cohesion_dir = cohesion_dir.normalized;
                    near.follow_dir = follow_dir.normalized;

                });
            }

            return true;
        }

        public NearData GetNearData(Flocking f)
        {
            if (!_nears_valid) _nears_valid = CreateNears();
            return _nears[f];
        }

        public void LateUpdate()
        {
            _nears_valid = false;
        }
    }
}