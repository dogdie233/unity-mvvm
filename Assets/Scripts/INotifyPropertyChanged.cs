using System;

public class PropertyChangedTreeEventArgs : EventArgs
{
    public PropertyChangedTreeEventArgs(string propertyName, PropertyChangedTreeEventArgs childEvent)
    {
        PropertyName = propertyName;
        ChildEvent = childEvent;
    }

    public string PropertyName { get; }
    public PropertyChangedTreeEventArgs ChildEvent { get; }
}

public delegate void PropertyTreeChangedEventHandler(object sender, PropertyChangedTreeEventArgs e);

public interface INotifyPropertyTreeChanged
{
    event PropertyTreeChangedEventHandler PropertyTreeChanged;
}
