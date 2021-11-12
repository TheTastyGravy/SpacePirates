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
    private bool isHidden = false;



    void Start()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].SetColor(colors[i]);
            cells[i].SetColor(new Color(cells[i].outer.color.r, cells[i].outer.color.g, cells[i].outer.color.b, 0));
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
        if (isHidden)
            return;

        foreach (var obj in cells)
        {
            obj.outer.enabled = true;
        }
        SetFuelLevel(lastLevel);
    }

    void OnDisable()
    {
        if (isHidden)
            return;

        foreach (var obj in cells)
        {
            obj.outer.enabled = false;
            obj.filled.enabled = false;
        }
    }

    internal void SetHidden(bool value)
    {
        isHidden = value;

        if (isHidden)
        {
            foreach (var cell in cells)
            {
                cell.SetColor(new Color(cell.outer.color.r, cell.outer.color.g, cell.outer.color.b, 0));
            }
        }
        else
        {
            IEnumerator SimpleFade()
            {
                float time = 2;
                float t = 0;
                while (t < time)
                {
                    foreach (var cell in cells)
                    {
                        cell.SetColor(new Color(cell.outer.color.r, cell.outer.color.g, cell.outer.color.b, Mathf.Lerp(0, 1, t / time)));
                    }
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            StartCoroutine(SimpleFade());
        }
        
    }
}
