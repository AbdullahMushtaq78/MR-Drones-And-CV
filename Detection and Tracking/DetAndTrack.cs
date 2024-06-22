using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lays = Unity.Sentis.Layers;
using System.IO;
using Unity.Sentis;
using UnityEngine.Video;
using FF = Unity.Sentis.Functional;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class DetAndTrack : MonoBehaviour
{
    [Header("Controls")]
    #region Cotrols
    public bool RunModel = false;
    public bool limitFrameRate = false;
    
    #if UNITY_EDITOR
        [SerializeField]
        [ShowInInspectorIf("limitFrameRate", true)]
        public int framesLimit = 60;
    #endif
    
    #endregion

    [Header("Model")]
    #region Model
    [SerializeField]
    ModelAsset modelAsset;
    IWorker engine;
    const BackendType backend = BackendType.GPUCompute;
    private const int numClasses = 80;
    [SerializeField, Range(0, 1)] float iouThreshold = 0.5f;
    [SerializeField, Range(0, 1)] float scoreThreshold = 0.5f;
    public int maxOutputBoxes = 64;
    TensorFloat centersToCorners;
    #endregion

    [Header("Camera And Display")]
    #region Camera And Display
    [SerializeField]
    Camera m_Camera;
    [SerializeField]
    RawImage displayImage;
    public RawImage frameBackground;
    public RenderTexture targetRT;
    private Transform displayLocation;
    public int captureWidth = 3840;
    public int captureHeight = 2160;
    public int imageWidth = 640;
    public int imageHeight = 640;
    #endregion

    [Header("Bounding Boxes")]
    #region Bounding Boxes
    public Sprite borderSprite;
    public Texture2D borderTexture;
    public TextAsset labelsAsset;
    public Font font;
    private string[] labels;
    Color[] colors = {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.white,
        Color.black,
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0.5f, 0f, 0.5f), // Purple
        new Color(0f, 0.5f, 0.5f) // Teal
        
    };
    public struct BoundingBox
    {
        public float centerX;
        public float centerY;
        public float width;
        public float height;
        public string label;
    }

    List<GameObject> boxPool = new List<GameObject>();
    #endregion


    private void Awake()
    {
        displayImage.gameObject.SetActive(RunModel);
        frameBackground.gameObject.SetActive(RunModel);
    }

    void Start()
    {
        if (limitFrameRate)
            Application.targetFrameRate = framesLimit;
        
        labels = labelsAsset.text.Split('\n');
        LoadModel();

        targetRT = new RenderTexture(imageWidth, imageHeight, 0);
        displayLocation = displayImage.transform;

        if (borderSprite == null)
        {
            borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(borderTexture.width / 2, borderTexture.height / 2));
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) || OVRInput.Get(OVRInput.Button.Four, OVRInput.Controller.LTouch))
        {
            RunModel = !RunModel;
            displayImage.gameObject.SetActive(RunModel);
            frameBackground.gameObject.SetActive(RunModel);
        }
        if (RunModel)
            ExecuteModel();
    }
    void LoadModel()
    {
        var model1 = ModelLoader.Load(modelAsset);
        centersToCorners = new TensorFloat(new TensorShape(4, 4),
        new float[]
        {
                    1,      0,      1,      0,
                    0,      1,      0,      1,
                    -0.5f,  0,      0.5f,   0,
                    0,      -0.5f,  0,      0.5f
        });
        //Here we transform the output of the model1 by feeding it through a Non-Max-Suppression layer.
        var model2 = Functional.Compile(
               input =>
               {
                   var modelOutput = model1.Forward(input)[0];
                   var boxCoords = modelOutput[0, 0..4, ..].Transpose(0, 1);        //shape=(8400,4)
                   var allScores = modelOutput[0, 4.., ..];                         //shape=(80,8400)
                   var scores = FF.ReduceMax(allScores, 0) - scoreThreshold;        //shape=(8400)
                   var classIDs = FF.ArgMax(allScores, 0);                          //shape=(8400) 
                   var boxCorners = FF.MatMul(boxCoords, FunctionalTensor.FromTensor(centersToCorners));
                   var indices = FF.NMS(boxCorners, scores, iouThreshold);           //shape=(N)
                   var indices2 = indices.Unsqueeze(-1).BroadcastTo(new int[] { 4 });//shape=(N,4)
                   var coords = FF.Gather(boxCoords, 0, indices2);                  //shape=(N,4)
                   var labelIDs = FF.Gather(classIDs, 0, indices);                  //shape=(N)
                   return (coords, labelIDs);
               },
               InputDef.FromModel(model1)[0]
         );
        engine = WorkerFactory.CreateWorker(backend, model2);
    }


    public Texture2D TakeScreenshot()
    {
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        m_Camera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        m_Camera.Render();

        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenshot.Apply();
        m_Camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Texture2D resizedScreenshot = ResizeTexture(screenshot, imageWidth, imageHeight);
        return resizedScreenshot;
    }

    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return newTexture;
    }

    
    public void ExecuteModel()
    {
        ClearAnnotations();
        if(m_Camera && targetRT)
        {
            m_Camera.targetTexture = targetRT;
            displayImage.texture = targetRT;
        }
        using var input = TextureConverter.ToTensor(targetRT, imageWidth, imageHeight, 3);
        engine.Execute(input);

        var output = engine.PeekOutput("output_0") as TensorFloat;
        var labelIDs = engine.PeekOutput("output_1") as TensorInt;

        output.CompleteOperationsAndDownload();
        labelIDs.CompleteOperationsAndDownload();

        float displayWidth = displayImage.rectTransform.rect.width;
        float displayHeight = displayImage.rectTransform.rect.height;

        float scaleX = displayWidth / imageWidth;
        float scaleY = displayHeight / imageHeight;

        int boxesFound = output.shape[0];
        for (int n = 0; n < Mathf.Min(boxesFound, maxOutputBoxes); n++)
        {
            var box = new BoundingBox
            {
                centerX = output[n, 0] * scaleX - displayWidth / 2,
                centerY = output[n, 1] * scaleY - displayHeight / 2,
                width = output[n, 2] * scaleX,
                height = output[n, 3] * scaleY,
                label = labels[labelIDs[n]],
            };
            DrawBox(box, n, displayHeight * 0.05f);
        }
    }

    public void DrawBox(BoundingBox box, int id, float fontSize)
    {
        GameObject panel;
        if (id < boxPool.Count)
        {
            panel = boxPool[id];
            panel.SetActive(true);
        }
        else
        {
            //panel = CreateNewBox(Color.yellow);
            panel = CreateNewBox(colors[id%colors.Length]);
        }
        //Set box position
        panel.transform.localPosition = new Vector3(box.centerX, -box.centerY);

        //Set box size
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(box.width, box.height);

        //Set label text
        var label = panel.GetComponentInChildren<Text>();
        label.text = box.label;
        label.fontSize = (int)fontSize;
    }

    public GameObject CreateNewBox(Color color)
    {
        //Create the box and set image

        var panel = new GameObject("ObjectBox");
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color;
        img.sprite = borderSprite;
        img.type = Image.Type.Sliced;
        panel.transform.SetParent(displayLocation, false);

        //Create the label

        var text = new GameObject("ObjectLabel");
        text.AddComponent<CanvasRenderer>();
        text.transform.SetParent(panel.transform, false);
        Text txt = text.AddComponent<Text>();
        txt.font = font;
        txt.color = color;
        txt.fontSize = 60;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform rt2 = text.GetComponent<RectTransform>();
        rt2.offsetMin = new Vector2(20, rt2.offsetMin.y);
        rt2.offsetMax = new Vector2(0, rt2.offsetMax.y);
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
        rt2.offsetMax = new Vector2(rt2.offsetMax.x, 30);
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);

        boxPool.Add(panel);
        return panel;
    }

    public void ClearAnnotations()
    {
        foreach (var box in boxPool)
        {
            box.SetActive(false);
        }
    }

    void OnDisable()
    {
        engine.Dispose();
        centersToCorners?.Dispose();
    }
}













#if UNITY_EDITOR
public class ShowInInspectorIfAttribute : PropertyAttribute
{
    public string ConditionName { get; private set; }
    public bool ConditionValue { get; private set; }

    public ShowInInspectorIfAttribute(string conditionName, bool conditionValue)
    {
        ConditionName = conditionName;
        ConditionValue = conditionValue;
    }
}

[CustomPropertyDrawer(typeof(ShowInInspectorIfAttribute))]
public class ShowInInspectorIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowInInspectorIfAttribute condition = attribute as ShowInInspectorIfAttribute;

        SerializedProperty conditionProperty = property.serializedObject.FindProperty(condition.ConditionName);
        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            bool showProperty = conditionProperty.boolValue == condition.ConditionValue;
            if (!showProperty)
                return;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowInInspectorIfAttribute condition = attribute as ShowInInspectorIfAttribute;

        SerializedProperty conditionProperty = property.serializedObject.FindProperty(condition.ConditionName);
        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
        {
            bool showProperty = conditionProperty.boolValue == condition.ConditionValue;
            if (!showProperty)
                return 0f;
        }

        return EditorGUI.GetPropertyHeight(property, label);
    }
}
#endif
