using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private Tool _overTool;
    private Tool _carriedTool;

    public Tool CarriedTool {get { return _carriedTool;} }

    public void PickupDropItem()
    {
        print("Try pickup");
        if (_carriedTool != null)
        {
            _carriedTool.Drop(transform.position);
            _overTool = _carriedTool;
            _carriedTool = null;
        }
        else if (_overTool && !_overTool.Carried)
        {
            _carriedTool = _overTool;
            _carriedTool.PickUp(transform);
        }
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        print("Try over");
        var tool = col.GetComponent<Tool>();
        if (tool != null)
        {
            print("is over");
            _overTool = tool;
        }
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        var tool = col.GetComponent<Tool>();
        if (tool == _overTool)
        {
            _overTool = null;
        }
    }

    public void ConsumeTool() {
        Destroy(_carriedTool.gameObject);
        _carriedTool = null;
    }
}
