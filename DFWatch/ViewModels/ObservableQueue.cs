﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.ViewModels;

/// <summary>
/// https://stackoverflow.com/a/72226319/15237757
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class ObservableQueue<TItem> : Queue<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public new void Enqueue(TItem item)
    {
        base.Enqueue(item);
        OnPropertyChanged();
        OnCollectionChanged(NotifyCollectionChangedAction.Add, item, this.Count - 1);
    }

    new public TItem Dequeue()
    {
        TItem removedItem = base.Dequeue();
        OnPropertyChanged();
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, 0);
        return removedItem;
    }

    new public bool TryDequeue(out TItem result)
    {
        if (base.TryDequeue(out result))
        {
            OnPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, result, 0);
            return true;
        }
        return false;
    }

    new public void Clear()
    {
        base.Clear();
        OnPropertyChanged();
        OnCollectionChangedReset();
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, TItem item, int index)
      => this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));

    private void OnCollectionChangedReset()
      => this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    private void OnPropertyChanged() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Count)));
}
