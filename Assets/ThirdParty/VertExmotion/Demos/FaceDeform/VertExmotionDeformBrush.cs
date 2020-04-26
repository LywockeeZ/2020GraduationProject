using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{

    [RequireComponent(typeof(VertExmotionSensor))]
    public class VertExmotionDeformBrush : MonoBehaviour
    {


        public enum eInteractionType
        {
            PUSH,
            PULL
        }

        public VertExmotion m_target;
        public eInteractionType m_interactionType;

        public float m_radiusMax = 1;
        public float m_pushStrength = .01f;
        public float m_inflateStrength = .01f;
        public bool m_useTrail = true;
        public float m_trailLengthMax = 2f;
        public float m_resetDeformationSpeed = .2f;

        [Range(0, 1)]
        public float m_brushDensity = .5f;
        float m_radius = 0;


        [Range(0, 1)]
        public float m_directionThreshold = .5f;

        MeshRenderer m_mr;


        VertExmotionSensor m_sensor;
        List<VertExmotionSensor> m_sensorsTrail = new List<VertExmotionSensor>();
        List<VertExmotionSensor> m_sensorsTrailDelete = new List<VertExmotionSensor>();
        Vector3 m_touchDir = Vector3.zero;
        Vector3 m_motionDir = Vector3.zero;
        Vector3 m_lastPosition = Vector3.zero;

        // Use this for initialization
        void Start()
        {

            m_sensor = GetComponent<VertExmotionSensor>();
            m_sensor.m_params.translation.amplitudeMultiplier = 0;

            if (m_target != null)
            {
                //m_target.m_VertExmotionSensors.Add(m_sensor);
                m_target.AddSensor(m_sensor, true);
            }

        }
        /*
        public void ChangeTarget(VertExmotion newtarget)
        {
            if (m_target != null)
                m_target.m_VertExmotionSensors.Remove(m_sensor);

            m_target = newtarget;

            if (m_target != null)
                m_target.m_VertExmotionSensors.Add(m_sensor);
        }
        */

        // Update is called once per frame
        void Update()
        {

            bool touchDetected = false;
            VertExmotionSensor lastFromTrail = null;

            //Raycast
            if (Input.GetMouseButton(0))
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    m_radius += Time.deltaTime;
                    transform.position = hitInfo.point;
                    m_touchDir = (hitInfo.collider.transform.position - hitInfo.point).normalized;

                    if (m_useTrail)
                    {
                        if (m_sensorsTrail.Count == 0)
                        {
                            AddSensorToTrail(hitInfo.point);
                        }


                        lastFromTrail = m_sensorsTrail[m_sensorsTrail.Count - 1];
                        float distanceFromTouch = Vector3.Distance(lastFromTrail.transform.position, hitInfo.point);

                        float f = Mathf.Clamp(1f - m_brushDensity, .01f, 1f);


                        if (distanceFromTouch > m_radiusMax * f)
                        {
                            lastFromTrail.m_envelopRadius = distanceFromTouch;
                            AddSensorToTrail(hitInfo.point);
                            lastFromTrail = m_sensorsTrail[m_sensorsTrail.Count - 1];
                        }
                        lastFromTrail.m_envelopRadius = distanceFromTouch;

                    }



                    if (m_lastPosition != Vector3.zero)
                    {
                        m_motionDir = Vector3.Lerp(m_motionDir, (transform.position - m_lastPosition).normalized, Time.deltaTime * 10f).normalized;
                    }

                    m_lastPosition = transform.position;
                    touchDetected = true;
                }

            }


            m_radius = Mathf.Clamp(m_radius, 0, m_radiusMax);


            if (!touchDetected)
            {
                //No touch -> delete all sensors
                m_radius -= Time.deltaTime;
                m_lastPosition = Vector3.zero;
                m_motionDir = Vector3.zero;

                for (int i = 0; i < m_sensorsTrail.Count; ++i)
                {
                    m_sensorsTrailDelete.Add(m_sensorsTrail[i]);
                }
                m_sensorsTrail.Clear();
            }


            if (m_sensorsTrail.Count > 1)
            {
                //remove sensors when trail longer than m_trailLengthMax
                float trailLength = Vector3.Distance(m_lastPosition, m_sensorsTrail[m_sensorsTrail.Count - 1].transform.position);
                for (int i = m_sensorsTrail.Count - 2; i >= 0; --i)
                {
                    float dist = Vector3.Distance(m_sensorsTrail[i].transform.position, m_sensorsTrail[i + 1].transform.position);                    

                    if (trailLength + dist > m_trailLengthMax)
                    {

                        float newDist = m_trailLengthMax - trailLength;
                        
                        if (newDist > 0.001f)
                        {
                            m_sensorsTrail[i].transform.position = m_sensorsTrail[i + 1].transform.position + (m_sensorsTrail[i].transform.position- m_sensorsTrail[i+1].transform.position).normalized * newDist;
                        }
                        else
                        {
                            m_sensorsTrailDelete.Add(m_sensorsTrail[i]);
                            m_sensorsTrail.RemoveAt(i);
                        }
                        //break;
                    }

                    trailLength += dist;
                }
            }


            //Delete unwanted trail parts smoothly
            for (int i = 0; i < m_sensorsTrailDelete.Count; ++i)
            {
                m_sensorsTrailDelete[i].m_envelopRadius -= Time.deltaTime * m_resetDeformationSpeed;

                float offset = m_sensorsTrailDelete[i].m_params.translation.worldOffset.magnitude;
                offset -= Time.deltaTime * Time.deltaTime * m_resetDeformationSpeed;
                m_sensorsTrailDelete[i].m_params.translation.worldOffset = m_sensorsTrailDelete[i].m_params.translation.worldOffset.normalized * offset;

                //m_sensorsTrailDelete[i].m_params.translation.worldOffset -= m_sensorsTrailDelete[i].m_params.translation.worldOffset.normalized * Time.deltaTime * m_sensorsTrailDelete[i].m_envelopRadius * .8f;

                if (m_sensorsTrailDelete[i].m_envelopRadius < 0)
                //if (m_sensorsTrailDelete[i].m_params.translation.worldOffset.magnitude < 0.001f)
                //if (offset <= 0f)
                {
                    //m_target.m_VertExmotionSensors.Remove(m_sensorsTrailDelete[i]);
                    m_target.RemoveSensor(m_sensorsTrailDelete[i]);
                    Destroy(m_sensorsTrailDelete[i].gameObject);
                    m_sensorsTrailDelete.RemoveAt(i--);
                }
            }

            float scale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;


            switch (m_interactionType)
            {

                case eInteractionType.PUSH:
                    m_sensor.m_params.translation.worldOffset = m_pushStrength * scale * Vector3.Lerp(m_touchDir, m_motionDir, m_directionThreshold);
                    break;

                case eInteractionType.PULL:
                    m_sensor.m_params.translation.worldOffset = m_pushStrength * scale * Vector3.Lerp(-m_touchDir, m_motionDir, m_directionThreshold);
                    break;
            }

            m_sensor.m_params.inflate = m_inflateStrength;
            m_sensor.m_envelopRadius = m_radius;


            if (lastFromTrail != null)
            {
                //update last trail
                lastFromTrail.m_params.inflate = m_inflateStrength;
                lastFromTrail.m_params.translation.worldOffset = m_sensor.m_params.translation.worldOffset;
            }

            for (int i = 0; i < m_sensorsTrail.Count; ++i)
            {
                if (i > 0)
                    Debug.DrawLine(m_sensorsTrail[i - 1].transform.position, m_sensorsTrail[i].transform.position, Color.black);

               

                m_sensorsTrail[i].m_envelopRadius = Mathf.Clamp(Vector3.Distance(m_sensorsTrail[i].transform.position, m_lastPosition), 0, m_radiusMax);
                m_sensorsTrail[i].m_params.translation.worldOffset = m_sensorsTrail[i].m_params.translation.worldOffset.normalized
                    * m_sensor.m_params.translation.worldOffset.magnitude;
            }

        }


        /// <summary>
        /// Create a new sensor to the trail
        /// </summary>
        /// <param name="pos"></param>
        void AddSensorToTrail(Vector3 pos)
        {
            GameObject go = new GameObject("Sensor_" + m_sensorsTrail.Count);
            go.transform.position = pos;
            VertExmotionSensor s = go.AddComponent<VertExmotionSensor>();
            //m_target.Sensors.Add(s);
            s.m_params.translation.amplitudeMultiplier = 0f;
            m_target.AddSensor(s, true);

            m_sensorsTrail.Add(s);
        }
    }
}