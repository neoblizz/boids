using UnityEngine;
using System.Collections;
 
public class UnityFlockController : MonoBehaviour {
 
    public Vector3 offset;
    public Vector3 range;
    public Vector3 bound;

    public float speed = 3.0f;
 
    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;
 
	void Start () {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
 
    }
	
 
	void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);
 
        if (Vector3.Distance(nextMovementPoint,transform.position) <= 10.0f)
        {
            CalculateNextMovementPoint();
        }
	}
 
    void CalculateNextMovementPoint()
    {
        float posX = Random.Range(initialPosition.x - range.x, initialPosition.x + range.x);
        float posY = Random.Range(initialPosition.y - range.y, initialPosition.y + range.y);
        float posZ = Random.Range(initialPosition.z - range.z, initialPosition.z + range.z);
        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
        //if(nextMovementPoint.x > bound.x || nextMovementPoint.x < bound.x) nextMovementPoint.x = bound.x;
        //if(nextMovementPoint.y > bound.y || nextMovementPoint.y < bound.y) nextMovementPoint.y = bound.y;
        //if(nextMovementPoint.x > bound.z || nextMovementPoint.x < bound.z) nextMovementPoint.x = bound.z;

    }
}