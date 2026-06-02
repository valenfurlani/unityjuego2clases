using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] private Color color = new Color(0.4f, 0.85f, 1f, 0.45f);
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private int segments = 80;
    [SerializeField] private int sortingOrder = 10;

    private LineRenderer lineRenderer;
    private AutomaticShooter shooter;
    private float currentRadius;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        shooter = GetComponent<AutomaticShooter>();
        Setup();
    }

    private void Setup()
    {
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.sortingOrder = sortingOrder;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void Update()
    {
        if (shooter == null) return;

        float radius = shooter.DetectionRadius;
        if (!Mathf.Approximately(radius, currentRadius))
        {
            currentRadius = radius;
            UpdateCircle();
        }
    }

    private void UpdateCircle()
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            lineRenderer.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * currentRadius,
                Mathf.Sin(angle) * currentRadius,
                0f));
        }
    }
}
