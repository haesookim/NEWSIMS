using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    protected static T _instance;
    protected bool _enabled;
    
    public static T Instance
    {
        get
        {
           if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						_instance = obj.AddComponent<T> ();
					}
				}
				return _instance;
        }
    }

    public virtual void Awake()
    {
       
    }

	
}
