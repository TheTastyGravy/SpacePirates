using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICharacter : MonoBehaviour
{
    public class CharacterData
	{
        public string characterName;
        public GameObject character;
        public Renderer renderer;
        public Material[] variants;
        public Transform grabTransform;
	}
    private CharacterData[] characters;
    private CharacterData currentCharacter = null;


    // Material used when there are no variants
    public Material fallbackMaterial;
    public Transform fallbackGrabTransform;



    protected virtual void Awake()
	{
        // Get characters
        characters = new CharacterData[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
		{
            // Get child and clean it
            Transform obj = transform.GetChild(i);
            if (obj.TryGetComponent(out Animator animator))
            {
                Destroy(animator);
            }

            CharacterSettings settings = obj.GetComponent<CharacterSettings>();
            CharacterData data = new CharacterData
            {
                characterName = settings.characterName,
                character = obj.gameObject,
                renderer = obj.GetComponentInChildren<Renderer>(),
                variants = settings.variants,
                grabTransform = settings.grabTransform != null ? settings.grabTransform : fallbackGrabTransform
            };
			characters[i] = data;
        }

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
		}

        // Setup the new character, making it the first sibling so the animator works
        currentCharacter = characters[index];
        currentCharacter.character.SetActive(true);
        currentCharacter.character.transform.SetAsFirstSibling();
        m_CharacterIndex = index;
        m_CharacterName = currentCharacter.characterName;
        SetVariant(0);
    }

    public void SetVariant(int index)
	{
        if (m_CharacterIndex == -1)
        {
            return;
        }

        int variantCount = GetVariantCount();
        // If there are no variants, use the fallback
        if (variantCount == 0)
		{
            currentCharacter.renderer.material = fallbackMaterial;
            m_VariantIndex = 0;
        }
		else
		{
            index = Mathf.Clamp(index, 0, variantCount);
            currentCharacter.renderer.material = currentCharacter.variants[index];
            m_VariantIndex = index;
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