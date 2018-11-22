using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestView : ViewItem<TestData>
{
    public override void Initialize(TestData data)
    {
        base.Initialize(data);
        transform.position = Data.Position;
    }
}