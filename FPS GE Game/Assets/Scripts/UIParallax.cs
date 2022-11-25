using UnityEngine;

public class UIParallax : MonoBehaviour {
    
    private Vector3 pz;
    private Vector3 StartPos;
     
    public float moveModifier;
     
    void Start ()
    {
        StartPos = transform.position;
    }
     
    void Update ()
    {
        var pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        pz.z = 0;
        gameObject.transform.position = pz;
        transform.position = new Vector3(StartPos.x + (pz.x * moveModifier), StartPos.y + (pz.y * moveModifier), 0);
    }
     
}
