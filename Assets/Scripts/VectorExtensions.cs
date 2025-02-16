using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        public static Vector2[] _directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        public static Vector2[] _directions8 = { Vector2.right, Vector2.left, Vector2.up, Vector2.down,
            new Vector2(1,1).normalized, new Vector2(-1,1).normalized, new Vector2(1,-1), new Vector2(-1,-1) };
        public static Vector2 ClampMagnitude(this Vector2 direction, float maxMag)
        {
            float magnitude = direction.magnitude;

            if (magnitude > maxMag)
            {
                magnitude = maxMag;
            }

            float vectorMagnitude = magnitude / maxMag;

            return direction.normalized * vectorMagnitude;
        }

        public static Vector2 GetDelta(Vector2 start, Vector2 end)
        {
            return start - end;
        }

        public static Vector2 GetDirection(Vector2 start, Vector2 end)
        {
            return (start - end).normalized;
        }

        public static Vector2 Rotate(this Vector2 start, float angle)
        {
            float startAngle = start.GetVector2Angle();
            float newAngle = startAngle + angle;

            return GetVector2FromAngle(newAngle);
        }

        public static float GetVector2Angle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        public static float GetVector2AngleRadians(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        public static float Sin(float degrees)
        {
            return Mathf.Sin(degrees * Mathf.Deg2Rad);
        }
        public static float Cos(float degrees)
        {
            return Mathf.Cos(degrees * Mathf.Deg2Rad);
        }

        public static float Tan(float degrees)
        {
            return Mathf.Tan(degrees * Mathf.Deg2Rad);
        }

        public static Vector2 GetVector2FromAngle(float angle)
        {
            float degAngle = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(degAngle);
            float y = Mathf.Sin(degAngle);

            return new Vector2(x, y);
        }

        public static Vector2 GetVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 SetZToZero(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector3 SetZToZero(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector2 RandomPointInRadius(float centralAngle, float radialRange, float maxDistance)
        {
            float minRadialRange = centralAngle - (radialRange / 2);
            float maxRadialRange = centralAngle + (radialRange / 2);

            float distance = UnityEngine.Random.Range(0, maxDistance);
            float angle = UnityEngine.Random.Range(minRadialRange, maxRadialRange);

            Vector2 point = GetVector2FromAngle(angle).normalized * distance;
            return point;
        }

        public static Vector2 RandomPointInRadius(Vector2 direction, float radialRange, float maxDistance)
        {
            float centralAngle = GetVector2Angle(direction);

            float minRadialRange = centralAngle - (radialRange / 2);
            float maxRadialRange = centralAngle + (radialRange / 2);

            float distance = UnityEngine.Random.Range(0, maxDistance);
            float angle = UnityEngine.Random.Range(minRadialRange, maxRadialRange);

            Vector2 point = GetVector2FromAngle(angle).normalized * distance;
            return point;
        }

        public static Vector2 RandomValidPoint(byte validDirections, float radialRange, float maxDistance)
        {
            if (radialRange > 45) radialRange = 45;

            bool isRight = RightIsValid(validDirections);
            bool isLeft = LeftIsValid(validDirections);
            bool isUp = UpIsValid(validDirections);
            bool isDown = DownIsValid(validDirections);

            bool[] validityArr = new bool[4];
            validityArr[0] = isRight;
            validityArr[1] = isLeft;
            validityArr[2] = isUp;
            validityArr[3] = isDown;

            int validCount = 0;
            Vector2[] randomPoints = new Vector2[4];
            if (isRight)
            {
                var rightDir = Vector2.right;
                float rightRange = radialRange;
                if (!isDown)
                {

                    rightDir = GetVector2FromAngle(45 / 2);
                    rightRange = radialRange / 2;
                }
                else if (!isUp)
                {
                    rightDir = GetVector2FromAngle(315 + (45 / 2));
                    rightRange = radialRange / 2;
                }
                randomPoints[0] = RandomPointInRadius(rightDir, rightRange, maxDistance); validCount++;
            }
            if (isLeft)
            {
                var leftDir = Vector2.left;
                float leftRange = radialRange;

                if (!isDown)
                {
                    leftDir = GetVector2FromAngle(180 - (45 / 2)); ;
                    leftRange = radialRange / 2;
                }
                else if (!isUp)
                {
                    leftDir = GetVector2FromAngle(180 + (45 / 2)); ;
                    leftRange = radialRange / 2;
                }

                randomPoints[1] = RandomPointInRadius(leftDir, leftRange, maxDistance); validCount++;
            }
            if (isUp)
            {
                var upDir = Vector2.up;
                float upRange = radialRange;
                if (!isRight)
                {
                    upDir = GetVector2FromAngle(90 + (45 / 2));
                    upRange = radialRange / 2;
                }
                else if (!isLeft)
                {
                    upDir = GetVector2FromAngle(90 - (45 / 2));
                    upRange = radialRange / 2;
                }

                randomPoints[2] = RandomPointInRadius(upDir, upRange, maxDistance); validCount++;
            }
            if (isDown)
            {
                var downDir = Vector2.down;
                float downRange = radialRange;

                if (!isRight)
                {
                    downDir = GetVector2FromAngle(270 - (45 / 2));
                    downRange = radialRange / 2;
                }
                else if (!isLeft)
                {
                    downDir = GetVector2FromAngle(270 + (45 / 2));
                    downRange = radialRange / 2;
                }

                randomPoints[3] = RandomPointInRadius(downDir, downRange, maxDistance); validCount++;
            }

            Vector2[] randomValidPoints = new Vector2[validCount];
            int validIndex = 0;
            for (int i = 0; i < validCount; i++)
            {
                if (validityArr[i])
                {
                    randomValidPoints[validIndex++] = randomPoints[i];
                }
            }

            int randomIndex = UnityEngine.Random.Range(0, validCount);

            return randomValidPoints[randomIndex];
        }

        public static bool RightIsValid(byte validDirections) => ((validDirections >> 0) & 1) == 1;
        public static bool LeftIsValid(byte validDirections) => ((validDirections >> 1) & 1) == 1;
        public static bool UpIsValid(byte validDirections) => ((validDirections >> 2) & 1) == 1;
        public static bool DownIsValid(byte validDirections) => ((validDirections >> 3) & 1) == 1;

        public static Vector2 GetClosestDirection(Vector2 from)
        {
            var maxDot = -Mathf.Infinity;
            var ret = Vector3.zero;

            for (int i = 0; i < _directions.Length; i++)
            {
                var t = Vector3.Dot(from, _directions[i]);
                if (t > maxDot)
                {
                    ret = _directions[i];
                    maxDot = t;
                }
            }

            return ret;
        }

        public static Vector2 GetClosestDirection8(Vector2 from)
        {
            var maxDot = -Mathf.Infinity;
            var ret = Vector3.zero;

            for (int i = 0; i < _directions8.Length; i++)
            {
                var t = Vector3.Dot(from, _directions8[i]);
                if (t > maxDot)
                {
                    ret = _directions8[i];
                    maxDot = t;
                }
            }

            return ret;
        }

        private static int GetClosestDirectionIndex(Vector2 from)
        {
            var maxDot = -Mathf.Infinity;
            var index = 0;

            for (int i = 0; i < _directions.Length; i++)
            {
                var t = Vector3.Dot(from, _directions[i]);
                if (t > maxDot)
                {
                    index = i;
                    maxDot = t;
                }
            }

            return index;
        }
        private static int GetClosestDirectionIndex8(Vector2 from)
        {
            var maxDot = -Mathf.Infinity;
            var index = 0;

            for (int i = 0; i < _directions8.Length; i++)
            {
                var t = Vector3.Dot(from, _directions8[i]);
                if (t > maxDot)
                {
                    index = i;
                    maxDot = t;
                }
            }

            return index;
        }
    }
}