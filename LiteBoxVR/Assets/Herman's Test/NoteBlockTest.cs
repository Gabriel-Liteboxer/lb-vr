using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBlockTest : MonoBehaviour
{

    [SerializeField] private Material m_NormalMaterial;
    [SerializeField] private float m_NoteMoveSpeed;
    [SerializeField] private float m_NoteDissolveSpeed;
    [SerializeField] private GameObject m_NoteExplosion;
    private Material m_DissolveMaterial;

    private float m_DissolveAmount = 0;
    private bool m_SolidMatSet = false;

    private float m_DestroyTimer = .5f;


    private void Awake()
    {
        m_DissolveMaterial = GetComponent<Renderer>().material;
        m_DissolveMaterial.SetFloat("_DissolveAmount", 1);
        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(transform.position.x, transform.position.y, transform.position.z + .8f), "time", m_NoteMoveSpeed, "easetype", iTween.EaseType.easeInExpo));

    }

    void Update()
    {
        m_DissolveAmount = m_DissolveMaterial.GetFloat("_DissolveAmount");
        if (m_DissolveAmount > .005f)
        {
            m_DissolveMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(m_DissolveAmount, 0, Time.deltaTime * m_NoteDissolveSpeed));
        }
        else if (!m_SolidMatSet)
        {
            m_SolidMatSet = true;
            GetComponent<Renderer>().material = m_NormalMaterial;
        }
        if (m_SolidMatSet)
        {
            m_DestroyTimer -= Time.deltaTime;
        }
        if (m_DestroyTimer <= 0)
        {
            Instantiate(m_NoteExplosion, transform.position, Quaternion.identity);
            FloorManager.Instance.PlayFloorAnim();
            Destroy(gameObject);
        }

    }
}
