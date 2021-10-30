using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ICharacter : MonoBehaviour
{
    public class CharacterData
	{
        public string characterName;
        public GameObject character;
        public Renderer renderer;
        public Material[] variants;
        public Transform grabTransform;
        public Animator animator;
	}
    private CharacterData[] characters;
    protected CharacterData currentCharacter = null;
    public ParticleSystem ringEffect;
    public TextMeshProUGUI playerText;
    public Image arrow;
    public Color[] playerColors;

    // Grab transform used when not set in character settings
    public Transform fallbackGrabTransform;

    private bool useCharacterSelectAnimations = false;



    protected virtual void Awake()
	{
        ringEffect.Stop();
        arrow.enabled = false;
        playerText.enabled = false;
        Animator baseAnim = GetComponent<Animator>();

        // Get characters
        characters = new CharacterData[Mathf.Min(transform.childCount, 4)];
        for (int i = 0; i < Mathf.Min(transform.childCount, 4); i++)
		{
            // Get child and clean it
            Transform obj = transform.GetChild(i);

            // Get or create an animator, and set to to use the base animators settings
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
			{
                anim = obj.gameObject.AddComponent<Animator>();
			}
            anim.applyRootMotion = baseAnim.applyRootMotion;
            anim.avatar = baseAnim.avatar;
            anim.runtimeAnimatorController = baseAnim.runtimeAnimatorController;
            anim.enabled = false;


            CharacterSettings settings = obj.GetComponent<CharacterSettings>();
            settings.Init();
            CharacterData data = new CharacterData
            {
                characterName = settings.characterName,
                character = obj.gameObject,
                renderer = obj.GetComponentInChildren<Renderer>(),
                variants = settings.variants,
                grabTransform = settings.grabTransform != null ? settings.grabTransform : fallbackGrabTransform,
                animator = anim
            };
			characters[i] = data;
        }

        Destroy(baseAnim);

        // Disable all characters, and activate the first one
        foreach (var it in characters)
		{
            it.character.SetActive(false);
		}
        SetCharacter(0);
    }

    protected virtual void Start()
	{
        if (m_Player != null)
        {
            transform.parent = m_Player.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            //use debug mode

            return;
        }
    }

    public void BeginGame()
    {
        ringEffect.Play();
        ParticleSystem.MainModule main = ringEffect.main;
        main.startColor = playerColors[Player.playerIndex];

        arrow.enabled = true;
        arrow.color = playerColors[Player.playerIndex];
        playerText.enabled = true;
        playerText.color = playerColors[Player.playerIndex];
        playerText.text = "P" + (Player.playerIndex + 1).ToString();
    }

    public void SetCharacter(int index)
    {
        if (m_CharacterIndex == index || index >= GetCharacterCount())
		{
            return;
		}

        // Disable the current character
        if (currentCharacter != null)
		{
            currentCharacter.character.SetActive(false);
            currentCharacter.animator.enabled = false;
		}

        // Setup the new character
        currentCharacter = characters[index];
        currentCharacter.character.SetActive(true);
        m_CharacterIndex = index;
        m_CharacterName = currentCharacter.characterName;
        currentCharacter.animator.enabled = true;
        currentCharacter.animator.Play(useCharacterSelectAnimations ? "Base Layer.CharSelect_" + currentCharacter.characterName : "Base Layer.Idle", 0, 0);
        
        SetVariant(0);
    }

    public void SetVariant(int index)
	{
        if (m_CharacterIndex == -1)
        {
            return;
        }

        index = Mathf.Clamp(index, 0, GetVariantCount());
        currentCharacter.renderer.material = currentCharacter.variants[index];
        m_VariantIndex = index;
    }

    public void SetUseCharacterSelectAnimations(bool value)
	{
        if (useCharacterSelectAnimations == value)
		{
            return;
		}
        useCharacterSelectAnimations = value;

        if (currentCharacter != null && currentCharacter.animator != null)
		{
            currentCharacter.animator.Play(value ? "Base Layer.CharSelect_" + currentCharacter.characterName : "Base Layer.Idle", 0, 0);
            // Fix to hide player indicator after returning to menu
            if (value)
			{
                ringEffect.Stop();
                arrow.enabled = false;
                playerText.enabled = false;
            }
        }
	}


    public int GetCharacterCount() => characters.Length;
    public CharacterData GetCharacter() => characters[m_CharacterIndex];
    public int GetVariantCount()
    {
        return currentCharacter.variants.Length;
    }
    public int GetVariantCount(int characterIndex)
    {
        return characters[characterIndex].variants.Length;
    }

    public string CharacterName
	{
		get
		{
            return m_CharacterName;
		}
	}
    public int CharacterIndex
    {
        get
        {
            return m_CharacterIndex;
        }
    }
    public int VariantIndex
    {
        get
        {
            return m_VariantIndex;
        }
    }
    public Player Player
    {
        get
        {
            return m_Player;
        }
		set
		{
            m_Player = value;
		}
    }

    private string m_CharacterName;
    private int m_CharacterIndex = -1;
    private int m_VariantIndex = -1;
    private Player m_Player;
}