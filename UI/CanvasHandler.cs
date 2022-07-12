using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  class containing methods for working with UI elements
/// </summary>
public class CanvasHandler : MonoBehaviour
{
    [SerializeField]
    float time;

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    enum renderModes
    {
        overlay,
        camera,
        world
    }
    // smoothly changes alpha value of UI to 0
    public void fadeUIAway()
    {
        checkCanvas();
        checkCanvasGroup();
        StartCoroutine(fadeAway(canvasGroup, time));
    }
    IEnumerator fadeAway(CanvasGroup canvasGroup, float time, float timeStep = 0.02f)
    {
        float alphaStep = timeStep / time;
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= alphaStep;
            yield return new WaitForSeconds(timeStep);
        }
    }
    // smoothly changes alpha value of UI to 1
    public void fadeUIin()
    {
        checkCanvas();
        checkCanvasGroup();
        StartCoroutine(fadeIn(canvasGroup, time));
    }
    IEnumerator fadeIn(CanvasGroup canvasGroup, float time, float timeStep = 0.02f)
    {
        float alphaStep = timeStep / time;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += alphaStep;
            yield return new WaitForSeconds(timeStep);
        }
    }

    void checkCanvas()
    {
        if(canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    void checkCanvasGroup()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void changeRenderModeToOverlay()
    {
        checkCanvas();
        changeScreenSpace(renderModes.overlay);
    }

    public void changeRenderModeToWorld()
    {
        checkCanvas();
        changeScreenSpace(renderModes.world);
    }

    public void changeRenderModeToCamera()
    {
        checkCanvas();
        changeScreenSpace(renderModes.camera);
    }

    void changeScreenSpace(renderModes renderMode)
    {
        switch (renderMode)
        {
            case renderModes.overlay:
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case renderModes.camera:
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                break;
            case renderModes.world:
                canvas.renderMode = RenderMode.WorldSpace;
                break;
        }
    }
}
