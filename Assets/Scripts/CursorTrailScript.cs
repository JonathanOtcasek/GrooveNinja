//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CursorTrailScript : MonoBehaviour
{
    public static CursorTrailScript me;

    public GameObject fingerPosition;
    public TrailRenderer fingerTrail;

    Vector3 newPosition;
    //private Vector3 screenPoint;
    private Vector3 offset;
    //private Vector3 screenPosition;
    bool isDragging = false;
    public bool demoPhase = true;

    void Start()
    {
        if(me == null)
        {
            me = this;
        } else
        {
            Destroy(me);
        }
        newPosition = fingerPosition.transform.position;
    }

    void OnMouseUp()
    {
        print(true);
        isDragging = false;
        fingerTrail.emitting = false;
    }

    void Update()
    {
        if (!demoPhase)
        {
            if (isDragging)
            {
                Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 4f);
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
                fingerPosition.transform.position = currentPosition;

                fingerTrail.emitting = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                fingerTrail.emitting = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                fingerTrail.Clear();

                newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
                fingerPosition.transform.position = newPosition;
                isDragging = true;
                //screenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                fingerTrail.emitting = false;
            }
        }
    }
}

