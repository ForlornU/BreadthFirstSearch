using UnityEngine;

public static class TransformExtensions 
{
    public static void ResetTransformation(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
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
