using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxArea : MonoBehaviour
{
    public class InstantiatedParallaxLayer
    {
        public Transform instantiatedLayer;
        public ParallaxLayer layerReference;
        public Vector2 speedOffset = Vector2.zero;
        public Vector2 elementSize = Vector2.zero;
    }

    [InlineEditor]
    public ParallaxBackground configuration;

    public Camera editorCamera;

    public Transform Mask;
    public Transform Root;

    public Transform parallaxLayerPrefab;
    public SpriteRenderer parallaxingBackgroundPrefab;

    public BoxCollider2D bounds;

    private readonly float focus = 96;

    public List<InstantiatedParallaxLayer> _instantiatedLayers = new List<InstantiatedParallaxLayer>();

    private void Start()
    {
        bounds = GetComponent<BoxCollider2D>();
        Mask.localScale = bounds.size;
    }

    private void Update()
    {
        if (parallaxingBackgroundPrefab == null)
        {
            return;
        }

        if (editorCamera == null && Manager.instance == null)
        {
            return;
        }

        if (editorCamera == null)
        {
            editorCamera = Manager.instance.mainCam;
        }

        Rect cameraBounds = editorCamera.GetCameraBounds();

        if (_instantiatedLayers.Count != configuration.layers.Count)
        {
            foreach (InstantiatedParallaxLayer layer in _instantiatedLayers)
            {
                Destroy(layer.instantiatedLayer.gameObject);
            }

            _instantiatedLayers = new List<InstantiatedParallaxLayer>();

            foreach (ParallaxLayer layer in configuration.layers)
            {
                CreateLayer(layer);
            }
        }

        foreach (InstantiatedParallaxLayer layer in _instantiatedLayers)
        {
            DrawLayer(layer, cameraBounds);
        }
    }

    [Button("Refresh")]
    public void Refresh()
    {
        foreach (Transform layer in Root)
        {
            DestroyImmediate(layer.gameObject);
        }

        _instantiatedLayers = new List<InstantiatedParallaxLayer>();

        foreach (ParallaxLayer layer in configuration.layers)
        {
            CreateLayer(layer);
        }
    }

    public void CreateLayer(ParallaxLayer layer)
    {
        InstantiatedParallaxLayer i = new InstantiatedParallaxLayer
        {
            instantiatedLayer = Instantiate(parallaxLayerPrefab, Root),
            layerReference = layer
        };

        Rect size = layer.image.rect;
        size.width = size.width / layer.image.pixelsPerUnit + layer.gap.x;
        size.height = size.height / layer.image.pixelsPerUnit + layer.gap.y;
        i.elementSize = new Vector2(size.width, size.height);
        Rect fullSize = ComputeLayerWidth(layer, size);

        float depth = 0;

        float zOffset = 0;

        depth = layer.depth / focus;
        depth = depth + 1;
        depth = 1 / (depth * depth);

        if (layer.overridePriority)
        {
            zOffset = layer.priority;
        }
        else
        {
            zOffset = 10 + 1000 - layer.depth;
        }

        Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>()
        {
            [-1] = bounds.bounds.min,
            [0] = bounds.bounds.center,
            [1] = bounds.bounds.max
        };

        i.instantiatedLayer.position = new Vector3(offsets[(int)layer.horizontalAlign].x, offsets[(int)layer.verticalAlign].y, 0);

        float xRepeats = 0;

        float horizontalOffset = (float)layer.horizontalAlign * -0.5f * fullSize.width;
        float verticalOffset = (float)layer.verticalAlign * -0.5f * fullSize.height;

        Vector2 toAddDueToSpeed = new Vector2(size.width * layer.speed.x != 0 ? 1 : 0, size.height * layer.speed.y != 0 ? 1 : 0);

        for (float x = -0.5f * fullSize.width + horizontalOffset - toAddDueToSpeed.x; x < 0.5f * fullSize.width + horizontalOffset + toAddDueToSpeed.x; x += size.width)
        {
            if (xRepeats > layer.repeat.x && layer.repeat.x >= 0)
            {
                break;
            }

            float yRepeats = 0;

            for (float y = -0.5f * fullSize.height + verticalOffset - toAddDueToSpeed.y; y < 0.5f * fullSize.height + verticalOffset + toAddDueToSpeed.y; y += size.height)
            {
                if (yRepeats > layer.repeat.y && layer.repeat.y >= 0)
                {
                    break;
                }

                SpriteRenderer image = Instantiate(parallaxingBackgroundPrefab, i.instantiatedLayer);
                Vector2 idealPivot = (new Vector2((int)layer.horizontalAlign, (int)layer.verticalAlign) + Vector2.one) * 0.5f;
                Vector2 pivotDifference = idealPivot - (image.sprite.pivot / image.sprite.rect.size);
                image.sprite = layer.image;
                image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                image.color = layer.tint;
                image.sortingLayerName = "Backgrounds";
                image.sortingOrder = (int)zOffset;

                if (layer.spriteMaterial != null)
                {
                    image.material = layer.spriteMaterial;
                }

                if (layer.extraEffects)
                {
                    Instantiate(layer.extraEffects, image.transform);
                }
                image.transform.localPosition = new Vector3(x + image.sprite.bounds.size.x * pivotDifference.x + layer.offset.x, y - image.sprite.bounds.size.y * pivotDifference.y + layer.offset.y, 0);
            }
        }
        _instantiatedLayers.Add(i);
    }

    private Rect ComputeLayerWidth(ParallaxLayer layer, Rect bounds)
    {
        if (layer.repeat.x > 0)
        {
            bounds.width += layer.gap.x;
            bounds.width *= layer.repeat.x - layer.gap.x;
        }
        else if (layer.repeat.x < 0)
        {
            bounds.width = this.bounds.bounds.size.x;
        }

        if (layer.repeat.y > 0)
        {
            bounds.height += layer.gap.y;
            bounds.height *= layer.repeat.y - layer.gap.y;
        }
        else if (layer.repeat.y < 0)
        {
            bounds.height = this.bounds.bounds.size.y;
        }

        bounds.width += layer.margin.left + layer.margin.right;
        bounds.height += layer.margin.top + layer.margin.bottom;

        return bounds;
    }

    private void DrawLayer(InstantiatedParallaxLayer layerInstance, Rect cameraBounds)
    {
        ParallaxLayer layer = layerInstance.layerReference;

        if (!layer.overridePriority && layer.depth / focus <= -1)
        {
            return;
        }

        float depth = 0;

        depth = layer.depth / focus;
        depth = depth + 1;
        depth = 1 / (depth * depth);

        Vector2 parallaxSpeed = new Vector2(depth, depth);
        if (layer.overrideSpeed)
        {
            parallaxSpeed = layer.speedOverride;
        }

        Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>()
        {
            [-1] = bounds.bounds.min,
            [0] = bounds.bounds.center,
            [1] = bounds.bounds.max
        };

        Dictionary<int, Vector2> cameraOffsets = new Dictionary<int, Vector2>()
        {
            [-1] = cameraBounds.min,
            [0] = cameraBounds.center,
            [1] = cameraBounds.max
        };

        Vector3 position = layerInstance.instantiatedLayer.position;

        layerInstance.speedOffset = layerInstance.speedOffset + layerInstance.layerReference.speed * Time.deltaTime;
        if (Mathf.Abs(layerInstance.speedOffset.x) > layerInstance.elementSize.x)
        {
            layerInstance.speedOffset.x = (Mathf.Abs(layerInstance.speedOffset.x) % layerInstance.elementSize.x) * Mathf.Sign(layerInstance.speedOffset.x);
        }
        if (Mathf.Abs(layerInstance.speedOffset.y) > layerInstance.elementSize.y)
        {
            layerInstance.speedOffset.y = (Mathf.Abs(layerInstance.speedOffset.y) % layerInstance.elementSize.y) * Mathf.Sign(layerInstance.speedOffset.y);
        }

        position.x = offsets[(int)layer.horizontalAlign].x + (1 - depth) * (cameraOffsets[(int)layer.horizontalAlign].x - offsets[(int)layer.horizontalAlign].x) + layerInstance.speedOffset.x;
        position.y = offsets[(int)layer.verticalAlign].y + (1 - depth) * (cameraOffsets[(int)layer.verticalAlign].y - offsets[(int)layer.verticalAlign].y) + layerInstance.speedOffset.y;

        layerInstance.instantiatedLayer.position = position;
    }

    public void OnDrawGizmos()
    {
        if (bounds == null)
        {
            bounds = GetComponent<BoxCollider2D>();
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.size.x, bounds.size.y, 1));
        Gizmos.color = Color.white;
    }
}