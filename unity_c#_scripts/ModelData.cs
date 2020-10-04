using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ModelData
{
    public JsonInput request;
    public FirstShapeRes response;

    public ModelData()
    {
        request = new JsonInput();
        response = new FirstShapeRes();
    }

    public ModelData(JsonInput request, FirstShapeRes response)
    {
        this.request = request;
        this.response = response;
    }
}
