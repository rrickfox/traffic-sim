using UnityEngine;

public class UpdatePublisher : MonoBehaviour
{
    public static event System.Action PreFixedUpdate;
    public static event System.Action NormalFixedUpdate;
    public static event System.Action PostFixedUpdate;
    void FixedUpdate()
    {
        PreFixedUpdate?.Invoke();
        NormalFixedUpdate?.Invoke();
        PostFixedUpdate?.Invoke();
    }

    public static void SubscribePreFixedUpdate(System.Action subscriber)
    {
        PreFixedUpdate += subscriber;
    }

    public static void SubscribeNormalFixedUpdate(System.Action subscriber)
    {
        NormalFixedUpdate += subscriber;
    }

    public static void SubscribePostFixedUpdate(System.Action subscriber)
    {
        PostFixedUpdate += subscriber;
    }

    public static void UnsubscribePreFixedUpdate(System.Action subscriber)
    {
        PreFixedUpdate -= subscriber;
    }

    public static void UnsubscribeNormalFixedUpdate(System.Action subscriber)
    {
        NormalFixedUpdate -= subscriber;
    }

    public static void UnsubscribePostFixedUpdate(System.Action subscriber)
    {
        PostFixedUpdate -= subscriber;
    }
}
