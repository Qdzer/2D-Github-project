using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public Vector3 positionToGo;
    public GameSceneSO sceneToGo;
    public SceneLoadEventSO LoadEventSO;
    public void TrggerAction()
    {
        Debug.Log("666");
        LoadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
