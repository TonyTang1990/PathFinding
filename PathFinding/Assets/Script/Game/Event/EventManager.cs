using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class MyIntEvent : UnityEvent<int>
{

}

public class EventManager : MonoBehaviour {

	private Dictionary<string, UnityEvent> mEventDictionary;

    private Dictionary<string, MyIntEvent> mIntEventDictionary;

	public static EventManager mEMInstance = null;

	void Awake()
	{
		if (mEMInstance == null) {
			mEMInstance = this;
			mEMInstance.Init();
		} else if (mEMInstance != this) {
			Destroy(gameObject);
		}
	}

	void Init()
	{
		if (mEventDictionary == null) {
			mEventDictionary = new Dictionary<string, UnityEvent>();
		}
        if (mIntEventDictionary == null)
        {
            mIntEventDictionary = new Dictionary<string, MyIntEvent>();
        }
	}

	public void StartListening(string eventname, UnityAction listener)
	{
		UnityEvent evt = null;
		if (mEMInstance.mEventDictionary.TryGetValue (eventname, out evt)) {
			evt.AddListener (listener);
		} else {
			evt = new UnityEvent();
			evt.AddListener(listener);
			mEMInstance.mEventDictionary.Add(eventname, evt);
		}
	}

	public void StopListening(string eventname, UnityAction listener)
	{
		if (mEMInstance == null) {
			return ;
		}
		UnityEvent thisevent = null;
		if (mEMInstance.mEventDictionary.TryGetValue (eventname, out thisevent)) {
			thisevent.RemoveListener(listener);
		}
	}

    public void StartListening(string eventname, UnityAction<int> listener)
    {
        MyIntEvent evt = null;
        if (mEMInstance.mIntEventDictionary.TryGetValue(eventname, out evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new MyIntEvent();
            evt.AddListener(listener);
            mEMInstance.mIntEventDictionary.Add(eventname, evt);
        }
    }

    public void StopListening(string eventname, UnityAction<int> listener)
    {
        if (mEMInstance == null)
        {
            return;
        }
        MyIntEvent thisevent = null;
        if (mEMInstance.mIntEventDictionary.TryGetValue(eventname, out thisevent))
        {
            thisevent.RemoveListener(listener);
        }
    }

    public bool HasListening(string eventname)
    {
        if (mEMInstance == null)
        {
            return false;
        }
        UnityEvent thisevent = null;
        MyIntEvent intevent = null;
        if (mEMInstance.mEventDictionary.TryGetValue(eventname, out thisevent))
        {
            if(thisevent != null)
            {
                return true;
            }
        }

        if (mEMInstance.mIntEventDictionary.TryGetValue(eventname, out intevent))
        {
            if (intevent != null)
            {
                return true;
            }
        }

        return false;
    }

	public void TriggerEvent(string eventname, int p = 0)
	{
		UnityEvent thisevent = null;
		if (mEMInstance.mEventDictionary.TryGetValue (eventname, out thisevent)) {
			if(thisevent != null)
            {
                thisevent.Invoke();
            }
		}

        MyIntEvent intevent = null;
        if (mEMInstance.mIntEventDictionary.TryGetValue(eventname, out intevent))
        {
            if (intevent != null)
            {
                intevent.Invoke(p);
            }
        }
	}
}
