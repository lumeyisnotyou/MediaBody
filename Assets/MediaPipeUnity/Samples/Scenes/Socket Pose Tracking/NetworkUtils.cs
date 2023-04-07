using Mediapipe.Unity;
using UnityEngine;

public static class NetworkUtils
{
  public static bool IsCombinedPointValid(int one, int two, float threshold = 0.8f)
  {
    return (PointListAnnotation.Landmarks[one].Visibility + PointListAnnotation.Landmarks[two].Visibility) / 2 >= threshold;
  }

  public static bool IsPointValid(int one, float threshold = 0.8f)
  {
    return PointListAnnotation.Landmarks[one].Visibility >= threshold;
  }

  public static Vector3 GetAverageVector3(int one, int two)
  {
    return (GetVector3(one) + GetVector3(two)) / 2;
  }

  public static Vector3 GetVector3(int index)
  {
    return new Vector3(
      PointListAnnotation.Landmarks[index].X,
      PointListAnnotation.Landmarks[index].Y,
      PointListAnnotation.Landmarks[index].Z
    );
  }

  // TODO
  public static Quaternion GetRotation(int index)
  {
    return Quaternion.identity;
  }
}
