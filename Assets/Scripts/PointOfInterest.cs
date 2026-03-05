using System;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    public event Action OnClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnMouseDown()
    {
        OnClicked?.Invoke();
    }
}
