using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InGameDebugConsole : MonoBehaviour
{
    // --- 기능 추가 시작 ---
    [Tooltip("이 리스트에 포함된 문구가 있는 로그는 무시됩니다.")]
    public List<string> ignoredLogMessages = new List<string>();
    // --- 기능 추가 끝 ---

    public enum LogLevel { All, WarningsAndErrors, ErrorsOnly }
    [Tooltip("Select the level of logs to display.")]
    public LogLevel filterLevel = LogLevel.All;

    public TextMeshProUGUI logText;

    private Queue<string> logMessages = new Queue<string>();

    [SerializeField]
    private int maxLines = 25;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // --- 무시할 메시지 필터 로직 ---
        foreach (string ignoredMessage in ignoredLogMessages)
        {
            if (!string.IsNullOrEmpty(ignoredMessage) && logString.Contains(ignoredMessage))
            {
                return; // 무시 리스트에 포함된 메시지이므로 여기서 함수 종료
            }
        }
        // --- 무시할 메시지 필터 로직 끝 ---

        if (filterLevel == LogLevel.ErrorsOnly && type != LogType.Error && type != LogType.Exception)
        {
            return;
        }
        if (filterLevel == LogLevel.WarningsAndErrors && type == LogType.Log)
        {
            return;
        }

        string formattedLog = "";
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                formattedLog = $"<color=red>{logString}</color>";
                break;
            case LogType.Warning:
                formattedLog = $"<color=yellow>{logString}</color>";
                break;
            default:
                formattedLog = logString;
                break;
        }

        logMessages.Enqueue(formattedLog);

        while (logMessages.Count > maxLines)
        {
            logMessages.Dequeue();
        }

        UpdateLogText();
    }

    private void UpdateLogText()
    {
        logText.text = string.Join("\n", logMessages);
    }
}