using UnityEngine;
using System.Collections;

public abstract class Subject : MonoBehaviour
{
    private readonly ArrayList observers = new ArrayList();

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach(IObserver observer in observers)
        {
            observer.Notify(this);
        }
    }
}
