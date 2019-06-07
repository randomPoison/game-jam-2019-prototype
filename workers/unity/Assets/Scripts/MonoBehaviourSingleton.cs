using System;
using UnityEngine;

namespace Singleton
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T: MonoBehaviourSingleton<T>
    {
        private static T _instance = null;

        protected static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(T)} instance has not yet been created");
                }

                return _instance;
            }
        }

        #region Unity Lifecycle Methods
        protected virtual void Awake()
        {
            // If there's already an instance of the client controller, destroy the
            // duplicate instance.
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            // No instance already exists, so make this the current instance.
            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}
