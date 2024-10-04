using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;


    public LineRenderer leftRubberBand;
    public LineRenderer rightRubberBand;
    public Transform leftAnchor;         
    public Transform rightAnchor;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private AudioSource audioSource;

     void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        leftRubberBand.positionCount = 2;
        rightRubberBand.positionCount = 2;

        audioSource = GetComponent<AudioSource>();
    }
    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
        //print("Slingshot:OnMouseEnter()"); 
    }

     void OnMouseExit()
    {
        launchPoint.SetActive(false);
        //print("Slingshot:OnMouseExit()");
    }

     void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        
        UpdateRubberBands(launchPos);

        audioSource.Play();
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;                   //gets mouse pos in 2d coordinates
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        UpdateRubberBands(projPos);

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            //audioSource.Play();

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile; // sets maincam poi
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            ResetRubberBands();
        }

    }

    void UpdateRubberBands(Vector3 projPos)
        {
            // Update the left rubber band to stretch from left anchor to the projectile
            leftRubberBand.SetPosition(0, leftAnchor.position);  // Start point: left arm
            leftRubberBand.SetPosition(1, projPos);              // End point: projectile

            // Update the right rubber band to stretch from right anchor to the projectile
            rightRubberBand.SetPosition(0, rightAnchor.position); // Start point: right arm
            rightRubberBand.SetPosition(1, projPos);              // End point: projectile
        }

        // Reset the rubber bands to their initial positions when the projectile is launched
        void ResetRubberBands()
        {
            // Set the rubber bands to only connect the slingshot arms (no projectile)
            leftRubberBand.SetPosition(1, leftAnchor.position);
            rightRubberBand.SetPosition(1, rightAnchor.position);
        }
}
