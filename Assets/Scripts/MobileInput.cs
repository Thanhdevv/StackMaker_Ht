using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance { get; private set; }
    public bool tap, swipeLeft, swipeRight, swipeUp, swipeDow;
    [SerializeField] private Vector2 swipeDelta, startTouch;

    private const float deadZone = 100;
    public void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        tap = swipeLeft = swipeRight = swipeDow = swipeUp = false;

        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0)){
            startTouch = swipeDelta = Vector2.zero;
        }
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            if(Input.touches.Length != 0){
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        if(swipeDelta.magnitude > deadZone)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if(x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            }
            else
            {
                if(y < 0)
                {
                    swipeDow = true;
                }
                else
                {
                    swipeUp = true;
                }
            }
            startTouch = swipeDelta = Vector2.zero;

        }

    }
}
