using System;
using System.Linq;
using System.Collections.Generic;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;
using UnityEngine;

public static class UnityExtensions
{

    #region LIST

    public static void FillAdd<T>(this List<T> list, T item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = item;
                return;
            }
        }
    }

    public static void FillAdd<T>(this List<T> list, T item, ref int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = item;
                index = i;
                return;
            }
        }

        list.Add(item);
        index = list.Count - 1;
    }

    public static bool ContainNullItem<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Modify the current color based on RGB color.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    public static void RGBToUnity(this Color color, float r, float g, float b, float a = 255f)
    {
        color.r = r / 255f;
        color.g = g / 255f;
        color.b = b / 255f;
        color.a = a / 255f;

        Debug.Log(color);
    }
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Round up a Vector3 and return it as Vector3Int.
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3Int ToRoundVector3Int(this Vector3 v3)
	{
		Vector3Int v3Int = new Vector3Int();
		v3Int.x = Mathf.RoundToInt(v3.x);
		v3Int.y = Mathf.RoundToInt(v3.y);
		v3Int.z = Mathf.RoundToInt(v3.z);
		return v3Int;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector3Int based on a Vector3.
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Vector3Int ToVector3Int(this Vector3 v3)
	{
		Vector3Int v3Int;

		v3Int = new Vector3Int((int)v3.x, (int)v3.y, (int)v3.z);

		return v3Int;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector3 based on a Vector3Int.
    /// </summary>
    /// <param name="v3Int"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this Vector3Int v3Int)
	{
		Vector3 v3;

		v3 = new Vector3(v3Int.x, v3Int.y, v3Int.z);

		return v3;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector2Int based on a Vector3Int using X and Z.
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector2Int ToVector2IntXZ(this Vector3Int v3)
	{
		Vector2Int v2Int = new Vector2Int();
		v2Int.x = v3.x;
		v2Int.y = v3.z;
		return v2Int;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector2Int Based on a Vector2.
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector2Int ToVector2Int(this Vector2 v2)
	{
		Vector2Int v2Int;

		v2Int = new Vector2Int((int)v2.x, (int)v2.y);

		return v2Int;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector3Int based on a Vector2Int.
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector3Int ToVector3Int(this Vector2Int v2)
	{
		Vector3Int v3Int = new Vector3Int();
		v3Int.x = v2.x;
		v3Int.y = 0;
		v3Int.z = v2.y;
		return v3Int;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return a Vector2 based on a Vector2Int.
    /// </summary>
    /// <param name="v2Int"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector2Int v2Int)
	{
		Vector2 v2;

		v2 = new Vector2(v2Int.x, v2Int.y);

		return v2;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return the played animation clip in the given layer.
    /// </summary>
    /// <param name="_animator"></param>
    /// <param name="_layer"></param>
    /// <returns></returns>
    public static AnimationClip GetCurrentAnimationClip(this Animator _animator, int _layer)
	{
		AnimationClip clip = new AnimationClip();
		AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(_layer);
		if (clipInfos.Length > 0)
		{
			clip = clipInfos[0].clip;
		}

		return clip;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Covert the object to a MonoBehaviour.
    /// </summary>
    /// <param name="_object"></param>
    /// <returns></returns>
	public static MonoBehaviour ToMonoBehaviour(this object _object)
	{
		return (MonoBehaviour)_object;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Remove this component from the GameObject on which his attach to.
    /// </summary>
    /// <param name="_comp"></param>
    /// <param name="_delay"></param>
    public static void RemoveFromGameObject(this Component _comp, float _delay = 0f)
    {
        UnityEngine.Object.Destroy(_comp, _delay);
    }
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Check if the following Component is present on this GameObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <returns></returns>
	public static bool HaveComponent<T>(this Component _comp)
	{
		return _comp.GetComponent<T>() != null;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Check if the Component is present on at least one child of this GameObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <param name="_includeInactive"></param>
    /// <returns></returns>
	public static bool HaveComponentInChildren<T>(this Component _comp, bool _includeInactive = false)
	{
		return _comp.GetComponentOnlyInChildren<T>(_includeInactive) != null;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Check if the Component is present on at least one parent of this GameObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <returns></returns>
	public static bool HaveComponentInParent<T>(this Component _comp)
	{
		return _comp.GetComponentOnlyInParent<T>() != null;
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Same as the native function GetComponentInParent() but exclude the GameObject on which this function is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <returns></returns>
    public static T GetComponentOnlyInParent<T>(this Component _comp)
    {
        Transform parent = _comp.transform;
        do
        {
            parent = parent.parent;

            if (parent.GetComponent<T>() != null)
            {
                return parent.GetComponent<T>();
            }

        } while (parent != null);

        return default(T);
    }
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Same as the native function GetComponentInChildren() but exclude the GameObject on which this function is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <param name="_includeInactive"></param>
    /// <returns></returns>
    public static T GetComponentOnlyInChildren<T>(this Component _comp, bool _includeInactive = false)
    {
        foreach (Transform childTrans in _comp.transform.GetChilds())
        {
            if (childTrans.gameObject.activeSelf || _includeInactive)
            {
                if (childTrans.GetComponent<T>() != null)
                {
                    return childTrans.GetComponent<T>();
                }
            }
        }

        return default(T);
    }
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return the Component on the specific child if there one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <param name="_childIndex"></param>
    /// <returns></returns>
    public static T GetComponentOnlyInChildren<T>(this Component _comp, int _childIndex)
    {
        Transform child = _comp.transform.GetChild(_childIndex);
        if (child != null)
        {
            return child.GetComponent<T>();
        }
        return default(T);
    }
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Same as the native function GetComponentsInChildren() but exclude the GameObject on which this function is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_comp"></param>
    /// <param name="_includeInactive"></param>
    /// <returns></returns>
    public static T[] GetComponentsOnlyInChildren<T>(this Component _comp, bool _includeInactive = false)
	{
		List<T> childsComp = new List<T>();

		foreach (Transform childTrans in _comp.transform.GetChilds())
		{
			if (childTrans.gameObject.activeSelf || _includeInactive)
			{
				if (childTrans.GetComponent<T>() != null)
				{
					childsComp.Add(childTrans.GetComponent<T>());
				}
			}
		}

		return childsComp.ToArray();
	}
    // ----------------------------------------------------------------------------------------------------------**

    /// <summary>
    /// Return the childs of this transform as an array of Transform.
    /// </summary>
    /// <param name="_trans"></param>
    /// <returns></returns>
    public static Transform[] GetChilds(this Transform _trans)
    {
        Transform[] childs = new Transform[_trans.childCount];

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i] = _trans.GetChild(i);
        }

        return childs;
    }
}
