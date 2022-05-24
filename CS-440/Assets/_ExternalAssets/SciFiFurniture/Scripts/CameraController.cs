using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   public float Sensitivity = 0.9f;
	
	[Range(0,100)]
	public float Speed = 20f;
	[Range(0,100)]
	public float RotationSpeed = 7f;
	public bool LockMouse;

	private float Xaxis;
	private float Yaxis;
	private float LastX;
	private float LastY;
	private float AdvanceSpeed = 1f;
    private void LateUpdate()
    {
        if(Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            LockMouse = !LockMouse;

            if(!LockMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState  = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        Move();
    }

    private void Move()
    {
        if(LockMouse)
        {
            Xaxis = Input.GetAxis("Mouse X") + LastX * Sensitivity;
            Yaxis = -Input.GetAxis("Mouse Y") + LastY * Sensitivity;

            LastX = Xaxis;
            LastY = Yaxis;


            transform.Rotate(RotationSpeed * Yaxis * Time.deltaTime, RotationSpeed * Xaxis * Time.deltaTime,0);

            if(Input.GetKey(KeyCode.Space))
            {
                AdvanceSpeed = 5f;   
            }
        }
        else
        {
            AdvanceSpeed = 1;
        }

       transform.Translate(AdvanceSpeed * Speed * Input.GetAxis("Horizontal") * Time.deltaTime,0,AdvanceSpeed * Speed * Input.GetAxis("Vertical") * Time.deltaTime);
       
       Vector3 stayHorizontal = transform.eulerAngles;
       stayHorizontal.z = 0f;
       transform.eulerAngles = stayHorizontal; 
    }
}
