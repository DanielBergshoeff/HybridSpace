using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataContainer : MonoBehaviour {

    [SerializeField] private TestViewContainer container;

    private const float SPAWNTIME = 2f;
    private float time = 0;
    private DataContainer<TestData> dataContainer = new DataContainer<TestData>();
    private int count = 0;

    private void Start() {
        container.Initialize(dataContainer);
        dataContainer.AddData(new TestData(new Vector3(dataContainer.DataItems.Count, 0, 0)));
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.N)) {
            dataContainer.AddData(new TestData(new Vector3(dataContainer.DataItems.Count, 0, 0)));
        }

        time += Time.deltaTime;
        if (time > SPAWNTIME) {
            time = 0;
            count++;
            dataContainer.DataItems[0].Position = new Vector3(count, 0, 0);
            Debug.Log(dataContainer.DataItems[0].Position);
        }
    }
}