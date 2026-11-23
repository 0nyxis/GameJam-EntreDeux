using System.Collections.Generic;
using UnityEngine;

public class LevelParallax : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundData
    {
        public Sprite sprite;

        [Range(0f, 1f)]
        public float speed = 0.5f;

        [Tooltip("Coché : le layer défile (parallaxe) et se répète en X. Décoché : fixe en X.")]
        public bool wrapX = true;
        [Tooltip("Coché : le layer défile (parallaxe) et se répète en Y. Décoché : fixe en Y.")]
        public bool wrapY = true;

        [Tooltip("Position du layer par rapport à la caméra au démarrage.")]
        public Vector2 startOffset = Vector2.zero;
    }

    private class Layer
    {
        public Transform parent;

        private Transform tMiddle, tLeft, tRight, tUp, tDown;
        private readonly float speed;
        private readonly bool wrapX, wrapY;
        private readonly Vector2 startOffset;
        private readonly Vector2 anchor;   // position de la caméra au démarrage
        private readonly Vector2 size;     // taille d'une tuile (monde)
        private Vector2 origin;            // position locale de la tuile centrale

        public Layer(BackgroundData data, int layerOrder, Transform root, Vector2 cameraPos)
        {
            wrapX = data.wrapX;
            wrapY = data.wrapY;
            speed = data.speed;
            startOffset = data.startOffset;
            anchor = cameraPos;
            origin = startOffset;

            parent = new GameObject("BackgroundLayer_" + layerOrder).transform;
            parent.SetParent(root);
            parent.position = new Vector3(anchor.x, anchor.y, root.position.z);
            parent.localRotation = Quaternion.identity;
            parent.localScale = Vector3.one;

            tMiddle = CreateTile(data.sprite, layerOrder);
            size = tMiddle.GetComponent<SpriteRenderer>().bounds.size;

            if (wrapX)
            {
                tLeft = CreateTile(data.sprite, layerOrder);
                tRight = CreateTile(data.sprite, layerOrder);
            }
            if (wrapY)
            {
                tUp = CreateTile(data.sprite, layerOrder);
                tDown = CreateTile(data.sprite, layerOrder);
            }

            PlaceTiles();
        }

        private Transform CreateTile(Sprite sprite, int layerOrder)
        {
            GameObject go = new GameObject("Background_" + sprite.name);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = -10 - (layerOrder * 2); // -12, -14, -16...
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private void PlaceTiles()
        {
            tMiddle.localPosition = origin;
            if (wrapX)
            {
                tLeft.localPosition = origin + new Vector2(-size.x, 0);
                tRight.localPosition = origin + new Vector2(size.x, 0);
            }
            if (wrapY)
            {
                tDown.localPosition = origin + new Vector2(0, -size.y);
                tUp.localPosition = origin + new Vector2(0, size.y);
            }
        }

        public void UpdateLayer(Vector2 cameraPos)
        {
            // Parallaxe seulement sur les axes "wrap". Sinon le layer reste fixe dans le monde.
            float x = wrapX ? anchor.x + (cameraPos.x - anchor.x) * (1f - speed) : anchor.x;
            float y = wrapY ? anchor.y + (cameraPos.y - anchor.y) * (1f - speed) : anchor.y;
            parent.position = new Vector3(x, y, parent.position.z);

            if (!wrapX && !wrapY) return;

            bool moved = false;
            Vector2 center = (Vector2)parent.position + origin;

            if (wrapX && size.x > 0f)
            {
                while (cameraPos.x < center.x - size.x / 2f) 
                { 
                    origin.x -= size.x; center.x -= size.x; moved = true; 
                }
                while (cameraPos.x > center.x + size.x / 2f) 
                { 
                    origin.x += size.x; center.x += size.x; moved = true; 
                }
            }
            if (wrapY && size.y > 0f)
            {
                while (cameraPos.y < center.y - size.y / 2f) 
                { 
                    origin.y -= size.y; center.y -= size.y; moved = true; 
                }
                while (cameraPos.y > center.y + size.y / 2f) 
                { 
                    origin.y += size.y; center.y += size.y; moved = true; 
                }
            }

            if (moved)
            {
                PlaceTiles(); 
            }
        }

        public void DestroyLayer()
        {
            if (parent != null)
            {
                Object.Destroy(parent.gameObject); 
            }
        }
    }

    [SerializeField] public List<BackgroundData> backgroundSprites;

    private readonly List<Layer> backgroundLayers = new List<Layer>();
    private Camera mainCamera;

    private void Awake()
    {
        if (backgroundSprites == null || backgroundSprites.Count == 0)
        {
            Debug.LogWarning("Background sprites list is empty. Please assign sprites in the inspector.");
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        StartBackgrounds(mainCamera.transform.position);
    }

    private void Update()
    {
        UpdateBackgrounds(mainCamera.transform.position);
    }

    public void StartBackgrounds(Vector2 cameraPos)
    {
        transform.position = new Vector3(cameraPos.x, cameraPos.y, 0f);

        for (int i = 0; i < backgroundSprites.Count; i++)
        {
            if (backgroundSprites[i].sprite == null) continue;
            {
                backgroundLayers.Add(new Layer(backgroundSprites[i], i + 1, transform, cameraPos));
            }
        }
    }

    public void UpdateBackgrounds(Vector2 cameraPos)
    {
        transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);

        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            backgroundLayers[i].UpdateLayer(cameraPos);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            backgroundLayers[i].DestroyLayer();
        }
    }
}