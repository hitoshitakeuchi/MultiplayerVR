using UnityEngine;
//using UnityEditor;
using System.Collections;

namespace MovementPlus
{

    [ExecuteInEditMode]
    public class SpeedController : MonoBehaviour
    {

        public float neutralSpeed;
        public float targetSpeed;

        public int tweenIndex;
        public float tweenDuration;
        public bool interval;

        public float intervalTime;
        public float duration;


        private float tweenTime = 0f;
        private float targetTime = 0f;

        public float time = 0f;
        private float lastTimeStamp = 0f;
        public bool inTarget = false;
        public bool inTween = false;

        public bool simulate = false;

        void OnEnable()
        {
            //if (!Application.isPlaying)
            //{
                //EditorApplication.update += EditorUpdate;
            //}
            simulate = false;
        }

        void OnDisable()
        {
            simulate = false;
        }


        void Start()
        {
            time = 0f;
        }

        void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                if (!simulate) return;
                if (!isActiveAndEnabled) return;
                if (transform.GetComponent<MovementController>() == null) return;
                if (transform.GetComponent<MovementController>().simulate)
                {

                    if (!inTween && !inTarget) time += Time.realtimeSinceStartup - lastTimeStamp;
                    if (inTween) tweenTime += Time.realtimeSinceStartup - lastTimeStamp;
                    if (inTarget) targetTime += Time.realtimeSinceStartup - lastTimeStamp;

                    calculateTimeScale();
                }

                lastTimeStamp = Time.realtimeSinceStartup;
            }
        }

        public void reset()
        {
            time = 0;
            tweenTime = 0;
            targetTime = 0;
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (transform.GetComponent<MovementController>() == null) return;
                if (!inTween && !inTarget) time += Time.deltaTime;
                if (inTween) tweenTime += Time.deltaTime;
                if (inTarget) targetTime += Time.deltaTime;
                calculateTimeScale();
            }
        }

        public void toggle()
        {
            if (interval) return;
            inTarget = !inTarget;

            if (tweenIndex == 0)
            {
                if (inTarget) transform.GetComponent<MovementController>().timeScale = targetSpeed;
                else transform.GetComponent<MovementController>().timeScale = neutralSpeed;
            }
            else
            {
                inTween = true;
                time = 0;
                targetTime = 0;
            }
        }

        private void calculateTimeScale()
        {
            if (inTween)
            {
                if (tweenIndex == 0)//None
                {
                    if (inTarget) transform.GetComponent<MovementController>().timeScale = targetSpeed;
                    else transform.GetComponent<MovementController>().timeScale = neutralSpeed;
                    inTween = false;
                }
                else if (tweenIndex == 1)//Linear
                {
                    if (inTarget) transform.GetComponent<MovementController>().timeScale = neutralSpeed + tweenTime / tweenDuration * (targetSpeed - neutralSpeed);
                    else transform.GetComponent<MovementController>().timeScale = targetSpeed + tweenTime / tweenDuration * (neutralSpeed - targetSpeed);
                }
                else if (tweenIndex == 2)//Sine
                {
                    float t = tweenTime / tweenDuration;
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);

                    if (inTarget) transform.GetComponent<MovementController>().timeScale = neutralSpeed + t * (targetSpeed - neutralSpeed);
                    else transform.GetComponent<MovementController>().timeScale = targetSpeed + t * (neutralSpeed - targetSpeed);
                }
                else if (tweenIndex == 3)//Smoothstep x2
                {
                    float t = tweenTime / tweenDuration;
                    t = t * t * (3f - 2f * t);

                    if (inTarget) transform.GetComponent<MovementController>().timeScale = neutralSpeed + t * (targetSpeed - neutralSpeed);
                    else transform.GetComponent<MovementController>().timeScale = targetSpeed + t * (neutralSpeed - targetSpeed);
                }
                else if (tweenIndex == 4)//Smoothstep x3
                {
                    float t = tweenTime / tweenDuration;
                    t = t * t * t * (t * (6f * t - 15f) + 10f);

                    if (inTarget) transform.GetComponent<MovementController>().timeScale = neutralSpeed + t * (targetSpeed - neutralSpeed);
                    else transform.GetComponent<MovementController>().timeScale = targetSpeed + t * (neutralSpeed - targetSpeed);
                }

                if (tweenTime >= tweenDuration)
                {
                    inTween = false;
                    tweenTime = 0f;
                    if (inTarget) transform.GetComponent<MovementController>().timeScale = targetSpeed;
                    else transform.GetComponent<MovementController>().timeScale = neutralSpeed;
                }
            }
            if (interval)
            {
                if (inTarget && !inTween)
                {
                    if (targetTime >= duration)
                    {
                        inTween = true;
                        inTarget = false;
                    }
                }
                else if (time >= intervalTime)
                {
                    time = 0f;
                    inTween = true;
                    inTarget = true;
                    targetTime = 0f;
                }
            }
            if (inTarget)
            {

            }
        }
    }

}
