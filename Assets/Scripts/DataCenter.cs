using System.Runtime.CompilerServices;

public class ChartInfo : INotifyPropertyTreeChanged
{
    private string name = "Name of a chart";
    private int progress = 0;

    public string Name
    {
        get => name;
        set
        {
            if (value == name) return;
            name = value;
            PropertyTreeChanged?.Invoke(this, new PropertyChangedTreeEventArgs(nameof(Name), null));
        }
    }

    public int Progress
    {
        get => progress;
        set
        {
            if (value == progress) return;
            progress = value;
            PropertyTreeChanged?.Invoke(this, new PropertyChangedTreeEventArgs(nameof(Progress), null));
        }
    }

    public event PropertyTreeChangedEventHandler PropertyTreeChanged;
}

public static class DataCenter
{
    private static string testText;
    private static ChartInfo currentChart;

    public static string TestText
    {
        get => testText;
        set
        {
            if (value == testText) return;
            testText = value;
            PropertyTreeChanged?.Invoke(null, new PropertyChangedTreeEventArgs(nameof(TestText), null));
        }
    }
    public static ChartInfo CurrentChart
    {
        get => currentChart;
        set
        {
            if (value == currentChart) return;
            if (currentChart != null)
                currentChart.PropertyTreeChanged -= (_, e) => ChildPropertyChangedEventHanlder(e);
            currentChart = value;
            if (value != null)
                currentChart.PropertyTreeChanged += (_, e) => ChildPropertyChangedEventHanlder(e);
            PropertyTreeChanged?.Invoke(null, new PropertyChangedTreeEventArgs(nameof(CurrentChart), null));
        }
    }

    static DataCenter()
    {
        TestText = "test";
        CurrentChart = new ChartInfo();
    }

    public static event PropertyTreeChangedEventHandler PropertyTreeChanged;

    private static void ChildPropertyChangedEventHanlder(PropertyChangedTreeEventArgs e, [CallerMemberName] string caller = "")
    {
        PropertyTreeChanged?.Invoke(null, new PropertyChangedTreeEventArgs(caller, e));
    }
}