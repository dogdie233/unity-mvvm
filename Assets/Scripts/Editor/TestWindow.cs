using UnityEditor;
using UnityEngine;

public class TestWindow : EditorWindow
{
    private static string _testText;
    private static string _name;
    private static ChartInfo _chartInfo;

    [MenuItem("Window/Data Test")]
    static void Init()
    {
        TestWindow window = (TestWindow)EditorWindow.GetWindow(typeof(TestWindow));
        _testText = DataCenter.TestText;
        _name = DataCenter.CurrentChart.Name;
        _chartInfo = DataCenter.CurrentChart;
        window.Show();
    }

    void OnGUI()
    {
        _testText = EditorGUILayout.TextField(nameof(DataCenter.TestText), _testText);
        if (GUILayout.Button("确认"))
            DataCenter.TestText = _testText;

        _name = EditorGUILayout.TextField("CurrentChartName", _name);
        if (GUILayout.Button("对当前引用修改"))
            _chartInfo.Name = _name;
        if (GUILayout.Button("创建新引用并赋值"))
        {
            // Progress会归零因为默认是0
            _chartInfo = new ChartInfo() { Name = _name };
            DataCenter.CurrentChart = _chartInfo;
        }
    }
}