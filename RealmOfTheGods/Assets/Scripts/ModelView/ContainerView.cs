using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewItem<T> : MonoBehaviour {
    public T Data;

    public virtual void Initialize(T data) {
        Data = data;
    }

    public virtual void UpdateData(T data) {
        Data = data;
    }

    public virtual void OnDestroy() {

    }
}

public class DataContainer<T> {
    readonly public Signal<T> DataContainerChangedSignal = new Signal<T>();

    public List<T> DataItems {
        get {
            if (dataItems == null) {
                dataItems = new List<T>();
            }
            return dataItems;
        }
    }
    private List<T> dataItems;

    public virtual void AddData(T item) {
        if (DataItems.Contains(item)) { return; }
        DataItems.Add(item);
        DataContainerChangedSignal.Dispatch(item);
    }
}


public abstract class ViewContainer<U, T> : MonoBehaviour where U : ViewItem<T> {
    public virtual DataContainer<T> DataContainer {
        get {
            return dataContainer;
        }
    }

    public U SourceItem;

    public List<U> ViewItems {
        get {
            if (viewItems == null) {
                viewItems = new List<U>();
            }
            return viewItems;
        }
    }
    private List<U> viewItems;
    private DataContainer<T> dataContainer = new DataContainer<T>();

    public void Initialize(DataContainer<T> dataContainer) {
        this.dataContainer = dataContainer;
        dataContainer.DataContainerChangedSignal.AddListener(OnDataContainerChanged);
    }

    protected virtual void Awake() {
        SourceItem.gameObject.SetActive(false);
    }

    protected virtual void OnViewAdded(U view) {

    }

    private void OnDataContainerChanged(T obj) {
        UpdateViews();
    }

    private void UpdateViews() {
        List<U> oldViews = new List<U>(ViewItems);
        ViewItems.Clear();

        foreach (T item in DataContainer.DataItems) {
            U view = oldViews.Find((obj) => obj.Data.Equals(item));
            if (view == null) {
                view = CreateView(item);
            } else {
                oldViews.Remove(view);
            }
            ViewItems.Add(view);
        }

        RemoveOldViews(oldViews);
    }

    private void RemoveOldViews(List<U> oldViews) {
        for (int i = oldViews.Count - 1; i >= 0; i--) {
            U view = oldViews[i];
            oldViews.Remove(view);
            DestroyView(view);
        }
    }

    private U CreateView(T item) {
        U newView = Instantiate(SourceItem);
        newView.gameObject.SetActive(true);
        newView.transform.SetParent(transform, false);
        newView.Initialize(item);
        OnViewAdded(newView);
        return newView;
    }

    private void DestroyView(U view) {
        view.OnDestroy();
        Destroy(view);
    }
}