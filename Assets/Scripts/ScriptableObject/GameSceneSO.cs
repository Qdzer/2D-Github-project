using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scene/GaneSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType SceneType;
    public AssetReference sceneReference;
}