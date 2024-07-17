using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation
{
    private TextureData _texData;
    private SpriteMgr _spriteMgr;
    private bool _playing = false;
    private float _speed = 0.2f;
    private float _time = 0;
    private int _frame = 0;

    public SpriteAnimation(string goName)
    {
        _spriteMgr = new SpriteMgr(goName);
    }

    public void UpdateFrame()
    {
        if (!_playing)
            return;

        if (_time >= speed)
        {
            _time = 0;
            SetSprite();
        }

        _time += Time.deltaTime;
    }

    public void SetTextureData(TextureData texData)
    {
        _texData = texData;
        SetSprite();
    }

    public void SetSprite()
    {
        Sprite sprite = _texData.GetTexture2D(_frame);
        _spriteMgr.sprite = sprite;
        _frame++;

        if (_frame >= _texData.FrameCount())
            _frame = 0;
    }

    public void Reset()
    {
        _frame = 0;
        _time = 0;
    }

    public void Play()
    {
        _playing = true;
    }

    public void Stop()
    {
        _playing = false;
    }

    public SpriteMgr spriteMgr
    {
        get { return _spriteMgr; }
    }

    public float speed
    {
        set { _speed = value; }
        get { return _speed; }
    }

    public void Dispose()
    {
        _spriteMgr?.Dispose();
    }
}
