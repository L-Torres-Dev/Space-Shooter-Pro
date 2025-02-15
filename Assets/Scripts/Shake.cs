using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public IEnumerator Co_Shake(ShakeType type, float shakeDuration, float afterWait, float magnitude)
    {
        switch (type)
        {
            case ShakeType.SmallHori:
                yield return SmallShake(type, shakeDuration, afterWait, magnitude);
                break;
            case ShakeType.Large:
                yield return LargeShake(shakeDuration, afterWait, magnitude);
                break;
            default:
                yield return SmallShake(type, shakeDuration, afterWait, magnitude);
                break;
        }
    }

    private IEnumerator SmallShake(ShakeType type, float shakeDuration, float afterWait, float magnitude)
    {
        float elapsed = 0;
        float shakeTime = 0;

        float shakeNegative = -magnitude;
        float shakePositive = magnitude;

        var endOfFrame = new WaitForEndOfFrame();
        float timeBetweenShakes = .03f;
        Vector3 position = Vector3.zero;
        while (elapsed < shakeDuration)
        {
            yield return endOfFrame;

            position = transform.position;

            if (type == ShakeType.SmallHori)
                position.x = position.x + shakePositive;
            else
                position.y = position.y + shakePositive;

            transform.position = position;

            shakeTime = 0;
            while (shakeTime < timeBetweenShakes)
            {
                shakeTime += Time.unscaledDeltaTime;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return endOfFrame;


            position = transform.position;

            if (type == ShakeType.SmallHori)
                position.x = position.x + shakeNegative;
            else
                position.y = position.y + shakeNegative;

            transform.position = position;

            shakeTime = 0;
            while (shakeTime < timeBetweenShakes)
            {
                shakeTime += Time.unscaledDeltaTime;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            elapsed += Time.unscaledDeltaTime;
        }

        yield return new WaitForSecondsRealtime(afterWait);
    }
    private IEnumerator LargeShake(float shakeDuration, float afterWait, float magnitude)
    {
        //r = ac^(θcot(b))
        //0 < a < 1 
        //pi/2 < b < pi
        //c determines how many revolutions the spiral has from a to 0

        float a = magnitude;
        float b = Mathf.Deg2Rad * 91;
        float c = 1.21f;

        float randomStartAngle = Random.Range(0, 360);
        float currentAngle = 0;
        float maxAngle = 2160;

        float timeBetweenShakes = .03f;
        float elapsed = 0;
        float progress = 0;
        float radius = 0;
        Vector3 startPos = transform.position;
        while (elapsed < shakeDuration)
        {
            progress = Mathf.InverseLerp(0, shakeDuration, elapsed);

            currentAngle = Mathf.Lerp(0, maxAngle, progress) + randomStartAngle;

            radius = a * Mathf.Pow(c, currentAngle * (1 / Mathf.Tan(b)));

            Vector3 position = GetVector2FromAngle(currentAngle);
            position = position.normalized * radius;
            float randAngle = Random.Range(0, 360);
            position += GetVector2FromAngle(randAngle) * .05f;
            transform.position = startPos + position;

            elapsed += Time.unscaledDeltaTime;

            float shakeTime = 0;
            while (shakeTime < timeBetweenShakes)
            {
                shakeTime += Time.unscaledDeltaTime;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(afterWait);
    }

    private Vector3 GetVector2FromAngle(float angle)
    {
        float degAngle = Mathf.Deg2Rad * angle;
        float x = Mathf.Cos(degAngle);
        float y = Mathf.Sin(degAngle);

        return new Vector3(x, y);
    }
}

public enum ShakeType
{
    SmallHori, SmallVert, Large
}
