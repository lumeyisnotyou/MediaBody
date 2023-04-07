using System.IO;
using UnityEngine;

public static class NetworkConfigLoader
{

  private const string DefaultOscTargetIp = "127.0.0.1";
  private const int DefaultOscTargetPort = 9000;

  public static Config LoadConfig()
  {
    // Load the game settings from a file, or create a new instance if the file doesn't exist
    var path = Path.Combine(Application.persistentDataPath, "GameSettings.json");
    if (File.Exists(path))
    {
      var json = File.ReadAllText(path);
      return JsonUtility.FromJson<Config>(json);
    }
    else
    {
      SaveConfig(DefaultOscTargetIp, DefaultOscTargetPort);
      return new Config()
      {
        address = DefaultOscTargetIp,
        port = DefaultOscTargetPort
      };
    }
  }

  public static void SaveConfig(string address, int port)
  {
    var config = new Config()
    {
      address = address,
      port = port
    };

    var json = JsonUtility.ToJson(config);
    var path = Path.Combine(Application.persistentDataPath, "GameSettings.json");
    File.WriteAllText(path, json);
  }
}

