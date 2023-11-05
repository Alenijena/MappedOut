using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraVisual : MonoBehaviour
{      
    [SerializeField] private CinemachineFreeLook cam;
    [SerializeField] private float bobHeight = 10f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private bool viewBobbing = true; 
    [SerializeField] private Rigidbody playerRigidBody;

    private void HandleViewBobbing () {
        if (viewBobbing) {
            
        }
    }

    public void EnableViewBobbing() {
        viewBobbing = true;
    }

    public void DisableViewBobbing() {
        viewBobbing = false;
    }

    void Update() {
        HandleViewBobbing();
    }
}
