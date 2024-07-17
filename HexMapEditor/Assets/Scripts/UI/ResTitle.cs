using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResTitle : MonoBehaviour
{
    public Button _titleButton;
    public Image _titleImage;
    public RectTransform _titleRTF;
    public bool _playing = true;
    private float _speed = 0.2f;
    private float _time = 0;
    private int _frame = 0;

    private float _width;
    private float _height;
    private TextureData _texData;
    private SpriteAnimation _animator;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;


    private void Awake()
    {
        _titleButton = GetComponent<Button>();
        _titleImage = GetComponent<Image>();
        _titleRTF = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _titleButton.onClick.AddListener(TitleClick);
    }

    private void Update()
    {
        if (!_playing)
            return;

        if (_time >= _speed)
        {
            _time = 0;
            SetSprite();
        }

        _time += Time.deltaTime;
    }

    public void SetSprite()
    {
        Sprite sprite = _texData.GetTexture2D(_frame);
        _titleImage.sprite = sprite;
        _frame++;

        if (_frame >= _texData.FrameCount())
            _frame = 0;
    }

    public void SetTexture(TextureData texData)
    {
        _texData = texData;
        SetSprite();
    }

    public void SetScale(float multiple)
    {
        _width = _titleImage.sprite.texture.width * multiple;
        _height = _titleImage.sprite.texture.height * multiple;
        _titleRTF.sizeDelta = new Vector2(_width, _height);
    }

    private void TitleClick()
    {
        if (BrushManager.instance.areaResObj)
        {
            Destroy(BrushManager.instance.areaResObj.GetComponent<Outline>());
        }

        if (BrushManager.instance.bArea)
        {
            BrushManager.instance.areaResObj = gameObject;
            gameObject.AddComponent<Outline>();
            gameObject.GetComponent<Outline>().effectColor = Color.green;
            gameObject.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
        }
        else
        {
            ResBrushManager.instance.UpdateBrush(_texData);
        }

        BrushManager.instance.Destroy();
    }

    public MeshFilter meshFilter
    {
        get
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();

            return _meshFilter;
        }
    }

    public MeshRenderer meshRenderer
    {
        get
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();

            return _meshRenderer;
        }
    }

    public MeshCollider meshCollider
    {
        get
        {
            if (_meshCollider == null)
                _meshCollider = GetComponent<MeshCollider>();

            return _meshCollider;
        }
    }

    public Mesh sharedMesh
    {
        set
        {
            if (value == null)
                return;

            meshFilter.sharedMesh = value;
            meshCollider.sharedMesh = value;
        }

        get
        {
            return meshFilter.sharedMesh;
        }
    }
}
