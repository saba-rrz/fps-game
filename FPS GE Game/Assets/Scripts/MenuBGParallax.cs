using UnityEngine;


public class MenuBGParallax : MonoBehaviour
{
  private Vector2 _startPos;
  [SerializeField] private int moveModifier;
  private Camera _camera;

  private void Start()
  {
    _camera = Camera.main;
    _startPos = transform.position;
  }

  private void Update()
  {
    Vector2 pz = _camera!.ScreenToViewportPoint(Input.mousePosition);

    var position = transform.position;
    
    float posX = Mathf.Lerp(position.x, _startPos.x + (moveModifier * pz.x), 2f * Time.deltaTime);
    float posY = Mathf.Lerp(position.y, _startPos.y + (moveModifier * pz.y), 2f * Time.deltaTime);

    transform.position = new Vector3(posX, posY, 0f);
  }
}
