using UnityEngine;
using Beat;

public class ProjectileScript : MonoBehaviour
{
    public bool last = false;
    public CursorGameScript boss;

    public Vector3 targetPos;
    Vector3 startingPos;

    Clock myclock;

    float counter;

    private void Start()
    {
        myclock = Clock.clok;
        counter = 0f + (float)(myclock.Time / myclock.TickLength);
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //counter += (float)Clock.Instance.TickLength;
        transform.position = Vector3.LerpUnclamped(startingPos, targetPos, ((float)(myclock.Time / myclock.TickLength) - counter) /384f);
        if(Vector3.Distance(transform.position, startingPos) > 100f)
        {
            EndMe();
        }
    }

    public void EndMe()
    {
        Destroy(gameObject);
        if (last)
        {
            boss.GameEnd();
        }
    }
}
