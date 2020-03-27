﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public enum AttachmentRule { KeepRelative, KeepWorld, SnapToTarget }

public class TreasureHunter : MonoBehaviour
{
    // Start is called before the first frame update
    //public Collectible[] collectibles;
    public TreasureHunterInventory inventory;
    //public CollectibleTreasure[] collectiblesInScene;
    public List<CollectibleTreasure> collectiblesInScene;

    public StringIntDictionary strDict;

    public CollectibleIntDictionary collDict;

    //public Text scoreText;
    public GameObject leftPointerObject;
    public GameObject rightPointerObject;

    //public TextMesh text;
    //public TextMesh debugText;

    public TextMesh testText;
    Vector3 previousPointerPos; //using this for velocity since Unity's broken physics engine won't give it to me otherwise
    CollectibleTreasure thingIGrabbed;
    public LayerMask collectiblesMask;
    public Light light;
    //public GameObject ground;  
    private bool lost;
    private bool win;

    //private AudioSource scream;

    int totalPoints = 0;
    int totalItems = 0;
    int d;

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
        howMuchUserRotated = angleBetweenVectors(prevForwardVector,Camera.forward);
        directionUserRotated = (d(Camera.position+prevForwardVector, Camera.position, Camera.position + Camera.forward)<0)?1:-1;
        deltaYawRelativeToCenter = prevYawRelativeToCenter-angleBetweenVectors(Camera.forward,VRTrackingOrigin.position-Camera.position);
        distanceFromCenter = (Camera.position-VRTrackingOrigin.position).magnitude;
        longestDimensionOfPE = 5;
        howMuchToAccelerate=((deltaYawRelativeToCenter<0)? -decelerateThreshold [.13]: accelerateThreshold[.30]) * howMuchUserRotated * directionUserRotated * clamp(distanceFromCenter/longestDimensionOfPE/2,0,1);
        VRTrackingOrigin.RotateAround(Camera.position,(0,1,0),howMuchToAccel);
        prevForwardVector=Camera.forward;
        prevYawRelativeToCenter=angleBetweenVectors(Camera.forward,VRTrackingOrigin.position-Camera.position);


        /*
            OLD CODE
        */

        if (!lost)
        {
            if (!win)
            {
                if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                {
                    //RaycastHit outHit;
                    forceGrab(false);
                    //if (Physics.Raycast(leftPointerObject.transform.position, leftPointerObject.transform.forward, out outHit, 100.0f))
                    //{   

                    /*totalPoints += outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Value;
                    totalItems++;
                    //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f, Color.yellow);
                    print(outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Name);
                    if (!strDict.ContainsKey(outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Name))
                    {
                        strDict.Add(outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Name, 1);
                    }
                    else
                    {
                        strDict[outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Name] = strDict[outHit.collider.gameObject.GetComponent<CollectibleTreasure>().Name] + 1;
                    }
                    Destroy(outHit.collider.gameObject);
                    */
                    //text.text = "You have " + totalItems + " items worth " + totalPoints + " points! -Evan & Lily";
                    //}
                    // change text.text
                }
                else if (OVRInput.GetDown(OVRInput.RawButton.X))
                {
                    Collider[] overlappingThings = Physics.OverlapSphere(leftPointerObject.transform.position, 10, collectiblesMask);
                    if (overlappingThings.Length > 0)
                    {
                        CollectibleTreasure nearestCollectible = getClosestHitObject(overlappingThings);
                        attachGameObjectToAChildGameObject(nearestCollectible.gameObject, leftPointerObject, AttachmentRule.SnapToTarget, AttachmentRule.SnapToTarget, AttachmentRule.KeepWorld, true);
                        //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                        thingIGrabbed = nearestCollectible.gameObject.GetComponent<CollectibleTreasure>();
                    }
                    //scoreText.text = "You have " + totalItems + " items worth " + totalPoints + " points! -Evan & Lily";

                }
                else if (OVRInput.GetDown(OVRInput.RawButton.Y))
                {
                    forceGrab(true);
                }
                else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstick))
                {
                    Collider[] overlappingThings = Physics.OverlapSphere(leftPointerObject.transform.position, 0.01f, collectiblesMask);
                    if (overlappingThings.Length > 0)
                    {
                        attachGameObjectToAChildGameObject(overlappingThings[0].gameObject, leftPointerObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, true);
                        //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                        thingIGrabbed = overlappingThings[0].gameObject.GetComponent<CollectibleTreasure>();
                    }

                }
                else if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || OVRInput.GetUp(OVRInput.RawButton.X) || OVRInput.GetUp(OVRInput.RawButton.Y) || OVRInput.GetUp(OVRInput.RawButton.LThumbstick))
                {
                    // if position is at belt
                    // CenterEyeAnchor VR camera in unity 

                    var relDiffPos = Camera.main.transform.InverseTransformPoint(thingIGrabbed.gameObject.transform.position);
                    //text.text = "coin let go";

                    if (relDiffPos.y < -0.2 && relDiffPos.y > -1.5 && relDiffPos.x < 1 && relDiffPos.x > -1 && relDiffPos.z < 1 && relDiffPos.z > -1)
                    {
                        //debugText.text = thingIGrabbed.Name;

                        updateScore(thingIGrabbed);
                        if (thingIGrabbed.isCursed)
                        {

                            GameObject ground = GameObject.Find("/Static/Ground");
                            light.intensity = 8f;
                            light.color = Color.red;
                            Camera.main.backgroundColor = Color.black;
                            thingIGrabbed.gameObject.GetComponent<AudioSource>().Play();
                            Destroy(thingIGrabbed.gameObject, thingIGrabbed.gameObject.GetComponent<AudioSource>().clip.length);
                            thingIGrabbed = null;
                            lost = true;
                            Destroy(ground);
                        }
                        else
                        {
                            Destroy(thingIGrabbed.gameObject);
                            thingIGrabbed = null;
                        }

                    }
                    /*
                                var relDiffPos = Camera.main.transform.position;
                    var objectPosition = leftPointerObject.gameObject.transform.position;
                    var d = 0.2; 
                    var waist = (Camera.main.transform.position.x, Camera.main.transform.position.y-d, Camera.main.transform.position.z);
                    // d is y coordinate of object position; waist area (x, y-d, z)
                    if () {

                    }

                    */
                    else
                    {

                        letGo();
                    }
                }
                if (win)
                {
                    testText.text = "Winner";
                }
            }
            previousPointerPos = leftPointerObject.gameObject.transform.position;
        }
        else
        {
            testText.text = "Loser";
        }

        /*
            OLD CODE
        */

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

    //d(Vector3/Vector2 A,Vector3/Vector2 B,Vector3/Vector2 C)=(A.x−B.x)(C.y−B.y)−(A.y−B.y)(C.x−B.x)
        //d(Camera.position+prevForwardVector, Camera.position, Camera.position + Camera.forward)


    /*
        OLD CODE
    */

    void letGo()
    {
        if (thingIGrabbed)
        {
            Collider[] overlappingThingsWithLeftHand = Physics.OverlapSphere(leftPointerObject.transform.position, 0.01f, collectiblesMask);
            detachGameObject(thingIGrabbed.gameObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld);
            simulatePhysics(thingIGrabbed.gameObject, (leftPointerObject.gameObject.transform.position - previousPointerPos) / Time.deltaTime, true);
            thingIGrabbed = null;
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent = null;
        handleAttachmentRules(GOToDetach, locationRule, rotationRule, scaleRule);
    }

    public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule)
    {
        GOToHandle.transform.localPosition =
        (locationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.position :
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localPosition :
        new Vector3(0, 0, 0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles =
        (rotationRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.eulerAngles :
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localEulerAngles :
        new Vector3(0, 0, 0);

        GOToHandle.transform.localScale =
        (scaleRule == AttachmentRule.KeepRelative) ? GOToHandle.transform.lossyScale :
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule == AttachmentRule.KeepWorld) ? GOToHandle.transform.localScale :
        new Vector3(1, 1, 1);
    }

    public void simulatePhysics(GameObject target, Vector3 oldParentVelocity, bool simulate)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb)
        {
            if (!simulate)
            {
                Object.Destroy(rb);
            }
        }
        else
        {
            if (simulate)
            {
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB = target.AddComponent<Rigidbody>();
                newRB.velocity = oldParentVelocity;
            }
        }
    }
    void forceGrab(bool pressedA)
    {
        RaycastHit outHit;
        //notice I'm using the layer mask again
        if (Physics.Raycast(leftPointerObject.transform.position, leftPointerObject.transform.forward, out outHit, 100.0f, collectiblesMask))
        {
            AttachmentRule howToAttach = pressedA ? AttachmentRule.KeepWorld : AttachmentRule.SnapToTarget;
            attachGameObjectToAChildGameObject(outHit.collider.gameObject, leftPointerObject.gameObject, howToAttach, howToAttach, AttachmentRule.KeepWorld, true);
            thingIGrabbed = outHit.collider.gameObject.GetComponent<CollectibleTreasure>();
        }
    }
    public void updateScore(CollectibleTreasure treasure)
    {
        //debugText.text = treasure.GetComponent<CollectibleTreasure>().Name;
        totalPoints += treasure.Value;
        totalItems++;
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f, Color.yellow);
        //print(treasure.Name);
        /*if (!strDict.ContainsKey(treasure.Name))
        {
            strDict.Add(treasure.Name, 1);
        }
        else {
            strDict[treasure.Name] = strDict[treasure.Name] + 1;
        }*/
        //testText.text = "Updating Score";
        string name = treasure.Name;
        //CollectibleTreasure tres2 = Resources.Load(name) as CollectibleTreasure;
        var tres2 = Resources.Load<CollectibleTreasure>(name);
        //debugText.text = tres2.Name + tres2.Value;
        //if (tres2.isCursed){
        //    scream.Play();
        //}
        if (!collDict.ContainsKey(tres2))
        {
            collDict.Add(tres2, 1);
        }
        else
        {
            collDict[tres2] = collDict[tres2] + 1;
        }
        string message = "Item | Count | Value\n";
        foreach (CollectibleTreasure key in collDict.Keys)
        {
            message += key.Name + " | " + collDict[key] + " | " + key.Value + "\n";
        }
        message += "You have " + totalItems + " items worth " + totalPoints + " points! -Evan & Lily";
        testText.text = message;
        if (totalItems >= 5)
        {
            win = true;
        }
        //text.text = "You have " + totalItems + " items worth " + totalPoints + " points! -Evan & Lily";

    }
    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld)
    {
        GOToAttach.transform.parent = newParent.transform;
        handleAttachmentRules(GOToAttach, locationRule, rotationRule, scaleRule);
        if (weld)
        {
            simulatePhysics(GOToAttach, Vector3.zero, false);
        }
    }

    CollectibleTreasure getClosestHitObject(Collider[] hits)
    {
        float closestDistance = 10000.0f;
        CollectibleTreasure closestObjectSoFar = null;
        foreach (Collider hit in hits)
        {
            CollectibleTreasure c = hit.gameObject.GetComponent<CollectibleTreasure>();
            if (c)
            {
                float distanceBetweenHandAndObject = (c.gameObject.transform.position - leftPointerObject.gameObject.transform.position).magnitude;
                if (distanceBetweenHandAndObject < closestDistance)
                {
                    closestDistance = distanceBetweenHandAndObject;
                    closestObjectSoFar = c;
                }
            }
        }
        return closestObjectSoFar;
    }

    /*
        OLD CODE
    */



}
