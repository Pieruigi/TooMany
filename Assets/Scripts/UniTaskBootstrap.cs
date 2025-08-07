using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;

public class UniTaskBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        var loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop);
        Debug.Log("✅ UniTask PlayerLoop initialized");
    }
}
