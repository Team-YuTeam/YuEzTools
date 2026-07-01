using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace YuEzTools.UI;

public class FloatingButton : MonoBehaviour
{
    private Canvas canvas;
    private Image buttonImage;
    private RectTransform buttonRect;
    private Button button;

    private Coroutine hideCoroutine;
    private Coroutine slideCoroutine;

    private bool isHidden = false;
    private bool isHovering = false;
    private bool isDragging = false;
    private bool wasHiddenBeforeDrag = false;
    private float dragStartY;
    private float dragStartMouseY;
    private const float DRAG_THRESHOLD = 5f;
    private const float HIDE_DELAY = 0.5f;

    private const float BUTTON_SIZE = 60f;
    private const float SHOWN_X = -35f;   // 显示时：按钮中心离右边缘35px，即按钮突出5px
    private const float HIDDEN_X = 2f;    // 隐藏时：露出28px（超过2/5 = 24px）
    private const float ANIM_DURATION = 0.3f;

    private void Start()
    {
        CreateCanvas();
        CreateButton();
        ResetHideTimer();
    }

    private void CreateCanvas()
    {
        var canvasObj = new GameObject("YuET_FloatingCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void CreateButton()
    {
        var btnObj = new GameObject("FloatingBtn");
        btnObj.transform.SetParent(canvas.transform);

        buttonRect = btnObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(BUTTON_SIZE, BUTTON_SIZE);
        buttonRect.anchorMin = new Vector2(1f, 0.5f);
        buttonRect.anchorMax = new Vector2(1f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(SHOWN_X, 0f);

        buttonImage = btnObj.AddComponent<Image>();
        buttonImage.sprite = CreateCircleSprite(128, Main.ModColor32);
        buttonImage.color = new Color(1f, 1f, 1f, 0.45f);

        button = btnObj.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
    }

    private static Sprite CreateCircleSprite(int size, Color color)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2(size / 2f, size / 2f);
        var radius = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius)
                {
                    var alpha = 1f - (dist / radius) * 0.3f;
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private void Update()
    {
        if (buttonRect == null) return;

        var mousePos = Input.mousePosition;
        var mouseOver = RectTransformUtility.RectangleContainsScreenPoint(
            buttonRect, mousePos, canvas.worldCamera);

        // 拖动中
        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                var newY = dragStartY + (mousePos.y - dragStartMouseY);
                var halfH = Screen.height / 2f;
                newY = Mathf.Clamp(newY, -halfH + BUTTON_SIZE / 2f, halfH - BUTTON_SIZE / 2f);
                buttonRect.anchoredPosition = new Vector2(SHOWN_X, newY);
            }
            else
            {
                // 鼠标释放
                isDragging = false;
                var totalDrag = Mathf.Abs(buttonRect.anchoredPosition.y - dragStartY);

                if (totalDrag < DRAG_THRESHOLD)
                {
                    // 轻点：区分隐藏/显示状态
                    if (wasHiddenBeforeDrag)
                    {
                        // 隐藏时点击：只展开，不触发 MenuUI
                        SlideTo(SHOWN_X);
                        isHidden = false;
                    }
                    else
                    {
                        OnButtonClick();
                    }
                }
                ResetHideTimer();
            }
            return;
        }

        // 鼠标按下开始拖动
        if (mouseOver && Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            wasHiddenBeforeDrag = isHidden;
            isHidden = false;
            StopHideTimer();
            StopSlide();
            dragStartY = buttonRect.anchoredPosition.y;
            dragStartMouseY = mousePos.y;
            return;
        }

        // 悬浮检测
        if (mouseOver && !isHovering)
        {
            isHovering = true;
            StopHideTimer();

            if (isHidden)
            {
                SlideTo(SHOWN_X);
                isHidden = false;
            }
        }
        else if (!mouseOver && isHovering)
        {
            isHovering = false;
            ResetHideTimer();
        }
    }

    private void OnButtonClick()
    {
        if (Main.menuUI != null)
        {
            Main.menuUI.ToggleMenu(centerWindow: true);
        }
        ResetHideTimer();
    }

    private void StopHideTimer()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private void StopSlide()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
    }

    private void ResetHideTimer()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = this.StartCoroutine(AutoHideRoutine());
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSeconds(HIDE_DELAY);
        if (!isHidden)
        {
            SlideTo(HIDDEN_X);
            isHidden = true;
        }
    }

    private void SlideTo(float targetX)
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = this.StartCoroutine(SlideRoutine(targetX));
    }

    private IEnumerator SlideRoutine(float targetX)
    {
        var startX = buttonRect.anchoredPosition.x;
        var elapsed = 0f;

        while (elapsed < ANIM_DURATION)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / ANIM_DURATION;
            // ease in-out
            t = t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
            var x = Mathf.Lerp(startX, targetX, t);
            buttonRect.anchoredPosition = new Vector2(x, buttonRect.anchoredPosition.y);
            yield return null;
        }
        buttonRect.anchoredPosition = new Vector2(targetX, buttonRect.anchoredPosition.y);
    }
}