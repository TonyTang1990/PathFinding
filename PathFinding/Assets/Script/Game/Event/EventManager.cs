﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

	private Dictionary<string, UnityEvent> mEventDictionary;

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

    public bool HasListening(string eventname)
    {
        if (mEMInstance == null)
        {
            return false;
        }
        UnityEvent thisevent = null;
        if (mEMInstance.mEventDictionary.TryGetValue(eventname, out thisevent))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	public void TriggerEvent(string eventname)
	{
		UnityEvent thisevent = null;
		if (mEMInstance.mEventDictionary.TryGetValue (eventname, out thisevent)) {
			thisevent.Invoke();
		}
	}
}
