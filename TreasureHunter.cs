using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Mathf;

public class TreasureHunter : MonoBehaviour
{
    // Start is called before the first frame update
    //public Collectible[] collectibles;
    public TreasureHunterInventory inventory;
    //public CollectibleTreasure[] collectiblesInScene;
    public List<CollectibleTreasure> collectiblesInScene;

    public StringIntDictionary strDict;

    public CollectibleIntDictionary collDict;

    int totalPoints = 0;
    int totalItems = 0;
    int d;
    int prevForwardVector;
    int prevYawRelativeToCenter;

    //public GameObject collectible1;
    void Start()
    {
        d = D(Camera.position+prevForwardVector, Camera.position, Camera.position + Camera.forward);
        prevForwardVector = Camera.forward;
        prevYawRelativeToCenter = angleBetweenVectors(Camera.forward,VRTrackingOrigin.position-Camera.position);


        /*
            OLD CODE
        */

        lost = false;
        win = false;
        //testText.text = "WHer eis yext";
        //scream = GetComponent<AudioSource>();

        /*
            OLD CODE
        */

    } //end of start

    // Update is called once per frame
    void Update()
    {
        var howMuchUserRotated = angleBetweenVectors(prevForwardVector,Camera.forward);
        var directionUserRotated = (d(Camera.position+prevForwardVector, Camera.position, Camera.position + Camera.forward)<0)?1:-1;
        var deltaYawRelativeToCenter = prevYawRelativeToCenter-angleBetweenVectors(Camera.forward,VRTrackingOrigin.position-Camera.position);
        var distanceFromCenter = (Camera.position-VRTrackingOrigin.position).magnitude;
        var longestDimensionOfPE = 5;
        var howMuchToAccelerate=((deltaYawRelativeToCenter<0)? -decelerateThreshold [.13]: accelerateThreshold[.30]) * howMuchUserRotated * directionUserRotated * clamp(distanceFromCenter/longestDimensionOfPE/2,0,1);
        VRTrackingOrigin.RotateAround(Camera.position,(0,1,0),howMuchToAccel);
        prevForwardVector=Camera.forward;
        prevYawRelativeToCenter=angleBetweenVectors(Camera.forward,VRTrackingOrigin.position-Camera.position);

    } //end of update

    public int D(Vector3 A,Vector3 B,Vector3 C){ //right < 0 = true, left > 0 = false
        return (A.x - B.x)(C.z - B.z) - (A.z - B.z)(C.x - B.x);
    }

    public int angleBetweenVectors(Vector2 A, Vector2 B){
        return arccos(dot(normalize(A),normalize(B)));
    }


    /*public int directionUserRotated(int d){ //right < 0 = true, left > 0 = false
        return d<0?1:-1;
    }*/

}
