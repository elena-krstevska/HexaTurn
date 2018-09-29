using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UIController : MonoBehaviour {

    public GameObject title;
    public GameObject revertButton;
    public static event Action RevertLastMove = () => { };

    void Start()
    {
        AddListeners();
        PlayCoverAnimation();
    }

    void PlayCoverAnimation()
    {
        if(title)
            LeanTween.scale(title, title.transform.localScale * 1.3f, 0.5f).setLoopPingPong(100);
    }

    public void PlayBtnClicked(GameObject btn)
    {
        if (LeanTween.isTweening(btn))
            return;

        LeanTween.scale(btn, Vector3.one * 0.9f, 0.1f).setLoopPingPong(1).setOnComplete(()=> {
            SceneManager.LoadScene(1);
        });
    }

    public void RevertBtnClicked(GameObject btn)
    {
        if (LeanTween.isTweening(btn))
            return;

        LeanTween.scale(btn, Vector3.one * 0.9f, 0.1f).setLoopPingPong(1).setOnComplete(() => {
            RevertLastMove();
        });
    }

    public void BackBtnClicked(GameObject btn)
    {
        if (LeanTween.isTweening(btn))
            return;

        LeanTween.scale(btn, Vector3.one * 0.9f, 0.1f).setLoopPingPong(1).setOnComplete(() => {
            SceneManager.LoadScene(0);
        });
    }

    public void RestartBtnClicked(GameObject btn)
    {
        if (LeanTween.isTweening(btn))
            return;

        LeanTween.scale(btn, Vector3.one * 0.9f, 0.1f).setLoopPingPong(1).setOnComplete(() => {
            SceneManager.LoadScene(1);
        });
    }

    void ShakeRevertButton()
    {
        LeanTween.scale (revertButton, Vector3.one * 1.3f, 0.2f).setLoopPingPong (2);
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void AddListeners()
    {
        Grid.ShakeRevertButton += ShakeRevertButton;
    }

    void RemoveListeners()
    {
        Grid.ShakeRevertButton -= ShakeRevertButton;
    }
}
