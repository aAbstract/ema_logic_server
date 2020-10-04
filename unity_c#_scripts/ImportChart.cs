using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeCharts;
using TMPro;
using SimpleFileBrowser;

public class ImportChart : MonoBehaviour
{
    public LineChart inputChart;
    public LineChart outputChart;
    public TextMeshProUGUI errTxt;

    JsonInput request = new JsonInput();
    FirstShapeRes res = new FirstShapeRes();

    void OnEnable()
    {
        if (errTxt)
        {
            errTxt.text = "";
        }
        //request = new JsonInput();
        //res = new FirstShapeRes();
        RemoveChartPoints(inputChart);
        RemoveChartPoints(outputChart);
        Globals.modelInput = null;
        Globals.modelOutput = null;
    }

    void DrawModelInputChart()
    {
        if (inputChart)
        {
            for (int i = 0; i < request.args.data.x_vals.Count; i++)
            {
                AddChartPoint(request.args.data.x_vals[i], request.args.data.y_vals[i], inputChart);
            }
            inputChart.SetDirty();
        }
    }

    void DrawModelOutputChart(FirstShapeRes response)
    {
        if (outputChart)
        {
            for (int i = 0; i < response.model_td_x.Count; i++)
            {
                AddChartPoint(response.model_td_x[i], response.model_td_y[i], outputChart);
            }
            outputChart.SetDirty();
        }
    }

    #region Helpers
    void AddChartPoint(double x, double y, LineChart chart)
    {
        chart.GetChartData().DataSets[0].AddEntry(new LineEntry((float)x, (float)y));
    }
    void RemoveChartPoints(LineChart chart)
    {
        //for (int i = 0; i < chart.GetChartData().DataSets[0].Entries.Count; i++)
        //{
        //    RemoveChartPoint(chart);
        //}
        chart.GetChartData().DataSets[0].Clear();
        chart.SetDirty();
    }
    void RemoveChartPoint(LineChart chart)
    {
        LineDataSet dataSet = chart.GetChartData().DataSets[0];
        if (dataSet.Entries.Count > 0)
        {
            dataSet.RemoveEntry(dataSet.Entries.Count - 1);
            chart.SetDirty();
        }
    }
    #endregion

    #region LoadFunctionality
    IEnumerator ShowLoadDialogCoroutine()
    {
        FileBrowser.CheckPermission();
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, true, null, "Load File", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            ModelData loadedData = SaveModelSystem.LoadModel(FileBrowser.Result[0]);
            Debug.Log("Model Function: " + loadedData.request.api_func);
            request = loadedData.request;
            res = loadedData.response;
            Globals.modelInput = request;
            Globals.modelOutput = res;
            // Draw imported data 
            DrawModelInputChart();
            DrawModelOutputChart(res);
            // Calculate error in model 
            double errTerm = 0;
            if (errTxt)
            {
                for (int i = 0; i < request.args.data.y_vals.Count; i++)
                {
                    errTerm += Mathf.Pow(((float)res.model_td_y[i] - (float)request.args.data.y_vals[i]), 2);
                }
                errTxt.text = "Error: " + errTerm.ToString();
            }
        }
    }
    public void ImportBtnCallback()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    #endregion 
}
