using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotDemo;
using DG.Tweening;
using UTJ;

public class LookAtMouse : MonoBehaviour
{
    int i = 0;
    float timer;
    // Use this for initialization
    public Transform gunT;
    public Transform[] shootingT;

    private Tween rotationTween;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f))
        {
            Debug.Log(hit.collider.gameObject.name);
            Vector3 direction = hit.point - gunT.transform.position;
            Quaternion localRotation = Quaternion.LookRotation(direction);
            Quaternion targetQuarternion = Quaternion.Euler(-localRotation.eulerAngles.y, localRotation.eulerAngles.x, 0);
            //gunT.localRotation = Quaternion.Lerp(gunT.rotation, targetQuarternion, Time.deltaTime);
            gunT.localRotation = targetQuarternion;
            if (Input.GetMouseButton(0))
            {
                timer += Time.deltaTime;
                if (timer > .1f)
                {
                    i++;
                    Vector3 pos = shootingT[i % shootingT.Length].position;
                    if (i % 2 == 0)
                    {
                        Player.Instance.Fire_left(ref pos, ref localRotation);
                    }
                    else
                    {
                        Player.Instance.Fire_right(ref pos, ref localRotation);
                    }
                    //Beamer.Instance.Shoot(shootingT[i % shootingT.Length].position, 30 * direction.normalized, 10 /*damage*/);
                    timer = 0f;
                }
            }
        }
    }
}
