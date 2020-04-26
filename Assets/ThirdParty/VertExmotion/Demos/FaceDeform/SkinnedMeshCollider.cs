using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalagaan
{
    public class SkinnedMeshCollider : MonoBehaviour
    {


        Mesh m_mesh;
        SkinnedMeshRenderer m_smr;
        MeshCollider m_mc;

        // Use this for initialization
        void Start()
        {

            m_smr = GetComponent<SkinnedMeshRenderer>();
            m_mc = GetComponent<MeshCollider>();
            if (m_mc == null)
                m_mc = gameObject.AddComponent<MeshCollider>();

            m_mesh = new Mesh();


        }

        // Update is called once per frame
        void Update()
        {

            if (m_smr != null && m_mc != null && m_mesh != null)
            {
                m_smr.BakeMesh(m_mesh);
                m_mc.sharedMesh = m_mesh;
            }
            else
                enabled = false;

        }
    }
}