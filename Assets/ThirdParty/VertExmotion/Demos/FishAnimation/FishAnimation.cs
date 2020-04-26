using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{

    public class FishAnimation : MonoBehaviour
    {
        [Header("Sensors")]
        public VertExmotionSensor m_head;
        public VertExmotionSensor m_body;
        public VertExmotionSensor m_tail;
        public VertExmotionSensor m_caudalFin;
        public VertExmotionSensor m_pectoralFins;
        public VertExmotionSensor m_dorsalFin;
        public VertExmotionSensor m_global;

        [Header("Settings")]
        public float m_speed = 4f;

        [Header("Head")]
        public float m_headAmplitudeX = .2f;
        public float m_headAngle = 10f;
        public Vector3 m_headScale = Vector3.one;

        [Header("Body")]
        public float m_bodyAmplitudeX = .1f;
        public float m_bodyAmplitudeY = .1f;
        public float m_bodyAngle = 10f;
        public Vector3 m_bodyScale = Vector3.one;

        [Header("Tail")]
        public float m_tailAmplitudeX = 1f;
        public float m_tailAmplitudeY = .1f;
        public float m_tailAngle = 80f;
        public Vector3 m_tailScale = Vector3.one;

        [Header("Fins")]
        public float m_caudalFinAmplitudeX = 1f;
        public float m_pectoralFinsScaleX = 2f;
        public float m_dorsalFinAmplitude = .1f;
        public float m_dorsalFinAngle = 100f;

        [Header("Global")]
        public float m_globalAmplitudeX = 1f;
        public float m_globalAmplitudeZ = .1f;
        public float m_globalAngle = 30f;


        float m_randomDelta = 0f;

        // Use this for initialization
        void Start()
        {
            m_randomDelta = Random.value * 100f;
        }

        // Update is called once per frame
        void Update()
        {

            float sin = Mathf.Sin(Time.time * m_speed + m_randomDelta);
            float sinHalf = Mathf.Sin(Time.time * m_speed * .5f + m_randomDelta);

            float cos = Mathf.Cos(Time.time * m_speed + m_randomDelta);
            float cosHalf = Mathf.Cos(Time.time * m_speed * .5f + m_randomDelta);

            m_head.m_params.rotation.angle = sin * m_headAngle * Mathf.Cos(Time.time * m_speed * 2f);
            m_head.m_params.translation.localOffset.x = sin * m_headAmplitudeX;
            m_head.m_params.scale = m_headScale;


            m_body.m_params.rotation.angle = cos * m_bodyAngle;
            m_body.m_params.translation.localOffset.x = cos * m_bodyAmplitudeX;
            m_body.m_params.translation.localOffset.y = -cos * m_bodyAmplitudeY;
            //move the body position between head and tail -> more organic animation
            m_body.transform.position = Vector3.Lerp(m_head.transform.position, m_tail.transform.position, cosHalf * .05f + .2f  );
            m_body.m_params.scale = m_bodyScale;



            m_tail.m_params.rotation.angle = sin * m_tailAngle;
            m_tail.m_params.translation.localOffset.x = sin * m_tailAmplitudeX;
            m_tail.m_params.translation.localOffset.y = sinHalf * m_tailAmplitudeY;
            m_tail.m_params.translation.localOffset.z = Mathf.Abs(sin) * m_tailAmplitudeX * .1f;
            m_tail.m_params.scale = m_tailScale;

            m_caudalFin.m_params.translation.localOffset.x = sin * m_caudalFinAmplitudeX;
            m_caudalFin.m_params.rotation.angle = sin * m_tailAngle;

            m_pectoralFins.m_params.scale.x = (sin+1f) * m_pectoralFinsScaleX;

            m_dorsalFin.m_params.translation.localOffset.x = - sin * m_dorsalFinAmplitude;
            m_dorsalFin.m_params.rotation.angle = -sin * m_dorsalFinAngle;

            m_global.m_params.rotation.angle = -sin * m_globalAngle;
            m_global.m_params.translation.localOffset.x = sin * m_globalAmplitudeX;
            m_global.m_params.translation.localOffset.z = Mathf.Abs(sin) * m_globalAmplitudeZ;
        }
    }
}