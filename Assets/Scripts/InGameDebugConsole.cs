using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InGameDebugConsole : MonoBehaviour
{
    // --- ��� �߰� ���� ---
    [Tooltip("�� ����Ʈ�� ���Ե� ������ �ִ� �α״� ���õ˴ϴ�.")]
    public List<string> ignoredLogMessages = new List<string>();
    // --- ��� �߰� �� ---

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
        // --- ������ �޽��� ���� ���� ---
        foreach (string ignoredMessage in ignoredLogMessages)
        {
            if (!string.IsNullOrEmpty(ignoredMessage) && logString.Contains(ignoredMessage))
            {
                return; // ���� ����Ʈ�� ���Ե� �޽����̹Ƿ� ���⼭ �Լ� ����
            }
        }
        // --- ������ �޽��� ���� ���� �� ---

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