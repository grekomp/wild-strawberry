using UnityEngine;

public class Dot : MonoBehaviour {

	public Vector3 targetPosition;

	// Options
	public Color color;
	public float transitionSpeed;

	public bool mouseDown;
	public bool mouseLeft;

	SpriteRenderer spriteRenderer;

	private void Start()
	{
		// Getting spriteRenderer reference
		spriteRenderer = GetComponent<SpriteRenderer>();

		// Setting color
		spriteRenderer.color = color;
	}

	private void Update()
	{
		// Transitioning to target position
		transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

		// Check if mouse was released
		if (mouseLeft && !Input.GetMouseButton(0))
		{
			mouseDown = false;
			mouseLeft = false;
		}
	}

	private void OnMouseDown()
	{
		mouseDown = true;
		mouseLeft = false;
	}

	private void OnMouseUp()
	{
		if (mouseDown)
		{
			mouseDown = false;

			Board.Clicked(this);
		}
	}

	private void OnMouseExit()
	{
		mouseLeft = true;
	}

	public void Pop()
	{
		Destroy(gameObject);
	}


}
