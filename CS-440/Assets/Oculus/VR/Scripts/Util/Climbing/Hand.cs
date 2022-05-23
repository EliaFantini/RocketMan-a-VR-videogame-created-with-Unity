using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
  public OVRPlayerController climber = null;
  public OVRInput.Controller controller = OVRInput.Controller.None;

  public Vector3 Delta {private set; get; } = Vector3.zero;
  private Vector3 lastPosition = Vector3.zero;
  private GameObject currentPoint = null;
  private List<GameObject> contactPoints = new List<GameObject>(); //all points currently touching
  
  private MeshRenderer meshRenderer = null;

  public void Awake() {
      meshRenderer = GetComponentInChildren<MeshRenderer>();
  }

  private void Start() {
      lastPosition = transform.position;
  }
  public void Update() {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            GrabPoint();

        if(OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
            ReleasePoint();

  }
  //called every fixed frame-rate frame
  private void FixedUpdate() {
      lastPosition = transform.position;
  }
  private void LateUpdate() {
      Delta = lastPosition-transform.position;
  }

  public void GrabPoint() {
      currentPoint = Utility.GetNearest(transform.position, contactPoints);
      if(currentPoint) {
          climber.SetHand(this); //tells climber which hand we want to be pulling up with
          //meshRenderer.enabled = false;
      }
  }

  public void ReleasePoint(){
      if(currentPoint) {
          climber.ClearHand();
          //meshRenderer.enabled = true;
      }
      currentPoint = null; //just to be safe
  }

  private void OnTriggerEnter(Collider other){
      AddPoint(other.gameObject);
  }

  private void AddPoint(GameObject newObject){
      if(newObject.CompareTag("ClimbPoint"))
        contactPoints.Add(newObject);
  }

  private void OnTriggerExit(Collider other) {
      RemovePoint(other.gameObject);
  }

  private void RemovePoint(GameObject newObject){
      if(newObject.CompareTag("ClimbPoint"))
        contactPoints.Remove(newObject);
  }

}

