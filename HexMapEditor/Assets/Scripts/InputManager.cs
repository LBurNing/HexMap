using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    //单选高亮
    private GameObject _box;
    private HexCell _clickedCell;
    private bool _f5FirstClick = false;
    private bool _f6FirstClick = false;

    public HexCell ClickedCell
    {
        get { return _clickedCell; }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Event.Fire(Event.ESC_INPUT);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            _f5FirstClick = !_f5FirstClick;
            Event<bool>.Fire(Event.F5, _f5FirstClick);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            _f6FirstClick = !_f6FirstClick;
            Event<bool>.Fire(Event.F6, _f6FirstClick);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Event<int>.Fire(Event.ADD_BRUSH_SCALE, 1);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Event<int>.Fire(Event.ADD_BRUSH_SCALE, -1);
        }

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            Event<float>.Fire(Event.MOUSE_SCROLLWHEEL, mouseScroll);
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            Event<HexCell>.Fire(Event.KEY_COPY, _clickedCell);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            Event<HexCell>.Fire(Event.KEY_PASTE, _clickedCell);
        }
    }

    void HandleInput()
    {
        //Vector3 wordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //HexCell cell = HexGrid.instance.GetCell(wordPos);
        //Event<HexCell>.Fire(Event.MOUSE_CLICK, cell);

        //射线检测更加准确, 后期看需求修改
        Ray inputRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        RaycastHit hit;

        if (_box != null)
        {
            Destroy(_box.GetComponent<Outline>());
        }

        if (Physics.Raycast(inputRay, out hit))
        {
            FireMouseClick(hit);
        }

        HexCell brushCell = null;
        if (BrushManager.instance.brush)
        {
            brushCell = BrushManager.instance.brush;
        }
        else if (ResBrushManager.instance.brush)
        {
            brushCell = ResBrushManager.instance.brush;
        }
        else if (AreaBrushManager.instance.brush)
        {
            brushCell = AreaBrushManager.instance.brush;
        }

        if (brushCell)
        {
            RaycastHit[] hits = Physics.SphereCastAll(inputRay, brushCell.localScale.x / 2);
            for (int i = 0; i < hits.Length; i++)
            {
                FireMouseClick(hits[i]);
            }
        }
    }

    private void FireMouseClick(RaycastHit hit)
    {
        _clickedCell = HexGrid.instance.GetCell(hit.point);
        Event<HexCell>.Fire(Event.BRUSH_MOUSE_CLICK, _clickedCell);

        if (ResBrushManager.instance.IsNull 
            && BrushManager.instance.IsNull 
            && AreaBrushManager.instance.IsNull)
        {
            _box = _clickedCell.GetOutlineObj();
            _box.AddComponent<Outline>();

            Event<HexCell>.Fire(Event.MOUSE_CLICK, _clickedCell);
        }
    }
}
