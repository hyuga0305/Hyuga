using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GomiGomiManager : MonoBehaviour
{
	[System.Serializable]
	public struct GomiGomiInfomation
	{
		public GomiGomiInfomation(GameObject gameObject, float scaler, float mass)
		{
			rigidbody = null;
			collisionSize = Vector2.zero;
			collisionSizeDiv2 = Vector2.zero;

			m_gameObject = gameObject;
			m_scaler = scaler;
			m_mass = mass;
		}

		public GameObject gameObject { get { return m_gameObject; } }
		public Rigidbody rigidbody { get; private set; }
		public Vector3 scalingLocalScale { get { return m_gameObject.transform.localScale * m_scaler; } }
		public Vector2 collisionSize { get; private set; } 
		public Vector2 collisionSizeDiv2 { get; private set; }
		public float mass { get { return m_mass; } }

		[SerializeField]
		GameObject m_gameObject;
		[SerializeField, Range(0.0f, 1.0f)]
		float m_scaler;
		[SerializeField]
		float m_mass;

		public void SetRigidBody()
		{
			rigidbody = gameObject.GetComponent<Rigidbody>();
			if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
		}

		public void SetCollisionSize()
		{
			Vector3 scale = gameObject.transform.lossyScale;

			var box = gameObject.GetComponent<BoxCollider>();
			if (box != null)
			{
				collisionSize = new Vector2(scale.x * box.size.x, scale.z * box.size.z);
				collisionSizeDiv2 = collisionSize * 0.5f;
				return;
			}

			var sphere = gameObject.GetComponent<SphereCollider>();
			if (sphere != null) 
			{
				collisionSizeDiv2 = new Vector2(sphere.radius, sphere.radius);
				collisionSize = collisionSizeDiv2 * 2.0f;
				return;
			}

			var capsule = gameObject.GetComponent<CapsuleCollider>();
			if (capsule != null)
			{
				switch (capsule.direction)
				{
					case 0://X
						collisionSize = new Vector2(scale.x * (capsule.height + capsule.radius), scale.z * capsule.radius);
						break;
					case 1://Y
						collisionSize = new Vector2(scale.x * capsule.radius, scale.z * capsule.radius);
						break;
					case 2://Z
						collisionSize = new Vector2(scale.x * capsule.radius, scale.z * (capsule.height + capsule.radius));
						break;
				}
				collisionSizeDiv2 = collisionSize * 0.5f;
				return;
			}

			var mesh = gameObject.GetComponent<MeshCollider>();
			if (mesh != null)
			{
				collisionSize = mesh.sharedMesh.bounds.center + mesh.sharedMesh.bounds.size;
				collisionSizeDiv2 = collisionSize * 0.5f;
				return;
			}

			collisionSize = collisionSizeDiv2 = new Vector2(0.1f, 0.1f);
		}
	}

	public static GomiGomiManager instance { get; private set; } = null;

	[SerializeField]
	Vector2 m_stageSize = Vector2.one;
	[SerializeField]
	int m_numInstantiate = 100;
	[SerializeField]
	GomiGomiInfomation[] m_gomiGomis = null;

	Vector2 m_scalingStageSize = Vector2.zero;

	void Awake()
	{
		instance = this;
		m_scalingStageSize = m_stageSize * 0.5f;

		Vector3[] instantiatePoints = new Vector3[m_numInstantiate];
		int[] instantiateIndexes = new int[m_numInstantiate];

		foreach(var gomi in m_gomiGomis)
		{
			gomi.SetRigidBody();

			gomi.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			gomi.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			gomi.rigidbody.mass = gomi.mass;
		}

		for (int i = 0; i < m_numInstantiate; ++i)
		{
			int id = Random.Range(0, m_gomiGomis.Length - 1);
			var obj = GameObject.Instantiate(m_gomiGomis[id].gameObject);

			Vector3 position = new Vector3(
				Random.Range(-m_scalingStageSize.x, m_scalingStageSize.x), 0.1f, Random.Range(-m_scalingStageSize.y, m_scalingStageSize.y));

			for (int k = 0, resetCounter = 0; k < i && resetCounter < m_numInstantiate; ++k)
			{
				//AABB
				if (instantiatePoints[k].x - m_gomiGomis[instantiateIndexes[k]].collisionSizeDiv2.x
					> position.x + m_gomiGomis[id].collisionSizeDiv2.x) continue;
				if (instantiatePoints[k].x + m_gomiGomis[instantiateIndexes[k]].collisionSizeDiv2.x
					< position.x - m_gomiGomis[id].collisionSizeDiv2.x) continue;
				if (instantiatePoints[k].z - m_gomiGomis[instantiateIndexes[k]].collisionSizeDiv2.y
					> position.z + m_gomiGomis[id].collisionSizeDiv2.y) continue;
				if (instantiatePoints[k].z + m_gomiGomis[instantiateIndexes[k]].collisionSizeDiv2.y
					< position.z - m_gomiGomis[id].collisionSizeDiv2.y) continue;

				position = new Vector3(
					Random.Range(-m_scalingStageSize.x, m_scalingStageSize.x), 0.1f, Random.Range(-m_scalingStageSize.y, m_scalingStageSize.y));
				++resetCounter;
				k = 0;
			}

			obj.transform.position = new Vector3(
				Random.Range(-m_scalingStageSize.x, m_scalingStageSize.x), 0.1f, Random.Range(-m_scalingStageSize.y, m_scalingStageSize.y));

			obj.transform.localScale = m_gomiGomis[id].scalingLocalScale;

			var rigidbody = obj.GetComponent<Rigidbody>();

			rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
		}
	}

    // Update is called once per frame
    void Update()
    {
    }

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(new Vector3(0.0f, 0.5f, 0.0f), new Vector3(m_stageSize.x, 0.5f, m_stageSize.y));		
	}
#endif
}
