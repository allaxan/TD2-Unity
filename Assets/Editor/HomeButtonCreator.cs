using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class HomeButtonCreator
{
    const string IconPath = "Assets/Icons/Home.png";

    // Supprime les Canvas existants de la scène et recrée un Canvas propre avec le bouton Home.
    [MenuItem("Tools/Create Home Button")]
    static void CreateHomeButton()
    {
        Undo.SetCurrentGroupName("Create Home Button");

        foreach (var old in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))
            if (old != null && old.isRootCanvas)
                Undo.DestroyObjectImmediate(old.gameObject);

        Canvas canvas = CreateCanvas();

        // --- Bouton ---
        var buttonGO = new GameObject("HomeButton", typeof(RectTransform), typeof(Image));
        buttonGO.transform.SetParent(canvas.transform, false);
        var rt = (RectTransform)buttonGO.transform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f); // coin haut-gauche
        rt.anchoredPosition = new Vector2(30f, -30f);
        rt.sizeDelta = new Vector2(220f, 80f);

        var bg = buttonGO.GetComponent<Image>();
        bg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        bg.type = Image.Type.Sliced;
        bg.color = new Color(0.18f, 0.45f, 0.85f);
        bg.raycastTarget = false;

        // --- Icône maison ---
        var iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGO.transform.SetParent(buttonGO.transform, false);
        var iconRT = (RectTransform)iconGO.transform;
        iconRT.anchorMin = iconRT.anchorMax = iconRT.pivot = new Vector2(0f, 0.5f);
        iconRT.anchoredPosition = new Vector2(16f, 0f);
        iconRT.sizeDelta = new Vector2(52f, 52f);
        var iconImg = iconGO.GetComponent<Image>();
        iconImg.sprite = LoadIconAsSprite();
        iconImg.preserveAspect = true;
        iconImg.raycastTarget = false;

        // --- Texte ---
        var textGO = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(buttonGO.transform, false);
        var textRT = (RectTransform)textGO.transform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(80f, 0f);
        textRT.offsetMax = new Vector2(-12f, 0f);
        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = "Home";
        tmp.fontSize = 36;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.raycastTarget = false;

        // --- ButtonController (m_Title / m_Icon) ---
        var controller = buttonGO.AddComponent<ButtonController>();
        var so = new SerializedObject(controller);
        so.FindProperty("m_Title").objectReferenceValue = tmp;
        so.FindProperty("m_Icon").objectReferenceValue = iconImg;
        so.ApplyModifiedPropertiesWithoutUndo();

        int uiLayer = LayerMask.NameToLayer("UI");
        foreach (var t in canvas.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = uiLayer;

        Selection.activeGameObject = buttonGO;
    }

    static Canvas CreateCanvas()
    {
        var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
        Undo.RegisterCreatedObjectUndo(go, "Create Home Button");
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        return canvas;
    }

    static Sprite LoadIconAsSprite()
    {
        var importer = AssetImporter.GetAtPath(IconPath) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(IconPath);
    }
}
