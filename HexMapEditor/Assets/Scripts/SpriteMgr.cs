using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMgr
{
    private SpriteRenderer _sr;
    private TextureData _texData;

    public SpriteMgr(string name)
    {
        _sr = new GameObject(name).AddComponent<SpriteRenderer>();
        _sr.material = Global.instance.textureExMat;
    }

    public void SetRes(TextureData texData)
    {
        _texData = texData;
    }

    public Sprite sprite
    {
        set
        {
            _sr.sprite = value;
        }
    }

    public Texture2D texture2D
    {
        set
        {
            Vector2 pivot = new Vector2(0.5f, 0);
            pivot.y = (float)value.width / (float)value.height / 2.0f;
            Sprite sprite = Sprite.Create(value, new Rect(0, 0, value.width, value.height), pivot);
            _sr.sprite = sprite;
        }
    }

    public Transform parent
    {
        set
        {
            _sr.transform.parent = value;
            _sr.transform.localScale = Vector3.one;
            _sr.transform.localPosition = Vector3.zero;
        }
    }

    public Quaternion localRotation
    {
        set{ _sr.transform.localRotation = value; }
    }

    public int orderInLayer
    {
        set { _sr.sortingOrder = value; }
    }

    public void SetTitleColor(CellsType cellType)
    {
        Color cellColor = Global.instance.GetColor(cellType);
        _sr.material.SetColor("_Color", cellColor);
    }

    public void SetFogAlpha(LayerType layerType)
    {
        if(layerType == LayerType.fog)
        {
            Color color = _sr.material.color;
            _sr.material.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }

    public Vector3 localPosition
    {
        set { _sr.transform.localPosition = value; }
    }

    public GameObject gameobject
    {
        get { return _sr.gameObject; }
    }

    public void Dispose()
    {
        if (_sr != null)
            GameObject.Destroy(_sr);
    }
}
