using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomEventsInput : StandaloneInputModule {

    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }
}
