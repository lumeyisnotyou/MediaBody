using Mediapipe.Unity;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
  [SerializeField] private PointListAnnotation pointListAnnotation;
  [SerializeField] private float UserHeightInMeters = 1.7f;
  [SerializeField] private string OscTargetIp = "127.0.0.1";
  [SerializeField] private int OscTargetPort = 9000;

  // You must download and add OscCore to your project: https://github.com/vrchat/osccore/tree/all-in-one
  private OscCore.OscClient oscClient;

  /// This is the height of VRChat's default robot.
  /// You must adjust this to the standing height of the avatar
  /// in your scene used to produce a pose.
  private const float SENDER_AVATAR_HEIGHT = 1.87f;

  private float scaleFactor = 0.909091f; // (UserHeightInMeters / SENDER_AVATAR_HEIGHT)

  void Start()
  {
    scaleFactor = UserHeightInMeters / SENDER_AVATAR_HEIGHT;
    oscClient = new OscCore.OscClient(OscTargetIp, OscTargetPort);
  }

  // Priority => Hips, Left Foot, Right Foot, Left Elbow, Right Elbow, Left Knee, Right Knee, Chest
  // 13 Right Elbow
  // 14 Left Elbow
  // 25 Right Knee
  // 26 Left Knee
  // 28 Left Foot
  // 27 Right Foot
  // Average 12 and 13 for Chest
  // Average 23 and 24 for Hips
  void Update()
  {
    if (oscClient == null || pointListAnnotation.Landmarks == null)
      return;

    if (pointListAnnotation.Landmarks.Count < 32)
      return;

    if (IsCombinedPointValid((int)MediaPipeBodyPart.Hips_L, (int)MediaPipeBodyPart.Hips_R))
      oscClient.Send($"/tracking/trackers/1/position", GetAverageVector3((int)MediaPipeBodyPart.Hips_L, (int)MediaPipeBodyPart.Hips_R) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.LeftFoot))
      oscClient.Send($"/tracking/trackers/2/position", GetVector3((int)MediaPipeBodyPart.LeftFoot) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.RightFoot))
      oscClient.Send($"/tracking/trackers/3/position", GetVector3((int)MediaPipeBodyPart.RightFoot) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.LeftElbow))
      oscClient.Send($"/tracking/trackers/4/position", GetVector3((int)MediaPipeBodyPart.LeftElbow) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.RightElbow))
      oscClient.Send($"/tracking/trackers/5/position", GetVector3((int)MediaPipeBodyPart.RightElbow) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.LeftKnee))
      oscClient.Send($"/tracking/trackers/6/position", GetVector3((int)MediaPipeBodyPart.LeftKnee) * scaleFactor);

    if (IsPointValid((int)MediaPipeBodyPart.RightKnee))
      oscClient.Send($"/tracking/trackers/7/position", GetVector3((int)MediaPipeBodyPart.RightKnee) * scaleFactor);

    if (IsCombinedPointValid((int)MediaPipeBodyPart.Chest_L, (int)MediaPipeBodyPart.Chest_R))
      oscClient.Send($"/tracking/trackers/8/position", GetAverageVector3((int)MediaPipeBodyPart.Chest_L, (int)MediaPipeBodyPart.Chest_R) * scaleFactor);

    // oscClient.Send($"/tracking/trackers/1/rotation", pointListAnnotation.Landmarks[i].eulerAngles);
  }

  private bool IsCombinedPointValid(int one, int two, float threshold = 0.8f)
  {
    return (pointListAnnotation.Landmarks[one].Visibility + pointListAnnotation.Landmarks[two].Visibility) / 2 >= threshold;
  }

  private bool IsPointValid(int one, float threshold = 0.8f)
  {
    return pointListAnnotation.Landmarks[one].Visibility >= threshold;
  }

  private Vector3 GetAverageVector3(int one, int two)
  {
    return (GetVector3(one) + GetVector3(two)) / 2;
  }

  private Vector3 GetVector3(int index)
  {
    return new Vector3(
      pointListAnnotation.Landmarks[index].X,
      pointListAnnotation.Landmarks[index].Y,
      pointListAnnotation.Landmarks[index].Z
    );
  }

  // TODO
  private Quaternion GetRotation(int index)
  {
    return Quaternion.identity;
  }

  public enum MediaPipeBodyPart
  {
    Chest_L = 12,
    Chest_R = 13,
    RightElbow = 13,
    LeftElbow = 14,
    Hips_L = 23,
    Hips_R = 24,
    RightKnee = 25,
    LeftKnee = 26,
    RightFoot = 27,
    LeftFoot = 28
  }
}
