using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;
using SimpleFileBrowser;
using TMPro;
using Proyecto26;
using UnityEngine.Networking;
using System;
using System.Net;
using System.IO;
using System.Text;
using AwesomeCharts;
using UnityEngine.Android;

public class Globals
{
    public static string url = "http://ec2-3-15-236-180.us-east-2.compute.amazonaws.com:9999/";
    public static JsonInput modelInput;
    public static FirstShapeRes modelOutput;
    
}

public class HelperClass
{
    public static void HideWindow(GameObject window)
    {
        if (window)
        {
            window.SetActive(false);
        }
    }
    public static void ShowWindow(GameObject window)
    {
        if (window)
        {
            window.SetActive(true);
        }
    }
}

public class AppMan : MonoBehaviour
{
    #region UI
    public GameObject importSignalWindow;
    public GameObject importModelWindow;
    public GameObject archivedWindow;
    public GameObject exportWindow;
    public GameObject sampleModelWindow;
    public GameObject generatedModelWindow;

    public LineChart signalChart;

    public Slider filterRatioSlider;

    public Button importSignalBtn;
    public Button importModelBtn;
    public Button proceedToGenModelBtn;
    public Button filterBtn;

    public CustomDropdown mainDD;
    public TextMeshProUGUI mainDDTxt;
    public TextMeshProUGUI filterRatioTxt;

    void FileBrowserInits()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        FileBrowser.RequestPermission();
        //Permission.RequestUserPermission(Permission.ExternalStorageRead);
        //Permission.RequestUserPermission(Permission.ExternalStorageWrite);
#endif
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Signals", ".sig"), new FileBrowser.Filter("Models", ".ema"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".txt");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Show a save file dialog 
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Allow multiple selection: false
        // Initial path: "C:\", Title: "Save As", submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, false, false, "C:\\", "Save As", "Save" );

        // Show a select folder dialog 
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Allow multiple selection: false
        // Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
        // FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
        //                            () => { Debug.Log( "Canceled" ); },
        //                            true, false, null, "Select Folder", "Select" );

        // Coroutine example
        //StartCoroutine(ShowLoadDialogCoroutine());
    }
    IEnumerator ShowLoadSignalDialogCoroutine()
    {
        FileBrowser.CheckPermission();
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, true, null, "Load Signal", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            // Clean chart
            try
            {
                RemoveFirstSigChartPoints(signalChart);
                RemoveSecondSigChartPoints(signalChart);
            }
            catch
            {
                Debug.Log("First time ... ");
            }
//#if UNITY_ANDROID && !UNITY_EDITOR
//            string path = FileBrowser.Result[0].Replace("%2F", "/").Replace("%3A", ":");
//            path = path.Split(':')[2];
//            Debug.Log("File Path: " + path);
//            // Read the bytes of the first file via FileBrowserHelpers
//            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
//            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
//            SignalData loadedData = SaveSignalSystem.LoadSignal(path);
//#endif
#if UNITY_EDITOR
            //// Read the bytes of the first file via FileBrowserHelpers
            //// Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            ////byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            //SignalData loadedData = SaveSignalSystem.LoadSignal(FileBrowser.Result[0]);
#endif
            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            SignalData loadedData = SaveSignalSystem.LoadSignal(FileBrowser.Result[0]);
            Debug.Log("Sample Point: " + loadedData.x_val[0]);
            Debug.Log("Sample Point: " + loadedData.x_val[1]);
            Debug.Log("Sample Point: " + loadedData.x_val[2]);
            DrawSignal(loadedData.x_val, loadedData.y_val, signalChart, "Original");
            // Assign filter btn 
            if (filterBtn)
            {
                filterBtn.onClick.AddListener(delegate
                {
                    FilterSignalBtnCallback(loadedData);
                });
            }
            // Assign proceed to model btn
            if (proceedToGenModelBtn)
            {
                proceedToGenModelBtn.interactable = true;
                proceedToGenModelBtn.onClick.AddListener(delegate
                {
                    Debug.Log("Redirecting to Model Gen Window ... ");
                    HideAllWindows();
                    //HelperClass.ShowWindow();
                    HelperClass.ShowWindow(generatedModelWindow);
                });
            }
        }
    }
    IEnumerator FilterSignal(SignalData loadedSignal, double filter_ratio)
    {
        var request = new UnityWebRequest(url, "POST");
        JsonInput reqBody = new JsonInput();
        reqBody.api_func = "dyn_filter";
        reqBody.args = new Args();
        reqBody.args.data = new Data();
        reqBody.args.settings = new Settings();
        reqBody.args.data.x_vals = loadedSignal.x_val;
        reqBody.args.data.y_vals = loadedSignal.y_val;
        reqBody.args.settings.filter_ratio = filter_ratio;
        string jsonBody = JsonUtility.ToJson(reqBody) + ";";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Response: " + Encoding.UTF8.GetString(request.downloadHandler.data));
        string response = Encoding.UTF8.GetString(request.downloadHandler.data);
        SecondShapeRes resObj = new SecondShapeRes();
        resObj = JsonUtility.FromJson<SecondShapeRes>(response);
        Debug.Log("Request of Filteration: " + jsonBody);
        Debug.Log("Response of Filteration: " + response);
        // Plot response 
        DrawSignal(resObj.model_td_x, resObj.model_td_y, signalChart, "Filtered");
        // Assign the filtered data as input data to the generated model
        JsonInput modelGenInData = new JsonInput();
        modelGenInData.api_func = "model_gen";
        modelGenInData.args = new Args();
        modelGenInData.args.settings = new Settings();
        modelGenInData.args.data = new Data();
        modelGenInData.args.data.x_vals = loadedSignal.x_val;
        modelGenInData.args.data.y_vals = loadedSignal.y_val;
        modelGenInData.args.settings.filter_ratio = filter_ratio;
        Globals.modelInput = modelGenInData;
    }
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
        }
    }
    IEnumerator ShowSaveDialogCoroutine(JsonInput input, FirstShapeRes response)
    {
        FileBrowser.CheckPermission();

        // Save file 
        yield return FileBrowser.WaitForSaveDialog(false, false, null, "Save File", "Save");

        // Dialog is closed 
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            Debug.Log("Result: " + FileBrowser.Result[0]);
            if (input != null && response != null)
            {
                SaveModelSystem.SaveModel(input, response, FileBrowser.Result[0]);
            }
        }
    }
    void HideAllWindows()
    {
        HelperClass.HideWindow(importSignalWindow);
        HelperClass.HideWindow(archivedWindow);
        HelperClass.HideWindow(importModelWindow);
        HelperClass.HideWindow(exportWindow);
        HelperClass.HideWindow(sampleModelWindow);
        HelperClass.HideWindow(generatedModelWindow);
        if (FileBrowser.IsOpen)
        {
            FileBrowser.HideDialog();
        }
        if (proceedToGenModelBtn)
        {
            proceedToGenModelBtn.interactable = false;
        }
        // Clear 3D objects (if available)
        Remove3DObjects();
    }
    public void MainDDCallback()
    {
        if (mainDD)
        {
            if (mainDD.selectedItemIndex == 0)
            {
                Debug.Log("Simulation Mode Selected ... ");
                HideAllWindows();
                // Here add stars and 3D objects 
                RunSimBtnCallback();
            }
            else if (mainDD.selectedItemIndex == 1)
            {
                Debug.Log("Archived Mode Selected ... ");
                HideAllWindows();
                HelperClass.ShowWindow(archivedWindow);
            }
            else if (mainDD.selectedItemIndex == 2)
            {
                Debug.Log("Import Signal Mode Selected ... ");
                HideAllWindows();
                HelperClass.ShowWindow(importSignalWindow);
                try
                {
                    RemoveFirstSigChartPoints(signalChart);
                    RemoveSecondSigChartPoints(signalChart);
                }
                catch
                {
                    Debug.Log("First time ... ");
                }
            }
            else if (mainDD.selectedItemIndex == 3)
            {
                Debug.Log("Import Model Mode Selected ... ");
                HideAllWindows();
                HelperClass.ShowWindow(importModelWindow);
            }
            else if (mainDD.selectedItemIndex == 4)
            {
                Debug.Log("Sample Model Mode Selected ... ");
                HideAllWindows();
                HelperClass.ShowWindow(sampleModelWindow);
            }
            else if (mainDD.selectedItemIndex == 5)
            {
                Debug.Log("Export Mode Selected ... ");
                HideAllWindows();
                HelperClass.ShowWindow(exportWindow);
                StartCoroutine(ShowSaveDialogCoroutine(Globals.modelInput, Globals.modelOutput));
            }
        }
    }
    public void CloseImportSignalWindowCallback()
    {
        HelperClass.HideWindow(importSignalWindow);
    }
    public void CloseImportModelWindowCallback()
    {
        HelperClass.HideWindow(importModelWindow);
    }
    public void CloseArchivedWindowCallback()
    {
        HelperClass.HideWindow(archivedWindow);
    }
    public void CloseExportWindowCallback()
    {
        HelperClass.HideWindow(exportWindow);
    }
    public void CloseSampleModelWindowCallback()
    {
        HelperClass.HideWindow(sampleModelWindow);
    }
    public void RunSimBtnCallback()
    {
        Debug.Log("Simulation Mode Selected ... ");
        HideAllWindows();
        // Here add stars and 3D objects 
        Remove3DObjects();
        Init3DUI();
    }
    public void ImportModelBtnCallback()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    public void ImportSignalBtnCallback()
    {
        StartCoroutine(ShowLoadSignalDialogCoroutine());
    }
    public void FilterSignalBtnCallback(SignalData signalData)
    {
        if (filterRatioSlider)
        {
            StartCoroutine(FilterSignal(signalData, (double)filterRatioSlider.value));
        }
    }
    public void FilterSliderOnValueChanged()
    {
        if (filterRatioSlider)
        {
            if (filterRatioTxt)
            {
                filterRatioTxt.text = filterRatioSlider.value.ToString();
            }
        }
    }
#endregion

#region 3DSimulation
    public List<GameObject> starPrefabs;
    public List<GameObject> starsList;

    public float scaleFactor;
    public float rotationFactor;

    void Remove3DObjects()
    {
        for (int i = 0; i < starsList.Count; i++)
        {
            Destroy(starsList[i].gameObject);
        }
        starsList.Clear();
    }
    void Init3DUI()
    {
        if (Globals.modelOutput != null)
        {
            //for (int i = 0; i < Globals.modelOutput.star_arr.Count; i++)
            //{
            //    float x = (float)(Globals.modelOutput.star_arr[i].p_mag * Mathf.Cos((float)Globals.modelOutput.star_arr[i].p_angel));
            //    float y = (float)(Globals.modelOutput.star_arr[i].p_mag * Mathf.Sin((float)Globals.modelOutput.star_arr[i].p_angel));
            //    Vector3 starPos = new Vector3(x, y, 0) * scaleFactor;
            //    GameObject star = Instantiate(starPrefabs[UnityEngine.Random.Range(0, 4)], starPos, Quaternion.identity);

            //    starsList.Add(star);
            //}

            foreach (Star s in Globals.modelOutput.star_arr)
            {
                float x = (float)(s.p_mag * Mathf.Cos((float)s.p_angel));
                float y = (float)(s.p_mag * Mathf.Sin((float)s.p_angel));
                Vector3 starPos = new Vector3(x, y, 0) * scaleFactor;
                GameObject star = Instantiate(starPrefabs[UnityEngine.Random.Range(0, 4)], starPos, Quaternion.identity);

                starsList.Add(star);
            }

            foreach (Star s in Globals.modelOutput.star_arr)
            {
                Debug.Log("R: " + s.p_mag + ",  Theta: " + s.p_angel + ",  Omega: " + s.w);
            }
        }
        else
        {
            Debug.Log("No data to simulate ... ");
        }
    }
    void HandleRotation()
    {
        for (int i = 0; i < starsList.Count; i++)
        {
            if (i != 0)
            {
                starsList[i].transform.RotateAround(starsList[i - 1].transform.position, Vector3.forward, (float)Globals.modelOutput.star_arr[i].w*rotationFactor*Time.deltaTime);
            }
        }
    }
#endregion

#region UnityEvents
    void Start()
    {
        FileBrowserInits();
    }
    void Update()
    {
        HandleRotation();
    }
    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "WaveVisualization")
        {
            if (importSignalBtn)
            {
                importSignalBtn.onClick.AddListener(delegate
                {
                    StartCoroutine(ShowLoadDialogCoroutine());
                });
            }
            if (importModelBtn)
            {
                importModelBtn.onClick.AddListener(delegate
                {
                    StartCoroutine(ShowLoadDialogCoroutine());
                });
            }
            MainDDCallback();
            if (mainDD)
            {
                mainDD.dropdownEvent.AddListener(delegate
                {
                    MainDDCallback();
                });
            }
            if (filterRatioSlider)
            {
                filterRatioTxt.text = filterRatioSlider.value.ToString();
                filterRatioSlider.onValueChanged.AddListener(delegate
                {
                    filterRatioTxt.text = filterRatioSlider.value.ToString(); 
                });
            }
            filterRatioSlider.onValueChanged.Invoke(0.5f);
        }
    }
#endregion

#region APIComm
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

        //WWW www;
        //Hashtable postHeader = new Hashtable();
        //postHeader.Add("Content-Type", "application/json");

        //// convert json string to byte
        //var formData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        //www = new WWW(url, formData, postHeader);
        //yield return www.text;
        //Debug.Log(www.text);
    }

    IEnumerator GetWaveDynFilt(string jsonBody)
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
        SecondShapeRes resObj = new SecondShapeRes();
        resObj = JsonUtility.FromJson<SecondShapeRes>(response);
        Debug.Log("X Values: " + resObj.model_td_x);
        //WWW www;
        //Hashtable postHeader = new Hashtable();
        //postHeader.Add("Content-Type", "application/json");

        //// convert json string to byte
        //var formData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        //www = new WWW(url, formData, postHeader);
        //yield return www.text;
        //Debug.Log(www.text);
    }

    void SampleAPI()
    {
        //string test_json = "{\"api_func\": \"model_gen\",\"args\": {\"settings\": {\"filter_ratio\": 0.5},\"data\": {\"x_vals\": [0.0, 0.010101010101010102, 0.020202020202020204, 0.030303030303030304, 0.04040404040404041, 0.05050505050505051, 0.06060606060606061, 0.07070707070707072, 0.08080808080808081, 0.09090909090909091, 0.10101010101010102, 0.11111111111111112, 0.12121212121212122, 0.13131313131313133, 0.14141414141414144, 0.15151515151515152, 0.16161616161616163, 0.17171717171717174, 0.18181818181818182, 0.19191919191919193, 0.20202020202020204, 0.21212121212121213, 0.22222222222222224, 0.23232323232323235, 0.24242424242424243, 0.25252525252525254, 0.26262626262626265, 0.27272727272727276, 0.2828282828282829, 0.29292929292929293, 0.30303030303030304, 0.31313131313131315, 0.32323232323232326, 0.33333333333333337, 0.3434343434343435, 0.3535353535353536, 0.36363636363636365, 0.37373737373737376, 0.38383838383838387, 0.393939393939394, 0.4040404040404041, 0.4141414141414142, 0.42424242424242425, 0.43434343434343436, 0.4444444444444445, 0.4545454545454546, 0.4646464646464647, 0.4747474747474748, 0.48484848484848486, 0.494949494949495, 0.5050505050505051, 0.5151515151515152, 0.5252525252525253, 0.5353535353535354, 0.5454545454545455, 0.5555555555555556, 0.5656565656565657, 0.5757575757575758, 0.5858585858585859, 0.595959595959596, 0.6060606060606061, 0.6161616161616162, 0.6262626262626263, 0.6363636363636365, 0.6464646464646465, 0.6565656565656566, 0.6666666666666667, 0.6767676767676768, 0.686868686868687, 0.696969696969697, 0.7070707070707072, 0.7171717171717172, 0.7272727272727273, 0.7373737373737375, 0.7474747474747475, 0.7575757575757577, 0.7676767676767677, 0.7777777777777778, 0.787878787878788, 0.797979797979798, 0.8080808080808082, 0.8181818181818182, 0.8282828282828284, 0.8383838383838385, 0.8484848484848485, 0.8585858585858587, 0.8686868686868687, 0.8787878787878789, 0.888888888888889, 0.8989898989898991, 0.9090909090909092, 0.9191919191919192, 0.9292929292929294, 0.9393939393939394, 0.9494949494949496, 0.9595959595959597, 0.9696969696969697, 0.9797979797979799, 0.98989898989899, 1.0],\"y_vals\": [0.0, 1.0183261611006384, 0.6936523174365201, -0.4289077418601951, -0.6829998730840275, 0.37545736535505203, 1.3434740239838692, 0.9434723036625774, -0.18057226441582258, -0.3689911778989208, 0.7195003826283906, 1.6275953626987474, 1.148305533209522, 0.02189753727866861, -0.10530689915582436, 1.0038271964107461, 1.8435638944107684, 1.2823838988541945, 0.15388242100025973, 0.08442243031559193, 1.2060502286251547, 1.9706789075065496, 1.3268278963378775, 0.19807662393042091, 0.18429138713267257, 1.3119075733723615, 1.9967289036258173, 1.2715539987223619, 0.14623259158000557, 0.18769569426818422, 1.3166632743749966, 1.9192090700565234, 1.1163043070778782, -5.551115123125783e-16, 0.09797814597724008, 1.2255567526200086, 1.745571016235191, 0.8706955673522094, -0.22922477377504347, -0.07192002526150632, 1.053256599963242, 1.4924798377881838, 0.5532825650315922, -0.5214237586979691, -0.30076746636087187, 0.8223733742970281, 1.1841526913464815, 0.1897293294714113, -0.8499447754104859, -0.5611799955565699, 0.5611799955565715, 0.8499447754104827, -0.18972932947142837, -1.1841526913464824, -0.8223733742970264, 0.30076746636087337, 0.5214237586979649, -0.5532825650316017, -1.492479837788184, -1.0532565999632406, 0.07192002526150765, 0.22922477377504236, -0.8706955673522189, -1.7455710162351927, -1.2255567526200066, -0.09797814597723864, -8.881784197001252e-16, -1.1163043070778804, -1.9192090700565245, -1.3166632743749878, -0.18769569426817834, -0.1462325915800069, -1.2715539987223639, -1.9967289036258173, -1.311907573372356, -0.18429138713266713, -0.19807662393042014, -1.3268278963378761, -1.9706789075065494, -1.206050228625149, -0.0844224303155886, -0.15388242100026572, -1.282383898854206, -1.8435638944107686, -1.0038271964107441, 0.10530689915582747, -0.02189753727867505, -1.1483055332095298, -1.6275953626987474, -0.7195003826283757, 0.3689911778989218, 0.18057226441581947, -0.9434723036625834, -1.3434740239838667, -0.3754573653550392, 0.6829998730840272, 0.42890774186019287, -0.6936523174365236, -1.0183261611006365, -5.143516556418883e-15]}}}";
        JsonInput test_obj = new JsonInput();
        test_obj.api_func = "model_gen";
        test_obj.args = new Args();
        test_obj.args.settings = new Settings();
        test_obj.args.settings.filter_ratio = 0.5;
        test_obj.args.data = new Data();
        test_obj.args.data.x_vals = new List<double>() { 0.0, 0.010101010101010102, 0.020202020202020204, 0.030303030303030304, 0.04040404040404041, 0.05050505050505051, 0.06060606060606061, 0.07070707070707072, 0.08080808080808081, 0.09090909090909091, 0.10101010101010102, 0.11111111111111112, 0.12121212121212122, 0.13131313131313133, 0.14141414141414144, 0.15151515151515152, 0.16161616161616163, 0.17171717171717174, 0.18181818181818182, 0.19191919191919193, 0.20202020202020204, 0.21212121212121213, 0.22222222222222224, 0.23232323232323235, 0.24242424242424243, 0.25252525252525254, 0.26262626262626265, 0.27272727272727276, 0.2828282828282829, 0.29292929292929293, 0.30303030303030304, 0.31313131313131315, 0.32323232323232326, 0.33333333333333337, 0.3434343434343435, 0.3535353535353536, 0.36363636363636365, 0.37373737373737376, 0.38383838383838387, 0.393939393939394, 0.4040404040404041, 0.4141414141414142, 0.42424242424242425, 0.43434343434343436, 0.4444444444444445, 0.4545454545454546, 0.4646464646464647, 0.4747474747474748, 0.48484848484848486, 0.494949494949495, 0.5050505050505051, 0.5151515151515152, 0.5252525252525253, 0.5353535353535354, 0.5454545454545455, 0.5555555555555556, 0.5656565656565657, 0.5757575757575758, 0.5858585858585859, 0.595959595959596, 0.6060606060606061, 0.6161616161616162, 0.6262626262626263, 0.6363636363636365, 0.6464646464646465, 0.6565656565656566, 0.6666666666666667, 0.6767676767676768, 0.686868686868687, 0.696969696969697, 0.7070707070707072, 0.7171717171717172, 0.7272727272727273, 0.7373737373737375, 0.7474747474747475, 0.7575757575757577, 0.7676767676767677, 0.7777777777777778, 0.787878787878788, 0.797979797979798, 0.8080808080808082, 0.8181818181818182, 0.8282828282828284, 0.8383838383838385, 0.8484848484848485, 0.8585858585858587, 0.8686868686868687, 0.8787878787878789, 0.888888888888889, 0.8989898989898991, 0.9090909090909092, 0.9191919191919192, 0.9292929292929294, 0.9393939393939394, 0.9494949494949496, 0.9595959595959597, 0.9696969696969697, 0.9797979797979799, 0.98989898989899, 1.0};
        test_obj.args.data.y_vals = new List<double>() { 0.0, 1.0183261611006384, 0.6936523174365201, -0.4289077418601951, -0.6829998730840275, 0.37545736535505203, 1.3434740239838692, 0.9434723036625774, -0.18057226441582258, -0.3689911778989208, 0.7195003826283906, 1.6275953626987474, 1.148305533209522, 0.02189753727866861, -0.10530689915582436, 1.0038271964107461, 1.8435638944107684, 1.2823838988541945, 0.15388242100025973, 0.08442243031559193, 1.2060502286251547, 1.9706789075065496, 1.3268278963378775, 0.19807662393042091, 0.18429138713267257, 1.3119075733723615, 1.9967289036258173, 1.2715539987223619, 0.14623259158000557, 0.18769569426818422, 1.3166632743749966, 1.9192090700565234, 1.1163043070778782, -5.551115123125783e-16, 0.09797814597724008, 1.2255567526200086, 1.745571016235191, 0.8706955673522094, -0.22922477377504347, -0.07192002526150632, 1.053256599963242, 1.4924798377881838, 0.5532825650315922, -0.5214237586979691, -0.30076746636087187, 0.8223733742970281, 1.1841526913464815, 0.1897293294714113, -0.8499447754104859, -0.5611799955565699, 0.5611799955565715, 0.8499447754104827, -0.18972932947142837, -1.1841526913464824, -0.8223733742970264, 0.30076746636087337, 0.5214237586979649, -0.5532825650316017, -1.492479837788184, -1.0532565999632406, 0.07192002526150765, 0.22922477377504236, -0.8706955673522189, -1.7455710162351927, -1.2255567526200066, -0.09797814597723864, -8.881784197001252e-16, -1.1163043070778804, -1.9192090700565245, -1.3166632743749878, -0.18769569426817834, -0.1462325915800069, -1.2715539987223639, -1.9967289036258173, -1.311907573372356, -0.18429138713266713, -0.19807662393042014, -1.3268278963378761, -1.9706789075065494, -1.206050228625149, -0.0844224303155886, -0.15388242100026572, -1.282383898854206, -1.8435638944107686, -1.0038271964107441, 0.10530689915582747, -0.02189753727867505, -1.1483055332095298, -1.6275953626987474, -0.7195003826283757, 0.3689911778989218, 0.18057226441581947, -0.9434723036625834, -1.3434740239838667, -0.3754573653550392, 0.6829998730840272, 0.42890774186019287, -0.6936523174365236, -1.0183261611006365, -5.143516556418883e-15 };
        string test_json = JsonUtility.ToJson(test_obj) + ";";
        Debug.Log(test_json);
        if (test_obj.api_func == "model_gen")
        {
            StartCoroutine(GetWaveModelGen(test_json));
        }
        else if (test_obj.api_func == "dyn_filter")
        {
            StartCoroutine(GetWaveDynFilt(test_json));
        }
    }
#endregion

#region LoadSignal
    void DrawSignal(List<double> x_val, List<double> y_val, LineChart chart, string sig_type)
    {
        if (chart != null)
        {
            if (sig_type == "Original")
            {
                for (int i = 0; i < x_val.Count; i++)
                {
                    AddFirstChartPoint(x_val[i], y_val[i], chart);
                }
            }
            else
            {
                for (int i = 0; i < x_val.Count; i++)
                {
                    AddSecondChartPoint(x_val[i], y_val[i], chart);
                }
            }
            chart.SetDirty();
        }
    }
    void AddFirstChartPoint(double x, double y, LineChart chart)
    {
        chart.GetChartData().DataSets[0].AddEntry(new LineEntry((float)x, (float)y));
    }
    void AddSecondChartPoint(double x, double y, LineChart chart)
    {
        chart.GetChartData().DataSets[1].AddEntry(new LineEntry((float)x, (float)y));
    }
    void RemoveFirstSigChartPoints(LineChart chart)
    {
        chart.GetChartData().DataSets[0].Clear();
        chart.SetDirty();
    }
    void RemoveSecondSigChartPoints(LineChart chart)
    {
        chart.GetChartData().DataSets[1].Clear();
        chart.SetDirty();
    }
#endregion
}

#region RequestStructure
[Serializable]
public class JsonInput
{
    public string api_func;
    public Args args;
}
[Serializable]
public class Args
{
    public Settings settings;
    public Data data;
}
[Serializable]
public class Settings
{
    public double filter_ratio;
}
[Serializable]
public class Data
{
    public List<double> x_vals;
    public List<double> y_vals;
}
#endregion

#region ResponseStructure
[Serializable]
public class FirstShapeRes
{
    public List<Star> star_arr;
    public List<double> model_td_x;
    public List<double> model_td_y;
}
[Serializable]
public class Star
{
    public double p_mag;
    public double p_angel;
    public double w;
}
[Serializable]
public class SecondShapeRes
{
    public List<double> model_td_x;
    public List<double> model_td_y;
}
#endregion
