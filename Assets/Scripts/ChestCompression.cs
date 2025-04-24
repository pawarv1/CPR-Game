using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestCompression : MonoBehaviour
{
    public Transform controller; // drag the hand/controller into this
    public float compressionThreshold = 0.05f; // minimum distance to count as compression
    public float resetThreshold = 0.02f; // distance to reset after a compression

    private float lastY;
    private bool readyForNextCompression = true;
    private int compressionCount = 0;
    public Collider chestCollider;



    void Start() {
        lastY = controller.position.y;
    }

    void Update() {
        float currentY = controller.position.y;
        float deltaY = lastY - currentY;

        if (deltaY > compressionThreshold && readyForNextCompression) {
            RegisterCompression();
            readyForNextCompression = false;
        }

        // Reset if hand goes back up
        if (controller.position.y - lastY > resetThreshold) {
            readyForNextCompression = true;
        }

        lastY = currentY;
    }

    void RegisterCompression() {
        compressionCount++;
        Debug.LogWarning("Compression #" + compressionCount);
        // Optional: play sound, animate chest, give feedback, etc.
    }

    bool IsOverChest() => chestCollider.bounds.Contains(controller.position);


}
