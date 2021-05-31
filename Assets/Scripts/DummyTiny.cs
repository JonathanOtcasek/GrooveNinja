using UnityEngine;

public class DummyTiny : MonoBehaviour
{
    public TrailRenderer myTrail;

    public void Off()
    {
        myTrail.emitting = false;
    }
}
