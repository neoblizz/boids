using UnityEngine;
using System.Collections;
 
public class UnityFlock : MonoBehaviour {
 
    public float minSpeed = 20;
    public float turnSpeed = 20;
    public float randomFreq = 20;
    public float randomForce = 20;
 
    public float toOriginForce = 10;
    public float toOriginRange = 10;
 
    public float gravity = 2;
 
    //speration vars
    public float avoidanceRadius = 50;
    public float avoidanceForce = 20;
 
    //cohesion vars
    public float followVelocity = 4;
    public float followRadius = 40;
 
    private Transform manager;
    private Vector3 velocity;
    private Vector3 normalizeedVelocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    
    private Transform[] objects;
    private UnityFlock[] otherFlocks;
    private Transform transformComponent;
 
 
	void Start () {
        randomFreq = 1.0f / randomFreq;
 
        manager = transform.parent;

        //Flock transform
        transformComponent = transform;
 
        //Temporary components
        Component[] tempFlocks = null;
 
        //Get all the unity flock omponents from the parent transform in the group
        if (transform.parent )
        {
            tempFlocks = manager.GetComponentsInChildren<UnityFlock>();
        }
 
        //Assign and store all the flock objects in this group
        objects = new Transform[tempFlocks.Length];
        otherFlocks = new UnityFlock[tempFlocks.Length];
        for (int i = 0; i < tempFlocks.Length; i++)
        {
            objects[i] = tempFlocks[i].transform;
            otherFlocks[i] = (UnityFlock)tempFlocks[i];
        }
 
        //Null Parent as the flok leader will be UnityFlockController object
        transform.parent = null;
 
        //Calculate random push depends on the random frequency provided
        StartCoroutine(UpdateRandom());
	}
 
    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreq + Random.Range(-randomFreq / 2.0f, randomFreq / 2.0f));
        }
    }
 
	void Update () {
        //internal variables
        float speed = velocity.magnitude;
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        float count = 0;
        float f = 0.0f;
        float d = 0.0f;
        Vector3 myPosition = transformComponent.position;
        Vector3 forceV;
        Vector3 toAvg;
        Vector3 wantedVel;
        for (int i = 0; i < objects.Length; i++)
        {
            Transform transform = objects[i];
            if (transform != transformComponent)
            {
                Vector3 otherPosition = transform.position;
                //Average position to calculate cohesion
                avgPosition += otherPosition;
                count++;
                //Directional vector from other flock to this flock
                forceV = myPosition - otherPosition;
                //Magnitude of that diretional vector(length)
                d = forceV.magnitude;
                //Add push value if the magnitude,the length of the vetor,is less than followRadius to the leader
                if (d < followRadius)
                {
                    //calculate the velocity,the speed of the object,based current magnitude is less than the specified avoidance radius
                    if (d < avoidanceRadius)
                    {
                        f = 1.0f - (d / avoidanceRadius);
                        if (d > 0)
                        {
                            avgVelocity += (forceV / d) * f * avoidanceForce;
                        }
                    }
                    //just keep the current distance with the leader
                    f = d / followRadius;
                    UnityFlock otherSealgull = otherFlocks[i];
                    //we normalize the otherSealgull veloity vector to get the direction of movement,then wo set a new veloity
                    avgVelocity += otherSealgull.normalizeedVelocity * f * followVelocity;
                }
            }
        }
 
        if (count > 0)
        {
            //Calculate the aveage flock veloity(Alignment)
            avgVelocity /= count;
            //Calculate Center value of the flock（Cohesion)
            toAvg = (avgPosition / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }
        //Directional Vector to the leader
        forceV = manager.position - myPosition;
        d = forceV.magnitude;
        f = d / toOriginRange;
        //Calculate the velocity of the flok to the leader
        if (d > 0)
        {
            originPush = (forceV / d) * f * toOriginForce;
        }
 
        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }
 
        wantedVel = velocity;
        //Calculate final velocity
        wantedVel -= wantedVel * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += originPush * Time.deltaTime;
        wantedVel += avgVelocity * Time.deltaTime;
        wantedVel += toAvg.normalized * gravity * Time.deltaTime;
        //Final Velocity to rotate the flock into
        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.0f);
 
        transformComponent.rotation = Quaternion.LookRotation(velocity);
 
        transformComponent.Translate(velocity * Time.deltaTime, Space.World);
 
        normalizeedVelocity = velocity.normalized;
	}
}

