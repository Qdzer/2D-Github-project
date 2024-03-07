using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Transform playTrans;
    public Vector3 firstPosition;

    [Header("�¼�����")]
    public SceneLoadEventSO LoadEventSO;
    public GameSceneSO firstLoadScene;


    [Header("�㲥")]
    public VoidEventSO afterScerenLoadedEvent;
    public FadeEventSO fadeEvent;

    private GameSceneSO currenLoadedScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public float fadeDuration;

    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        //currenLoadedScene = firstLoadScene;
        //currenLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
    }
    //TODO:����MainMenu֮�����
    private void Start()
    {
        NewGame();
    }

    private void OnEnable()
    {
        LoadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }
    private void OnDisable()
    {
        LoadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        OnLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// ���������¼�����
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;

        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        if (currenLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }


    private void LoadNewScene()
    {
       var loadingOption= sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //TODO:���
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        if (currenLoadedScene != null)
        {
            yield return currenLoadedScene.sceneReference.UnLoadScene();
        }
        //�ر�����
        playTrans.gameObject.SetActive(false);
        //�����³���
        LoadNewScene();
    }
    /// <summary>
    /// ����������ɺ�
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currenLoadedScene = sceneToLoad;

        playTrans.position = positionToGo;
        //��������
        playTrans.gameObject.SetActive(true);
        if(fadeScreen)
        {
            //TODO:��͸��
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        //������������¼�
        afterScerenLoadedEvent?.RaiseEvent();
    }
}
