using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NewPlayerTypes;

public class CharacterSwitcherMenu : MonoBehaviour
{
    public static bool HasSelectedCharacter { get; private set; }
    public static byte LocalPlayerType { get; private set; }
    public static byte ReceivedPlayerType { get; set; }
    public static HoardHandler[] Hoards = new HoardHandler[2];
    
    private readonly DefaultControls.Resources _resources = new();

    private readonly TMP_FontAsset _antonFont = FindObjectOfType<OptionsButton>()
        .GetComponentInChildren<TextMeshProUGUI>()
        .font;
    
    private readonly Color _buttonFontColor = new Color32(255, 113, 71, 255);
    private readonly ColorBlock _buttonColors = new()
    {
        normalColor = Color.white,
        highlightedColor = Color.black,
        pressedColor = Color.white,
        disabledColor = Color.white,
        colorMultiplier = 1,
        fadeDuration = 0.1f
    };

    private const string BoltDescription =
        "<u>The Bolt</u><#025839>\n\n\n\n\n\n" +
        "+ Quick and nimble.\n\n\n<#8b0000>" +
        "- Unable to pick up most weapons nor use melee weapons.";
    private const string PlayerDescription =
        "<u>The Player</u><#025839>\n\n\n\n\n\n" +
        "+ Your average reliable Joe, well-rounded in all fields.\n\n\n" +
        "+ Able to use all weapons where others cannot.";
    private const string ZombieDescription =
        "<u>The Zombie</u><#025839>\n\n\n\n\n\n" +
        "+ Long and slender arms provide superior range.\n\n\n" + 
        "+ Pressing the Tab key (changeable in config) allows you to grab an object; release by jumping.\n\n\n<#8b0000>" +
        "- Unable to pick up most weapons nor use melee weapons.";

    private void Start()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        var canvasTransform = gameObject.transform;
        gameObject.AddComponent<HorizontalLayoutGroup>();
        gameObject.AddComponent<CanvasGroup>();
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    
        CreateButton(BoltDescription, () => SetCharacterType(1), canvasTransform);
        CreateButton(PlayerDescription, () => SetCharacterType(0), canvasTransform);
        CreateButton(ZombieDescription, () => SetCharacterType(2), canvasTransform);
        
        // Button text should have the same dimensions as the button itself
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        for (var i = 0; i < transform.childCount; i++)
        {
            var buttonRect = gameObject.transform.GetChild(i).GetComponent<RectTransform>();
            var buttonTextRect = buttonRect.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>();

            // Use assignment operator rather than Vector2.Set(), as it does not work as expected
            buttonTextRect.sizeDelta = buttonRect.sizeDelta;
        }
    }

    private void CreateButton(string buttonText, UnityAction buttonOnClickMethod, Transform buttonParent)
    {
        var buttonObj = DefaultControls.CreateButton(_resources);
        
        var button = buttonObj.GetComponent<Button>();
        button.colors = _buttonColors;
        button.onClick.AddListener(buttonOnClickMethod);
        
        var buttonTransform = buttonObj.transform;
        Destroy(buttonTransform.GetChild(0).gameObject); // Destroying old text so it can be replaced with tmp text
        
        var newTextObj = new GameObject("Text", typeof(RectTransform), 
            typeof(CanvasRenderer), 
            typeof(TextMeshProUGUI));
        newTextObj.transform.SetParent(buttonTransform);
        
        var text = newTextObj.GetComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.font = _antonFont;
        text.color = _buttonFontColor;
        text.richText = true;
        text.alignment = TextAlignmentOptions.Top;

        buttonTransform.SetParent(buttonParent);
    }

    private void SetCharacterType(byte playerType)
    {
        LocalPlayerType = playerType;
        HasSelectedCharacter = true;
        Debug.Log("Set character type to: " + LocalPlayerType);

        Destroy(gameObject);
    }
}