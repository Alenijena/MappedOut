using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    const string IS_WALKING = "IsWalking";
    const string IS_FALLING = "IsFalling";

    const string ANIATION_RUN = "Run";
    const string ANIMATION_FALL = "Fall";
    const string ANIMATION_IDLE = "Idle";

    [SerializeField] private Transform mainCamera;
    [SerializeField] private Player player;
    [SerializeField] private GameObject footStepParticle;
    [SerializeField] private Transform particleTransform;
    [SerializeField] private float footstepDuration = 0.4f;
    [SerializeField] private Animator animator;

    private ParticleSystem currentFootStepParticle;
    private Vector3 particleScale;

    private void Awake() {
        particleScale = footStepParticle.GetComponent<Transform>().localScale;
    }

    private void UpdateLookDirection() {
        float rotationSpeed = 5f;
        transform.forward = Vector3.Slerp(transform.forward, new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z), rotationSpeed * Time.deltaTime);
    }

    private void HandleParticleVisual() {
        if (currentFootStepParticle != null) {
            Transform particleTransform = currentFootStepParticle.GetComponent<Transform>();
            particleTransform.localScale = particleScale * player.GetScaleRatio();
        }

        if (player.IsWalking() && player.UnaccurateIsgrounded()) {
            if (currentFootStepParticle != null) {return;}
            ParticleSystem particleSystem =  Instantiate(footStepParticle, particleTransform).GetComponent<ParticleSystem>();

            var emission = particleSystem.emission;
            emission.enabled = true;

            currentFootStepParticle = particleSystem;
        } else {
            if (currentFootStepParticle != null) {
                var emission = currentFootStepParticle.emission;
                emission.enabled = false;
                Destroy(currentFootStepParticle, footstepDuration);
                currentFootStepParticle = null;
                Debug.Log(currentFootStepParticle);
            }
        }
    }

    private void HandleAnimator() {

        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_FALLING, !player.UnaccurateIsgrounded());
    }

    private void Update() {
        UpdateLookDirection();
        HandleParticleVisual();
        HandleAnimator();
    }
}
