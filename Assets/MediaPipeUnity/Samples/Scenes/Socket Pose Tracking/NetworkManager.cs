using Mediapipe.Unity;
using System.Net;
using UnityEngine;
using static NetworkUtils;

public class NetworkManager : MonoBehaviour
{
  private const float UserHeightInMeters = 1.7f;
  private string OscTargetIp;
  private int OscTargetPort;

  private OscCore.OscClient oscClient;

  /// This is the height of VRChat's default robot.
  /// You must adjust this to the standing height of the avatar
  /// in your scene used to produce a pose.
  private const float SENDER_AVATAR_HEIGHT = 1.87f;

  private float scaleFactor = 0.909091f; // (UserHeightInMeters / SENDER_AVATAR_HEIGHT)

  private void Start()
  {
    UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
    scaleFactor = UserHeightInMeters / SENDER_AVATAR_HEIGHT;
    var config = NetworkConfigLoader.LoadConfig();
    oscClient = new OscCore.OscClient(config.address, config.port);
    OscTargetIp = config.address;
    OscTargetPort = config.port;
  }

  public void UpdateOSCClient(string addressCanidate, string portCanidate)
  {
    var needsUpdate = false;

    if (IPAddress.TryParse(portCanidate, out var _))
    {
      OscTargetIp = addressCanidate;
      needsUpdate = true;
    }

    if (int.TryParse(portCanidate, out var parsedPort))
    {
      OscTargetPort = parsedPort;
      needsUpdate = true;
    }

    if (needsUpdate)
    {
      oscClient = null;
      oscClient = new OscCore.OscClient(OscTargetIp, OscTargetPort);
      NetworkConfigLoader.SaveConfig(OscTargetIp, OscTargetPort);
    }
  }

  /* Priority => Hips, Left Foot, Right Foot, Left Elbow, Right Elbow, Left Knee, Right Knee, Chest
  * 13 Right Elbow
  * 14 Left Elbow
  * 25 Right Knee
  * 26 Left Knee
  * 28 Left Foot
  * 27 Right Foot
  * Average 12 and 13 for Chest
  * Average 23 and 24 for Hips */
  private void Update()
  {
    if (oscClient == null || PointListAnnotation.Landmarks == null)
      return;

    if (PointListAnnotation.Landmarks.Count < 32)
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

  private void OnApplicationQuit()
  {
    oscClient = null;
    NetworkConfigLoader.SaveConfig(OscTargetIp, OscTargetPort);
  }
}
