using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SignalData
{
    public List<double> x_val;
    public List<double> y_val;

    public SignalData()
    {
        x_val = new List<double>();
        y_val = new List<double>();
    }
    
    public SignalData(List<double> x_val, List<double> y_val)
    {
        this.x_val = x_val;
        this.y_val = y_val;
    }
}
