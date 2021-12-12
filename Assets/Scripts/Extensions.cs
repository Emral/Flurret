using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void KeepOnScreen(this Collider2D collider, Camera cam)
    {
        Vector3 newPosition = collider.transform.position;
        Rect camRect = cam.GetCameraBounds();
        Vector2 size = collider.bounds.extents * 0.5f;
        newPosition.x = Mathf.Clamp(newPosition.x, camRect.xMin + size.x, camRect.xMax - size.x);
        newPosition.y = Mathf.Clamp(newPosition.y, camRect.yMin + size.y, camRect.yMax - size.y);
        collider.transform.position = newPosition;
    }

    public static Rect GetCameraBounds(this Camera cam)
    {
        Vector2 size = cam.GetCameraSize();
        return new Rect(
            new Vector2(cam.transform.position.x - 0.5f * size.x, cam.transform.position.y - 0.5f * size.y),
            size
        );
    }

    public static Vector2 GetCameraSize(this Camera cam)
    {
        float aspect = Screen.width / ((float)Screen.height);
        return new Vector2(cam.orthographicSize * aspect * 2f, cam.orthographicSize * 2f);
    }

    public static Vector2 Rotate(this Vector2 vec, float angle)
    {
        return (Vector2)(Quaternion.Euler(0, 0, angle) * vec);
    }

    public static IEnumerator Flicker(this Behaviour componentToFlicker, float flickerInterval = 0.05f, int flickerCount = 3)
    {
        for (int i = 0; i < flickerCount; i++)
        {
            componentToFlicker.enabled = false;
            yield return new WaitForSeconds(flickerInterval);
            componentToFlicker.enabled = true;
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    public static IEnumerator DespawnFlicker(this GameObject objectToKill, Behaviour componentToFlicker, float flickerInterval = 0.05f, int flickerCount = 3)
    {
        yield return componentToFlicker.Flicker(flickerInterval, flickerCount);
        Object.Destroy(objectToKill);
    }

    public static IEnumerator Flicker(this Renderer componentToFlicker, float flickerInterval = 0.05f, int flickerCount = 3)
    {
        for (int i = 0; i < flickerCount; i++)
        {
            componentToFlicker.enabled = false;
            yield return new WaitForSeconds(flickerInterval);
            componentToFlicker.enabled = true;
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    public static IEnumerator DespawnFlicker(this GameObject objectToKill, Renderer componentToFlicker, float flickerInterval = 0.05f, int flickerCount = 3)
    {
        yield return componentToFlicker.Flicker(flickerInterval, flickerCount);
        Object.Destroy(objectToKill);
    }

    public static T GetRandom<T>(this IList<T> list, int min = 0, int max = -1)
    {
        if (max == -1)
        {
            max = list.Count - 1;
        }
        return list[Random.Range(min, max + 1)];
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return ((mask & 1 << layer) > 0);
    }

    public static IEnumerator Fade(this UnityEngine.UI.Image img, Color startColor, Color endColor, float duration = 0.5f)
    {
        float timePassed = 0;

        while (timePassed < 1)
        {
            timePassed = Mathf.Clamp01(timePassed + Time.unscaledDeltaTime / duration);
            img.color = Color.Lerp(startColor, endColor, timePassed);
            yield return null;
        }
        img.color = endColor;
    }
}