using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrikeEffect : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject strikePrefab;
    [Space]
    public int minSegmentCount = 5;
    public int maxSegmentCount = 10;
    public float randomnessMagnitude = 1;
    public float flashTime = 0.5f;

    private Camera cam;
    private class StrikeEffect
    {
        public GameObject obj;
        public LineRenderer lineRenderer;
        public float timePassed = 0;
    }
    private List<StrikeEffect> effects = new List<StrikeEffect>();



    void Start()
    {
        Invoke(nameof(Init), 0.1f);
    }

    private void Init()
    {
        cam = Camera.main;
    }

    void Update()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            Color col = effects[i].lineRenderer.startColor;
            col.a = 1 - (effects[i].timePassed / flashTime);
            effects[i].lineRenderer.startColor = col;
            effects[i].lineRenderer.endColor = col;

            effects[i].timePassed += Time.deltaTime;
            if (effects[i].timePassed >= flashTime)
            {
                Destroy(effects[i].obj);
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Create lightning strikes converging at a target position
    /// </summary>
    /// <param name="target">The position in world space to strike</param>
    /// <param name="strikeCount">How many lightning strikes to create</param>
    public void CreateStrike(Vector3 target, int strikeCount)
    {
        // Move the target position onto the camera plane, and push it forward to avoid clipping the camera
        Vector2 targetScreen = cam.WorldToScreenPoint(target);
        target = cam.ScreenToWorldPoint(targetScreen) + canvas.forward * 2;

        for (int i = 0; i < strikeCount; i++)
        {
            StrikeEffect newEffect = new StrikeEffect();
            newEffect.obj = Instantiate(strikePrefab, canvas);
            newEffect.lineRenderer = newEffect.obj.GetComponent<LineRenderer>();
            // Get a position along the top and sides of the screen
            Vector2 spawnCoord;
            float randVal = Random.Range(0f, 3f);
            if (randVal < 1f)
                spawnCoord = new Vector2(-0.01f, Random.Range(0f, 1f));
            else if (randVal < 2f)
                spawnCoord = new Vector2(Random.Range(0f, 1f), 1.01f);
            else
                spawnCoord = new Vector2(1.01f, Random.Range(0f, 1f));
            Vector3 spawnPos = cam.ViewportToWorldPoint(spawnCoord) + canvas.forward * 2;
            
            int segmentCount = Random.Range(minSegmentCount, maxSegmentCount);
            Vector3[] positions = new Vector3[segmentCount];
            // Set the start and end positions
            positions[0] = spawnPos;
            positions[segmentCount - 1] = target;
            Transform camTrans = cam.transform;
            for (int j = 1; j < segmentCount - 1; j++)
            {
                positions[j] = Vector3.Lerp(spawnPos, target, j / (float)segmentCount);
                positions[j] += camTrans.rotation * (Random.insideUnitCircle * randomnessMagnitude);
            }
            // Set line renderer positions, and remove unnessesary ones
            newEffect.lineRenderer.positionCount = segmentCount;
            newEffect.lineRenderer.SetPositions(positions);
            newEffect.lineRenderer.Simplify(0.75f);
            effects.Add(newEffect);
        }
    }
}
