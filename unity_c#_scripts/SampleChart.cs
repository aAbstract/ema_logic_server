using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AwesomeCharts;
using System.Net;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class SampleChart : MonoBehaviour
{
    public LineChart inputChart;
    public LineChart outputChart;
    public TextMeshProUGUI errTxt;

    List<double> in_x_data = new List<double> { 0.0, 0.010101010101010102, 0.020202020202020204, 0.030303030303030304, 0.04040404040404041, 0.05050505050505051, 0.06060606060606061, 0.07070707070707072, 0.08080808080808081, 0.09090909090909091, 0.10101010101010102, 0.11111111111111112, 0.12121212121212122, 0.13131313131313133, 0.14141414141414144, 0.15151515151515152, 0.16161616161616163, 0.17171717171717174, 0.18181818181818182, 0.19191919191919193, 0.20202020202020204, 0.21212121212121213, 0.22222222222222224, 0.23232323232323235, 0.24242424242424243, 0.25252525252525254, 0.26262626262626265, 0.27272727272727276, 0.2828282828282829, 0.29292929292929293, 0.30303030303030304, 0.31313131313131315, 0.32323232323232326, 0.33333333333333337, 0.3434343434343435, 0.3535353535353536, 0.36363636363636365, 0.37373737373737376, 0.38383838383838387, 0.393939393939394, 0.4040404040404041, 0.4141414141414142, 0.42424242424242425, 0.43434343434343436, 0.4444444444444445, 0.4545454545454546, 0.4646464646464647, 0.4747474747474748, 0.48484848484848486, 0.494949494949495, 0.5050505050505051, 0.5151515151515152, 0.5252525252525253, 0.5353535353535354, 0.5454545454545455, 0.5555555555555556, 0.5656565656565657, 0.5757575757575758, 0.5858585858585859, 0.595959595959596, 0.6060606060606061, 0.6161616161616162, 0.6262626262626263, 0.6363636363636365, 0.6464646464646465, 0.6565656565656566, 0.6666666666666667, 0.6767676767676768, 0.686868686868687, 0.696969696969697, 0.7070707070707072, 0.7171717171717172, 0.7272727272727273, 0.7373737373737375, 0.7474747474747475, 0.7575757575757577, 0.7676767676767677, 0.7777777777777778, 0.787878787878788, 0.797979797979798, 0.8080808080808082, 0.8181818181818182, 0.8282828282828284, 0.8383838383838385, 0.8484848484848485, 0.8585858585858587, 0.8686868686868687, 0.8787878787878789, 0.888888888888889, 0.8989898989898991, 0.9090909090909092, 0.9191919191919192, 0.9292929292929294, 0.9393939393939394, 0.9494949494949496, 0.9595959595959597, 0.9696969696969697, 0.9797979797979799, 0.98989898989899, 1.0 };
    List<double> in_y_data = new List<double> { 0.0, 1.0183261611006384, 0.6936523174365201, -0.4289077418601951, -0.6829998730840275, 0.37545736535505203, 1.3434740239838692, 0.9434723036625774, -0.18057226441582258, -0.3689911778989208, 0.7195003826283906, 1.6275953626987474, 1.148305533209522, 0.02189753727866861, -0.10530689915582436, 1.0038271964107461, 1.8435638944107684, 1.2823838988541945, 0.15388242100025973, 0.08442243031559193, 1.2060502286251547, 1.9706789075065496, 1.3268278963378775, 0.19807662393042091, 0.18429138713267257, 1.3119075733723615, 1.9967289036258173, 1.2715539987223619, 0.14623259158000557, 0.18769569426818422, 1.3166632743749966, 1.9192090700565234, 1.1163043070778782, -5.551115123125783e-16, 0.09797814597724008, 1.2255567526200086, 1.745571016235191, 0.8706955673522094, -0.22922477377504347, -0.07192002526150632, 1.053256599963242, 1.4924798377881838, 0.5532825650315922, -0.5214237586979691, -0.30076746636087187, 0.8223733742970281, 1.1841526913464815, 0.1897293294714113, -0.8499447754104859, -0.5611799955565699, 0.5611799955565715, 0.8499447754104827, -0.18972932947142837, -1.1841526913464824, -0.8223733742970264, 0.30076746636087337, 0.5214237586979649, -0.5532825650316017, -1.492479837788184, -1.0532565999632406, 0.07192002526150765, 0.22922477377504236, -0.8706955673522189, -1.7455710162351927, -1.2255567526200066, -0.09797814597723864, -8.881784197001252e-16, -1.1163043070778804, -1.9192090700565245, -1.3166632743749878, -0.18769569426817834, -0.1462325915800069, -1.2715539987223639, -1.9967289036258173, -1.311907573372356, -0.18429138713266713, -0.19807662393042014, -1.3268278963378761, -1.9706789075065494, -1.206050228625149, -0.0844224303155886, -0.15388242100026572, -1.282383898854206, -1.8435638944107686, -1.0038271964107441, 0.10530689915582747, -0.02189753727867505, -1.1483055332095298, -1.6275953626987474, -0.7195003826283757, 0.3689911778989218, 0.18057226441581947, -0.9434723036625834, -1.3434740239838667, -0.3754573653550392, 0.6829998730840272, 0.42890774186019287, -0.6936523174365236, -1.0183261611006365, -5.143516556418883e-15 };

    FirstShapeRes res = new FirstShapeRes();

    void OnEnable()
    {
        if (errTxt)
        {
            errTxt.text = "";
        }
        RemoveChartPoints(inputChart);
        RemoveChartPoints(outputChart);
        DrawSampleInputChart();
        JsonInput test_obj = new JsonInput();
        test_obj.api_func = "model_gen";
        test_obj.args = new Args();
        test_obj.args.settings = new Settings();
        test_obj.args.settings.filter_ratio = 0.5;
        test_obj.args.data = new Data();
        test_obj.args.data.x_vals = in_x_data;
        test_obj.args.data.y_vals = in_y_data;
        string test_json = JsonUtility.ToJson(test_obj) + ";";
        Globals.modelInput = test_obj;
        StartCoroutine(GetWaveModelGen(test_json));
        //SaveSignalSystem.SaveSignal(in_x_data, in_y_data, @"D:\test_signal.sig");
    }

    void DrawSampleInputChart()
    {
        if (inputChart)
        {
            for (int i = 0; i < in_x_data.Count; i++)
            {
                AddChartPoint(in_x_data[i], in_y_data[i], inputChart);
            }
            inputChart.SetDirty();
        }
    }

    void DrawSampleOutputChart(FirstShapeRes response)
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
        Debug.Log("Added Point ... ");
    }
    void RemoveChartPoints(LineChart chart)
    {
        for (int i = 0; i < chart.GetChartData().DataSets[0].Entries.Count; i++)
        {
            RemoveChartPoint(chart);
        }
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

    #region Async
    string url = "http://ec2-3-15-236-180.us-east-2.compute.amazonaws.com:9999/";

    IEnumerator GetWaveModelGen(string jsonBody)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + Encoding.UTF8.GetString(request.downloadHandler.data));
        string response = Encoding.UTF8.GetString(request.downloadHandler.data);
        FirstShapeRes resObj = new FirstShapeRes();
        resObj = JsonUtility.FromJson<FirstShapeRes>(response);
        Debug.Log("P Mag: " + resObj.star_arr[1].p_mag);
        Debug.Log("P Angle: " + resObj.star_arr[1].p_angel);
        Globals.modelOutput = resObj;

        // Draw input and response 
        DrawSampleOutputChart(resObj);
        double errTerm = 0;
        for (int i = 0; i < resObj.model_td_y.Count; i++)
        {
            errTerm += Mathf.Pow((float)(resObj.model_td_y[i] - in_y_data[i]), 2);
        }
        if (errTxt)
        {
            errTxt.text = "Error: " + errTerm.ToString();
        }
    }
    #endregion
}
