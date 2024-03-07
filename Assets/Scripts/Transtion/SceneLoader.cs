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

    [Header("事件监听")]
    public SceneLoadEventSO LoadEventSO;
    public GameSceneSO firstLoadScene;


    [Header("广播")]
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
    //TODO:做完MainMenu之后更改
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
    /// 场景加载事件请求
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
            //TODO:变黑
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        if (currenLoadedScene != null)
        {
            yield return currenLoadedScene.sceneReference.UnLoadScene();
        }
        //关闭人物
        playTrans.gameObject.SetActive(false);
        //加载新场景
        LoadNewScene();
    }
    /// <summary>
    /// 场景加载完成后
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currenLoadedScene = sceneToLoad;

        playTrans.position = positionToGo;
        //启动人物
        playTrans.gameObject.SetActive(true);
        if(fadeScreen)
        {
            //TODO:变透明
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        //场景加载完后事件
        afterScerenLoadedEvent?.RaiseEvent();
    }
}
