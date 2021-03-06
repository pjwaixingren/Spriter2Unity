﻿using UnityEngine;
using System.Collections;
using System.Xml;
using UnityEditor;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor
{
    public static class PrefabUtils
    {
        public static float PixelScale = 0.01f;
        public static float ZSpacing = 0.1f;

        public static void BakeTransforms(this Ref childRef, out Vector3 localPosition, out Vector3 localEulerAngles, out Vector3 localScale)
        {
            TimelineKey key = childRef.Referenced;

            localPosition = Vector3.zero;
            localScale = Vector3.one;
            localEulerAngles = Vector3.zero;

            var unmapped = childRef.Unmapped;
            var spatial = childRef.Referenced as SpatialTimelineKey;
            if (spatial != null)
            {
                localPosition = unmapped.Position;

                var spriteKey = key as SpriteTimelineKey;
                if (spriteKey != null)
                {
                    var sinA = Mathf.Sin(unmapped.Angle);
                    var cosA = Mathf.Cos(unmapped.Angle);

                    var pvt = spriteKey.File.GetPivotOffetFromMiddle();

                    pvt.x *= unmapped.Scale.x;
                    pvt.y *= unmapped.Scale.y;

                    var rotPX = pvt.x * cosA - pvt.y * sinA;
                    var rotPY = pvt.x * sinA + pvt.y * cosA;

                    localPosition.x += rotPX;
                    localPosition.y += rotPY;

                    localScale.x = unmapped.Scale.x;
                    localScale.y = unmapped.Scale.y;
                    localPosition.z = ((ObjectRef)childRef).ZIndex * -ZSpacing;
                }
                localPosition *= PixelScale;
                localEulerAngles = new Vector3(0, 0, unmapped.Angle_Deg);
            }
        }
    }
    public static class XmlUtils
    {
        public static bool TryGetString(this XmlNode node, string key, out string value)
        {
            value = default(string);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = true;
                value = attr.Value;
            }

            return parsed;
        }

        public static bool TryGetInt(this XmlNode node, string key, out int value)
        {
            value = default(int);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = int.TryParse(attr.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

            return parsed;
        }

        public static bool TryGetFloat(this XmlNode node, string key, out float value)
        {
            value = default(float);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

            return parsed;
        }

        public static bool TryGetVector2(this XmlNode node, out Vector2 value)
        {
            value = default(Vector2);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;

            return parsed;
        }

        public static bool TryGetVector3(this XmlNode node, out Vector3 value)
        {
            value = default(Vector3);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;
            if (TryGetFloat(node, "z", out tmp)) value.z = tmp;
            else parsed = false;

            return parsed;
        }

        public static string GetString(this XmlNode node, string key, string defaultVal)
        {
            string value = defaultVal;
            var attr = node.Attributes[key];
            if(attr !=null)
            {
                value = attr.Value;
            }
            return value;
        }

        public static int GetInt(this XmlNode node, string key, int defaultVal)
        {
            int value = defaultVal;
            int tmp;
            if (TryGetInt(node, key, out tmp)) value = tmp;

            return value;
        }

        public static float GetFloat(this XmlNode node, string key, float defaultVal)
        {
            var value = defaultVal;
            float tmp;
            if (TryGetFloat(node, key, out tmp)) value = tmp;

            return value;
        }

        public static Vector2 GetVector2(this XmlNode node, Vector2 defaultVal)
        {
            var value = defaultVal;
            float tmp;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;

            return value;
        }

        public static Vector3 GetVector3(this XmlNode node)
        {
            var value = default(Vector3);
            float tmp;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            if (TryGetFloat(node, "z", out tmp)) value.z = tmp;

            return value;
        }
	}
    public static class AnimationCurveUtils
    {
        public static void AddKeyIfChanged(this AnimationCurve curve, Keyframe keyframe)
        {
            var keys = curve.keys;
            //If this is the first key on this curve, always add
            //NOTE: Add TWO copies of the first frame, then we adjust the last frame as we move along
            //This guarantees a minimum of two keys in each curve
            if (keys.Length == 0)
            {
                curve.AddKey(keyframe);
                keyframe.time += float.Epsilon;
                curve.AddKey(keyframe);
            }
            else
            {
                //TODO: This method of keyframe reduction causes artifacts in animations that are supposed to deliberately pause
                //Find the last keyframe
                Keyframe lastKey = keys[keys.Length - 1];
                if (lastKey.time >= keyframe.time)
                    Debug.LogError("Keyframes not supplied in consecutive order!!!");

                //Grab 2 frames ago
                var last2Key = keys[keys.Length - 2];

                //If the previous 2 frames were different, add a new frame
                if (lastKey.value != last2Key.value)
                {
                    curve.AddKey(keyframe);
                }
                //The previous frame is redundant - just move it
                else
                {
                    curve.MoveKey(keys.Length - 1, keyframe);
                }
            }
        }
    }
}