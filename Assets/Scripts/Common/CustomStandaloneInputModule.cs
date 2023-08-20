using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのクリック機能を消した拡張 StandaloneInputModule
/// </summary>
public class CustomStandaloneInputModule : StandaloneInputModule
{
    /// <summary>
    /// 毎フレーム監視
    /// </summary>
    public override void Process()
    {
        if (!this.eventSystem.isFocused && this.CustomShouldIgnoreEventsOnNoFocus())
        {
            return;
        }

        bool usedEvent = this.SendUpdateEventToSelectedObject();

        if (this.eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
            {
                usedEvent |= this.SendMoveEventToSelectedObject();
            }

            if (!usedEvent)
            {
                this.SendSubmitEventToSelectedObject();
            }
        }
    }

    /// <summary>
    /// フォーカスされている EventObject の監視?
    /// </summary>
    /// <returns>bool</returns>
    private bool CustomShouldIgnoreEventsOnNoFocus()
    {
#if UNITY_EDITOR
        return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
    }
}
