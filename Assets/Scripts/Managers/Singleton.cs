using System.Collections;
using UnityEngine;

public abstract class Singleton<T>: MonoBehaviour where T: Component {
    #region Variables
    public static T instance {
        get;
        private set;
    }

    public bool isReady {
        get;
        protected set;
    }
    #endregion

    #region Initialisation
    protected virtual void Awake() {
        if (instance != null) Destroy(instance.gameObject);
        instance = this as T;
    }

    protected virtual void Start() {
        StartCoroutine(CoroutineStart());
    }

    protected abstract IEnumerator CoroutineStart();
    #endregion
}