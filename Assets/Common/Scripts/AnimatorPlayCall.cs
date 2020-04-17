using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animator 播放动画回调相关
/// </summary>
public class AnimatorPlayCall : MonoBehaviour
{
    List<AnimatorPlayCallParam> Pools = new List<AnimatorPlayCallParam>();

    static AnimatorPlayCall instance;
    public static AnimatorPlayCall I
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("AnimatorPlayCall").AddComponent<AnimatorPlayCall>();
                GameObject.DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    /// <summary>
    /// 检测Animator是否播放完毕
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void OnIsPlayOver(Animator animator, bool isCurrent, System.Action action)
    {
        if (action == null)
            return;
        Pools.Add(new AnimatorPlayCallParam(animator, isCurrent, action));
    }

    void Update()
    {
        if (Pools.Count > 0)
        {
            for (int i = 0; i < Pools.Count; i++)
            {
                if (Pools[i].Check())
                {
                    Pools.Remove(Pools[i]);
                }
            }
        }
    }

    /// <summary>
    /// 播放回调参数
    /// </summary>
    class AnimatorPlayCallParam
    {
        public AnimatorPlayCallParam(Animator animator, bool isCurrent, System.Action call)
        {
            Layer = 0;
            Action = call;
            Animator = animator;
            IsCurrent = isCurrent;
        }

        public AnimatorPlayCallParam(Animator animator, bool isCurrent, int layer, System.Action call)
        {
            Layer = layer;
            Action = call;
            Animator = animator;
            IsCurrent = isCurrent;
        }

        /// <summary>
        /// 层
        /// </summary>
        public int Layer { private set; get; }

        /// <summary>
        /// 回调行为
        /// </summary>
        public System.Action Action { private set; get; }

        /// <summary>
        /// Animator 动画
        /// </summary>
        public Animator Animator { private set; get; }

        /// <summary>
        /// 是否当前动画
        /// </summary>
        public bool IsCurrent { private set; get; }

        /// <summary>
        /// 缓存的NameHash
        /// </summary>
        private int shortNameHash = 0;

        private AnimatorStateInfo CurInfo
        {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(Layer);
            }
        }

        private AnimatorStateInfo NextInfo
        {
            get
            {
                return Animator.GetNextAnimatorStateInfo(Layer);
            }
        }

        /// <summary>
        /// 每帧检测动画状态
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            if (Action == null)
                return true;
            if (IsCurrent)
            {
                if (shortNameHash == 0 && CurInfo.shortNameHash != 0)
                {
                    shortNameHash = CurInfo.shortNameHash;
                }
            }
            else
            {
                if (shortNameHash == 0 && NextInfo.shortNameHash != 0)
                {
                    shortNameHash = NextInfo.shortNameHash;
                }
            }
            if (CurInfo.shortNameHash != shortNameHash && shortNameHash != NextInfo.shortNameHash)
            {
                Action?.Invoke();
                return true;
            }
            return false;
        }
    }
}

/// <summary>
/// Animator扩展
/// </summary>
public static class AnimatorExtension
{
    public static void SetTrigger(this Animator animater, string name, System.Action action)
    {
        animater.SetTrigger(name);
        AnimatorPlayCall.I.OnIsPlayOver(animater, false, action);
    }

    public static void SetBool(this Animator animater, string name, bool value, System.Action action)
    {
        animater.SetBool(name, value);
        AnimatorPlayCall.I.OnIsPlayOver(animater, false, action);
    }

    public static void Play(this Animator animater, string name, System.Action action)
    {
        animater.Play(name);
        AnimatorPlayCall.I.OnIsPlayOver(animater, true, action);
    }

    public static void OnOver(this Animator animater, bool isCurrent, System.Action action)
    {
        AnimatorPlayCall.I.OnIsPlayOver(animater, isCurrent, action);
    }
}

