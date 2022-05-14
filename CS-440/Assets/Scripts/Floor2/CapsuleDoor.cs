using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleDoor : MonoBehaviour
{


	// Expose fence actions
	public void open () {
		StartCoroutine(MoveOverSpeed());
	}

    public IEnumerator MoveOverSpeed()
    {
        Vector3 end = transform.position;
        end.z = end.z - 0.08f;
        while (transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, 1f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        end = transform.position;
        end.x = end.x + 1.326f;
        while (transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, 1f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

}
