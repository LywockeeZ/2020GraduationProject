using UnityEngine;


    /// <summary>
    /// 继承Mono类的单例模式模板
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        /// <summary>
        /// 单例模式模板
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// awake时初始化实例，所以如果需要重写Awake()，要记得调用父类方法
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _instance = this as T;
        }
    }
