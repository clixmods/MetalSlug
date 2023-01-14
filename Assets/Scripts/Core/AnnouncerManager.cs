using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerInstance.eventPlayerDown += PlayerInstanceOneventPlayerDown;
    }

    private void PlayerInstanceOneventPlayerDown(PlayerInstance newplayer)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
