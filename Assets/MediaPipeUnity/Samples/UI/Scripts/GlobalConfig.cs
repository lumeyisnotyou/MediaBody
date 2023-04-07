// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine.UI;

namespace Mediapipe.Unity.UI
{
  public class GlobalConfig : ModalContents
  {
    private const string _GlogLogtostederrPath = "Scroll View/Viewport/Contents/GlogLogtostderr/Toggle";
    private const string _GlogStderrthresholdPath = "Scroll View/Viewport/Contents/GlogStderrthreshold/Dropdown";
    private const string _GlogMinloglevelPath = "Scroll View/Viewport/Contents/GlogMinloglevel/Dropdown";
    private const string _GlogVPath = "Scroll View/Viewport/Contents/GlogV/Dropdown";
    private const string _GlogLogDirPath = "Scroll View/Viewport/Contents/GlogLogDir/InputField";
    private const string _GlogAddressPath = "Scroll View/Viewport/Contents/GLogAddress/InputField";
    private const string _GlogPortPath = "Scroll View/Viewport/Contents/GLogPort/InputField";

    private Toggle _glogLogtostderrInput;
    private Dropdown _glogStderrthresholdInput;
    private Dropdown _glogMinloglevelInput;
    private Dropdown _glogVInput;
    private InputField _glogLogDirInput;

    private InputField _glogAddressInput;
    private InputField _glogPortInput;
    private NetworkManager _networkManager;

    private void Start()
    {
      InitializeGlogLogtostderr();
      InitializeGlogStderrthreshold();
      InitializeGlogMinloglevel();
      InitializeGlogV();
      InitializeGlogLogDir();
      InitializeGlogAddress();
      InitializeGlogPort();

      _networkManager = FindObjectOfType<NetworkManager>();
      var config = NetworkConfigLoader.LoadConfig();
      _glogAddressInput.text = config.address;
      _glogPortInput.text = config.port.ToString();
    }

    public void SaveAndExit()
    {
      GlobalConfigManager.GlogLogtostderr = _glogLogtostderrInput.isOn;
      GlobalConfigManager.GlogStderrthreshold = _glogStderrthresholdInput.value;
      GlobalConfigManager.GlogMinloglevel = _glogMinloglevelInput.value;
      GlobalConfigManager.GlogLogDir = _glogLogDirInput.text;
      GlobalConfigManager.GlogV = _glogVInput.value;

      if (_networkManager != null)
      {
        _networkManager.UpdateOSCClient(_glogAddressInput.text, _glogPortInput.text);
      }

      GlobalConfigManager.Commit();
      Exit();
    }

    private void InitializeGlogLogtostderr()
    {
      _glogLogtostderrInput = gameObject.transform.Find(_GlogLogtostederrPath).gameObject.GetComponent<Toggle>();
      _glogLogtostderrInput.isOn = GlobalConfigManager.GlogLogtostderr;
    }

    private void InitializeGlogStderrthreshold()
    {
      _glogStderrthresholdInput = gameObject.transform.Find(_GlogStderrthresholdPath).gameObject.GetComponent<Dropdown>();
      _glogStderrthresholdInput.value = GlobalConfigManager.GlogStderrthreshold;
    }

    private void InitializeGlogMinloglevel()
    {
      _glogMinloglevelInput = gameObject.transform.Find(_GlogMinloglevelPath).gameObject.GetComponent<Dropdown>();
      _glogMinloglevelInput.value = GlobalConfigManager.GlogMinloglevel;
    }

    private void InitializeGlogV()
    {
      _glogVInput = gameObject.transform.Find(_GlogVPath).gameObject.GetComponent<Dropdown>();
      _glogVInput.value = GlobalConfigManager.GlogV;
    }

    private void InitializeGlogLogDir()
    {
      _glogLogDirInput = gameObject.transform.Find(_GlogLogDirPath).gameObject.GetComponent<InputField>();
      _glogLogDirInput.text = GlobalConfigManager.GlogLogDir;
    }

    private void InitializeGlogAddress()
    {
      _glogAddressInput = gameObject.transform.Find(_GlogAddressPath).gameObject.GetComponent<InputField>();
    }

    private void InitializeGlogPort()
    {
      _glogPortInput = gameObject.transform.Find(_GlogPortPath).gameObject.GetComponent<InputField>();
    }
  }
}
