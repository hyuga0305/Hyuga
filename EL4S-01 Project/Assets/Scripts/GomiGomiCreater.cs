using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GomiGomiCreater : MonoBehaviour
{
	[System.Serializable]
	public struct GomiGomiInfomation
	{
		public GomiGomiInfomation(GameObject gameObject, float scaler)
		{
			m_gameObject = gameObject;
			m_scaler = scaler;
		}

		public GameObject gameObject { get { return m_gameObject; } }
		public Vector3 scalingLocalScale { get { return m_gameObject.transform.localScale * m_scaler; } }

		[SerializeField]
		GameObject m_gameObject;
		[SerializeField, Range(0.0f, 1.0f)]
		float m_scaler;
	}

	public static GomiGomiCreater instance { get; private set; } = null;

	[SerializeField]
	Vector2 m_stageSize = Vector2.one;
	//[SerializeField]
	//float
	[SerializeField]
	GomiGomiInfomation[] gomiGomis = null;

	float m_timer = 0.0f;

	void Awake()
	{
		instance = this;
		m_timer = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.time - m_timer > 0.1f)
		{
			int id = Random.Range(0, gomiGomis.Length - 1);
			var obj = GameObject.Instantiate(gomiGomis[id].gameObject);

			obj.transform.position = Vector3.up;
			obj.transform.localScale = gomiGomis[id].scalingLocalScale;
			var rigidbody =obj.AddComponent<Rigidbody>();

			rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
			m_timer = Time.time;
		}
    }
}
