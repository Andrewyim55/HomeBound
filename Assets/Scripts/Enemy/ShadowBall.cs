using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBall : Bullet
{
    [Header("Shadowball references")]
    [SerializeField] private Animator anim;

    // Update is called once per frame
    protected override void Update()
    {
        StartCoroutine(shadowBallFire());
    }

    IEnumerator shadowBallFire()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("idle");
        yield return new WaitForSeconds(timeTillDespawn);
        anim.SetTrigger("death");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
