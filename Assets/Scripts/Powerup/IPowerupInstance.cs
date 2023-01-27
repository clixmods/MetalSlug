using UnityEngine;

public interface IPowerupInstance
{
    void OnEnable();
    void OnTriggerEnter(Collider other);
    void OnSpawn();
    void OnGrab(PlayerInstance playerInstance);
}