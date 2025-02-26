using Mujoco;
using UnityEngine;

public class DisableGravity : MonoBehaviour {
    private MjScene _scene;

    void Start() {
        _scene = FindObjectOfType<MjScene>();
        if (_scene == null) {
            Debug.LogError("MjScene not found in the scene!");
            return;
        }
    }
}