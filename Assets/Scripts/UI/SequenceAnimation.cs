using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceAnimation : MonoBehaviour
{
    public float waitBetween = 0.10f;
    public float waitForEnd = 0.50f;
    List<Animator> _animators;

    void Start()
    {
        _animators = new List<Animator>(GetComponentsInChildren<Animator>());

        StartCoroutine(DoAnim());
    }

    IEnumerator DoAnim()
    {
        while (true)
        {
            foreach ( var animator in _animators)
            {
                animator.SetTrigger("DoAnim");
                yield return new WaitForSeconds(waitBetween);
            }
            yield return new WaitForSeconds(waitForEnd);
        }
    }
}