using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// ワールドへの入退場を確認するための最小ライフサイクル管理です。
/// プレイヤー名、UI、音声などのソーシャル機能は扱いません。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PlayerLifecycleManager : UdonSharpBehaviour
{
    private VRCPlayerApi localPlayer;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;

        if (localPlayer == null)
        {
            Debug.Log("[PlayerLifecycle] ClientSimまたはUnity Editorで起動しています。参加人数: " + VRCPlayerApi.GetPlayerCount());
            return;
        }

        Debug.Log("[PlayerLifecycle] ローカルプレイヤーを取得しました。参加人数: " + VRCPlayerApi.GetPlayerCount());
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Debug.Log("[PlayerLifecycle] プレイヤー参加を検知しました。参加人数: " + VRCPlayerApi.GetPlayerCount());
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        Debug.Log("[PlayerLifecycle] プレイヤー退出を検知しました。参加人数: " + VRCPlayerApi.GetPlayerCount());
    }
}
