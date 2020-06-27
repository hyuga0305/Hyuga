using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashbinController : MonoBehaviour
{
	public Vector3 moveDirection
	{
		get { return m_moveDirection; }
		set
		{
			m_moveDirection = value;
			m_isDirectionChange = true;
		}
	}

	[SerializeField]
	Rigidbody m_rigidBody = null;
	[SerializeField]
	Vector3 m_awakeForce = Vector3.zero;
	[SerializeField]
	float m_moveStartImpluse = 0.0f;
	[SerializeField]
	float m_moveForce = 0.0f;

	Vector3 m_moveDirection = Vector3.zero;
	bool m_isDirectionChange = false;

	void FixedUpdate()
	{
		if (m_isDirectionChange)
		{
			m_isDirectionChange = false;
			m_rigidBody.velocity = Vector3.zero;
			m_rigidBody.AddForce(m_moveDirection * m_moveStartImpluse, ForceMode.Impulse);
		}

		m_rigidBody.AddForce(m_moveDirection * m_moveForce, ForceMode.Force);
	}

	// Start is called before the first frame update
	void Start()
    {
		m_rigidBody.AddForce(m_awakeForce, ForceMode.Impulse);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
