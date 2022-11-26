using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleCollectibleScript : MonoBehaviour {

	

	public bool rotate;
	public float rotationSpeed;
	//public AudioClip collectSound;
	public AudioSource audioSource;

	void Update () {
		if (rotate)
			transform.Rotate (Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) {
			Collect ();
		}
	}

	private void Collect()
	{
		Destroy (gameObject);
	}
}
