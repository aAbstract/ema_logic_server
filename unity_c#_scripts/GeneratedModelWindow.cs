using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeCharts;
using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine.Networking;

public class GeneratedModelWindow : MonoBehaviour
{
    public LineChart inputChart;
    public LineChart outputChart;

    void OnEnable()
    {
        // Plot input Data 
        try
        {
            RemoveChartPoints(inputChart);
            RemoveChartPoints(outputChart);
        }
        catch
        {
            Debug.Log("No Data was preloaded ... ");
        }
        DrawSignal(Globals.modelInput.args.data.x_vals, Globals.modelInput.args.data.y_vals, inputChart);
        StartCoroutine(GenerateModelFromFilteredSignal());
    }

    #region ChartFunc
    void DrawSignal(List<double> x_vals, List<double> y_vals, LineChart chart)
    {
        for (int i = 0; i < x_vals.Count; i++)
        {
            AddChartPoint(x_vals[i], y_vals[i], chart);
        }
        chart.SetDirty();
    }
    void AddChartPoint(double x, double y, LineChart chart)
    {
        if (chart != null)
        {
            chart.GetChartData().DataSets[0].AddEntry(new LineEntry((float)x, (float)y));
        }
    }
    void RemoveChartPoints(LineChart chart)
    {
        if (chart != null)
        {
            chart.GetChartData().DataSets[0].Clear();
        }
        chart.SetDirty();
    }
    #endregion

    #region Async
    IEnumerator GenerateModelFromFilteredSignal()
    {
        var request = new UnityWebRequest(Globals.url, "POST");
        JsonInput reqBody = Globals.modelInput;
        //reqBody.api_func = "model_gen";
        //reqBody.args = new Args();
        //reqBody.args.data = new Data();
        //reqBody.args.settings = new Settings();
        //reqBody.args.data.x_vals = Globals..x_val;
        //reqBody.args.data.y_vals = loadedSignal.y_val;
        //reqBody.args.settings.filter_ratio = filter_ratio;
        string jsonBody = JsonUtility.ToJson(reqBody) + ";";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        Debug.Log("Request: " + jsonBody);
        Debug.Log("Response: " + request.downloadHandler.data.ToString());
        FirstShapeRes res = JsonUtility.FromJson<FirstShapeRes>(Encoding.UTF8.GetString(request.downloadHandler.data));
        Globals.modelOutput = res;

        DrawSignal(res.model_td_x, res.model_td_y, outputChart);
    }
    #endregion

    #region Callbacks
    public void CloseGeneratedModelBtnCallback()
    {
        HelperClass.HideWindow(this.gameObject);
    }
    #endregion
}
