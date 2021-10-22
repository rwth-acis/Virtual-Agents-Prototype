using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;

public class SelectorInputCommand : CompositeNode
{
    protected override void OnStart() {
        string input = "";
        if (String.IsNullOrEmpty(input) || input == "Abbruch" || input == "Weiter")
        {
            return;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
