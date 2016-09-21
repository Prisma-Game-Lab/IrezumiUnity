using UnityEngine;
using System.Collections;

public class RotatingPlatformController : MonoBehaviour {
    /*variaveis:
        tempo atual
        tempo de espera
        angulo maximo de rotacao
        rotacao atual
        sentido da rotacao
        velocidade da rotacao*/
    public float waitingTime;
    public float rotatingSpeed; //initial speed of rotation, degrees per second
    public float acceleration; //acceleration of rotation
    public int sense; //-1 to clockwise, 1 to anticlockwise

    private bool isRotating = false;
    private bool rotated = false;
    private Vector3 pivot;
    private Renderer sprite;
    private float speed;
    private float time;

	// Use this for initialization
	void Start ()
	{
	    speed = rotatingSpeed;
        sprite = transform.GetComponent<Renderer>();
        
        /*pivot is the inferior left(sense == -1) or right(sense == 1) corner of the platform*/
        pivot = transform.position;
        pivot.x += sense * sprite.bounds.extents.x;
	    pivot.y -= sprite.bounds.extents.y;
	}
	
	// Update is called once per frame
	void Update () {
	    if (isRotating)
	    {
	        time = Time.deltaTime;
            transform.RotateAround(pivot, Vector3.forward, speed * sense * time);
	        speed += acceleration*time;
	        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
	        {
	            rotated = true;
                isRotating = false;
	        }
	    }
	}

    IEnumerator Rotate(float time)
    {
        /*espera waitingTime segundos*/
        yield return new WaitForSeconds(time);
        //if ((sense == -1 && transform.eulerAngles.z < 270) || (sense == 1 && transform.eulerAngles.z < 90))
        {
            isRotating = true;
            //Debug.Log("isRotating = true");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!rotated)
            StartCoroutine(Rotate(waitingTime));
    }
}
