using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelParallax : MonoBehaviour
{
    /// <summary>
    /// Class visible dans l'éditeur, me permettant de rentrer les différents background en direct.
    /// </summary>
    [System.Serializable]
    public class BackgroundData
    {
        public Sprite sprite;

        [Range(0f, 1f)]
        public float speed = 0.5f;
        public bool wrapX = true;
        public bool wrapY = true;
        public Vector2 startOffset = Vector2.zero;
    }

    /// <summary>
    /// Classes créées et enregistrées dans une list dépendamment du nombre de layers de background dans le jeu.
    /// </summary>
    private class Layer
    {
        public Transform parent;
        private Transform transformLeft;
        private Transform transformMiddle;
        private Transform transformRight;
        private Transform transformUp;
        private Transform transformDown;
        private float parallaxFactor;
        private bool wrapX;
        private bool wrapY;
        private Vector2 startOffset;

        public Layer(BackgroundData _datas, int layerOrder, Transform _parent)
        {
            wrapX = _datas.wrapX;
            wrapY = _datas.wrapY;
            parallaxFactor = _datas.speed;
            parent = CreateBackgroundParents(_parent, layerOrder);
            transformMiddle = CreateSpritesGetTransform(_datas.sprite, layerOrder);

            if (wrapX)
            {
                transformLeft = CreateSpritesGetTransform(_datas.sprite, layerOrder);
                transformRight = CreateSpritesGetTransform(_datas.sprite, layerOrder);
            }

            if (wrapY)
            {
                transformUp = CreateSpritesGetTransform(_datas.sprite, layerOrder);
                transformDown = CreateSpritesGetTransform(_datas.sprite, layerOrder);
            }

            SetBackgroundsPositions();
        }

        public Transform CreateBackgroundParents(Transform _parent, int layerOrder)
        {
            GameObject empty = new GameObject("BackgroundLayer");

            empty.transform.SetParent(_parent);

            empty.transform.localPosition = Vector3.zero;
            empty.transform.localRotation = Quaternion.identity;
            empty.transform.localScale = Vector3.one;

            return empty.transform;
        }

        private Transform CreateSpritesGetTransform(Sprite _sprite, int layerOrder)
        {
            // Create a new GameObject for each background sprite
            GameObject backgroundObject = new GameObject("Background_" + _sprite.name);
            SpriteRenderer spriteRenderer = backgroundObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _sprite;
            spriteRenderer.sortingOrder = -10 - (layerOrder * 2); // Set the sorting order to ensure proper rendering (-10 ; -12, -14, -16....) 
            // Debug.Log("Setting sprite for " + backgroundObject.name + " : " + _sprite.rect.size.x + "x" + _sprite.rect.size.y);
            // Set the position of the background
            backgroundObject.transform.parent = parent;
            backgroundObject.transform.localPosition = Vector3.zero;
            return backgroundObject.transform;
        }

        private void SetBackgroundsPositions()
        {
            // Implementation for setting background positions
            Vector3 size = transformMiddle.GetComponent<SpriteRenderer>().bounds.size;

            transformMiddle.localPosition = (Vector2)(new Vector2(0, 0) + startOffset);

            if (wrapX)
            {
                transformLeft.localPosition = (Vector2)(new Vector2(-size.x, 0) + startOffset);
                transformRight.localPosition = (Vector2)(new Vector2(size.x, 0) + startOffset);
            }
            if (wrapY)
            {
                transformDown.localPosition = (Vector2)(new Vector2(0, -size.y) + startOffset);
                transformUp.localPosition = (Vector2)(new Vector2(0, size.y) + startOffset);
            }
        }

        public void WrapBackground(Vector2 _cameraPos)
        {
            Vector2 size = transformMiddle.GetComponent<SpriteRenderer>().bounds.size;

            if (_cameraPos.x < transformLeft.position.x + size.x / 2f)
            {
                UpdateBackground(Vector2.left);
            }
            else if (_cameraPos.x > transformRight.position.x - size.x / 2f)
            {
                UpdateBackground(Vector2.right);
            }

            if (_cameraPos.y < transformDown.position.y + size.y / 2f)
            {
                UpdateBackground(Vector2.down);
            }
            else if(_cameraPos.y < transformUp.position.y - size.y / 2f)
            {
                UpdateBackground(Vector2.up);
            }
        }

        private void UpdateBackground(Vector2 _dir)
        {
            // Implementation for setting background positions
            Vector3 size = transformMiddle.GetComponent<SpriteRenderer>().bounds.size;

            if (_dir.x < 0) // Vers la Gauche
            {
                Transform temp = transformRight;
                transformRight = transformMiddle;
                transformMiddle = transformLeft;
                transformLeft = temp;
                transformLeft.localPosition = transformMiddle.localPosition - new Vector3(size.x, 0, 0);
            }
            else if (_dir.x > 0) // Vers la Droite
            {
                Transform temp = transformLeft;
                transformLeft = transformMiddle;
                transformMiddle = transformRight;
                transformRight = temp;
                transformRight.localPosition = transformMiddle.localPosition + new Vector3(size.x, 0, 0);
            }

            if (_dir.y < 0) // Vers le Bas
            {
                Transform temp = transformUp;
                transformUp = transformMiddle;
                transformMiddle = transformDown;
                transformDown = temp;
                transformDown.localPosition = transformMiddle.localPosition - new Vector3(size.x, 0, 0);
            }
            else if (_dir.y > 0) // Vers le Haut
            {
                Transform temp = transformDown;
                transformDown = transformMiddle;
                transformMiddle = transformUp;
                transformUp = temp;
                transformUp.localPosition = transformMiddle.localPosition + new Vector3(size.x, 0, 0);
            }
        }

        public void DestroyLayer()
        {
            Destroy(parent.gameObject);
        }
    }

    [SerializeField] public List<BackgroundData> backgroundSprites; // Array of all the backgrounds to be parallaxed

    private List<Layer> backgroundLayers = new List<Layer>(); // Array of all the backgrounds to be parallaxed

    private void Awake()
    {
        // Ensure that the backgroundSprites list is not empty
        if (backgroundSprites == null || backgroundSprites.Count == 0)
        {
            Debug.LogWarning("Background sprites list is empty. Please assign sprites in the inspector.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBackgrounds(float _cameraPosX)
    {
        float currentCameraSize = Camera.main.orthographicSize;
        // Debug.Log("Current Camera Size: " + currentCameraSize);
        transform.position = new Vector3(_cameraPosX, -currentCameraSize, 0);

        for (int i = 0; i < backgroundSprites.Count; i++)
        {
            backgroundLayers.Add(new Layer(backgroundSprites[i], i + 1, transform));
        }
    }

    // Update is called once per frame
    public void UpdateBackgrounds(Vector2 _cameraPos)
    {
        transform.position = new Vector3(_cameraPos.x, _cameraPos.y, transform.position.z);

        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            backgroundLayers[i].parent.position = new Vector3(_cameraPos.x * (1f - backgroundSprites[i].speed), backgroundLayers[i].parent.position.y, backgroundLayers[i].parent.position.z);
            backgroundLayers[i].WrapBackground(_cameraPos);
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
