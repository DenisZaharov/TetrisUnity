using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureRotatorY : MonoBehaviour {

    const float actionInterval = 0.02f; //через какой промежуток времени происходит очередной шаг вращения

    Vector3 center;
    float lastRotateTime = 0;

    public void SetCenter(Vector3 center)
    {
        this.center = center;
    }

    void Update()
    {
        if (Time.realtimeSinceStartup - lastRotateTime < actionInterval)
            return;

        FigurePolygon polygon = gameObject.GetComponent<FigurePolygon>();
        if (polygon )
        {
            gameObject.transform.RotateAround(center, Vector3.up, 2.0f);
        }

        lastRotateTime = Time.realtimeSinceStartup;
    }
}
