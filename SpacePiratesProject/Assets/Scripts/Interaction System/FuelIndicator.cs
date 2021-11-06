using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelIndicator : MonoBehaviour
{
    public FuelCell[] cells;
    public Image interactionIcon;
    public Sprite normalIcon;
    public Sprite importantIcon;
    public Color[] colors;

    private float lastLevel = 0;



    void Start()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].SetColor(colors[i]);
        }

        SetFuelLevel(0);
    }

    public void SetFuelLevel(float fuel)
	{
        lastLevel = fuel;
        float per = 100 / cells.Length;

        int fullCount = (int)fuel / (int)per;
        Color newCol = colors[Mathf.Min(fullCount, colors.Length - 1)];
        newCol.a = interactionIcon.color.a;
        interactionIcon.color = newCol;
        interactionIcon.sprite = fuel < per ? importantIcon : normalIcon;

        for (int i = 0; i < cells.Length; i++)
        {
            // Get the level for this fuel cell
            float currentLevel = Mathf.Max(fuel - per * i, 0);

            if (currentLevel >= per)
			{
                //full
                cells[i].SetTarget(Vector3.one);
            }
			else if (currentLevel > 0)
			{
                //part full
                cells[i].SetTarget(Vector3.Lerp(Vector3.one, Vector3.zero, 1 - currentLevel / per));
            }
			else
			{
                //empty
                cells[i].SetTarget(Vector3.zero);
            }
        }
    }

    void OnEnable()
    {
        foreach (var obj in cells)
        {
            obj.outer.enabled = true;
        }
        SetFuelLevel(lastLevel);
    }

    void OnDisable()
    {
        foreach (var obj in cells)
        {
            obj.outer.enabled = false;
            obj.filled.enabled = false;
        }
    }
}
