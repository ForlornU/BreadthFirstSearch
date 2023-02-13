using UnityEngine;

public static class TransformExtensions 
{

    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static Vector3 DirectionTo(this Transform source, Transform destination)
    {
        return source.position.DirectionTo(destination.position);
    }

    public static Vector3 DirectionTo(this Transform source, Vector3 destination)
    {
        return source.position.DirectionTo(destination);
    }


}
