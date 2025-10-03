using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityGLTF;
using UnityGLTF.Loader;
public class ObjectLoader : MonoBehaviour
{
    public ModelsConfig[] models;
    public GameObject basePrefab, Image;
    public ObjectSpawner objectSpawner;
    public TextMeshProUGUI infoText;
    public Button continueBtn, captureBtn, optionsBtn;
    async void Start()
    {
        continueBtn.interactable = false;
        optionsBtn.interactable = false;
        captureBtn.gameObject.SetActive(false);
        Image.SetActive(true);
        objectSpawner.objectPrefabs.Clear();
        infoText.text = "Loading Models...";
        int i = 1;
        foreach (var model in models)
        {
            GameObject modelObj = Instantiate(basePrefab, transform);
            modelObj.name = model.name;

            MaterialPropertyBlockHelper materialPropertyBlockHelper = modelObj.GetComponentInChildren<MaterialPropertyBlockHelper>();
            ColorMaterialPropertyAffordanceReceiver colorMaterialPropertyAffordanceReceiver = modelObj.GetComponentInChildren<ColorMaterialPropertyAffordanceReceiver>();


            infoText.text = "(" + i + "/" + (models.Length) + ")" + " Loading Model: " + model.name + "...";
            GameObject obj = modelObj.GetComponent<CustomARObject>().root;
            var importOpt = new ImportOptions();
            importOpt.DataLoader = new UnityWebRequestLoader(model.modelURL);
            var import = new GLTFSceneImporter(model.name, importOpt);
            await import.LoadSceneAsync();
            infoText.text = "(" + i + "/" + (models.Length) + ")" + " Model Loaded: " + model.name;
            import.CreatedObject.transform.SetParent(obj.transform);

            materialPropertyBlockHelper.rendererTarget = obj.GetComponentInChildren<Renderer>();
            materialPropertyBlockHelper.enabled = true;
            colorMaterialPropertyAffordanceReceiver.enabled = true;
            MeshCollider meshCollider = obj.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            obj.transform.GetChild(0).GetChild(0).rotation = model.rotation;
            obj.transform.GetChild(0).GetChild(0).localScale = model.scale;


            meshCollider.sharedMesh = obj.GetComponentInChildren<MeshFilter>().mesh;
            meshCollider.convex = true;

            obj.transform.localScale = model.rootScale;
            obj.transform.position = model.rootPosition;
            obj.transform.rotation = model.rootRotation;
            objectSpawner.objectPrefabs.Add(modelObj);
            modelObj.SetActive(false);
            i++;
        }
        continueBtn.interactable = true;
        optionsBtn.interactable = true;
        captureBtn.gameObject.SetActive(true);
        Image.SetActive(false);
        infoText.text = "Models Loaded";
        StartCoroutine(HideInfoText());
    }
    public IEnumerator HideInfoText()
    {
        yield return new WaitForSeconds(3f);
        infoText.gameObject.SetActive(false);
    }
    public void TakeScreenshot()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }
    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);
        new NativeShare().AddFile(filePath)
            .SetSubject("AR Screenshot").SetText("").SetUrl("")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

       
    }
}

[System.Serializable]
public struct ModelsConfig
{
    public string name;
    public string modelURL;

    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public Vector3 rootScale;
    public Vector3 rootPosition;
    public Quaternion rootRotation;
}