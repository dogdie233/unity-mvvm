using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnityEngine;
using UnityEngine.Assertions;

public interface IText
{
    void SetText(string text);
}

public class DataBinding : MonoBehaviour, IText
{
    private IText nextText;
    [SerializeField] private string path;
    private Func<string> valueGetter;
    private Action<string> valueSetter;
    private string[] memberNames;

    private static Dictionary<Type, Expression> converters = new Dictionary<Type, Expression>()
    {
        { typeof(int), (Expression<Func<string, int>>)((string s) => int.Parse(s)) }
    };

    public void SetText(string text)
    {
        valueSetter?.Invoke(text);
    }

    public string Path
    {
        get => path;
        set
        {
            Assert.IsNotNull(value);
            if (path == value) return;
            path = value;
            RebuildExpression();
        }
    }

    private void Start()
    {
        var components = GetComponents<IText>();
        var foundSelf = false;
        foreach (var component in components)
        {
            if (foundSelf)
            {
                nextText = component;
                break;
            }
            if (component == this) foundSelf = true;
        }
        if (nextText == null)
        {
            Debug.LogError("Can't find next IText!");
        }

        DataCenter.PropertyTreeChanged += OnPropertyChanged;
        RebuildExpression();
        nextText.SetText(valueGetter());
    }

    private void OnDestroy()
    {
        DataCenter.PropertyTreeChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedTreeEventArgs e)
    {
        var expressionChars = path.ToCharArray();
        var index = 0;
        while (true)
        {
            var propertyNameChars = e.PropertyName.ToCharArray();
            for (var i = 0; i < propertyNameChars.Length; i++)
            {
                if (index >= expressionChars.Length) return;
                if (propertyNameChars[i] != expressionChars[index++]) return;
            }
            e = e.ChildEvent;
            if (e == null) break;  // ×îÎ²
            if (index >= path.Length || expressionChars[index++] != '.') return;
        }
        if (index != expressionChars.Length && expressionChars[index] != '.') return;
        nextText?.SetText(valueGetter());
    }

    private void RebuildExpression()
    {
        memberNames = path.Split('.');
        if (memberNames.Length == 0)
        {
            Debug.LogError("Path can't not be empty");
            return;
        }

        Expression memberExp = Expression.Property(null, typeof(DataCenter).GetProperty(memberNames[0]));

        for (int i = 1; i < memberNames.Length; i++)
        {
            var memberName = memberNames[i];
            memberExp = Expression.Property(memberExp, memberName);
        }
        var methodToStringExp = Expression.Call(memberExp, "ToString", Array.Empty<Type>());
        valueGetter = Expression.Lambda<Func<string>>(methodToStringExp).Compile();

        var valueParameterExp = Expression.Parameter(typeof(string));
        Expression converterInvokeExp = null;
        var targetType = Expression.Lambda<Func<Type>>(Expression.Call(memberExp, typeof(object).GetMethod("GetType"))).Compile()();
        if (targetType == typeof(string))
            converterInvokeExp = valueParameterExp;
        else if (converters.TryGetValue(targetType, out var converter))
            converterInvokeExp = Expression.Invoke(converter, valueParameterExp);
        else
        {
            Debug.LogWarning($"Can't found the converter (to {targetType})");
            return;
        }

        var methodSetPropertyExp = Expression.Assign(memberExp, converterInvokeExp);
        valueSetter = Expression.Lambda<Action<string>>(methodSetPropertyExp, valueParameterExp).Compile();
    }
}
