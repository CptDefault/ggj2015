using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public AudioClipContainer PickUpItem;
    public AudioClipContainer DropItem;

    private Tool _overTool;
    private Tool _carriedTool;

    public Tool CarriedTool {get { return _carriedTool;} }

    public GameObject TipPrefab;
    private UILabel _tipLabel;
    public UILabel TipLabel { get {return _tipLabel;}}

    public bool InDiningRoom;

    public void Awake () {
        if(_tipLabel == null)
            _tipLabel = (GameObject.Instantiate(TipPrefab) as GameObject).GetComponent<UILabel>();
    }

    public void Update () {
        // text follow around
        if(_tipLabel.text != "") {
            _tipLabel.transform.parent = UICamera.mainCamera.transform.parent;
            _tipLabel.transform.localScale = TipPrefab.transform.localScale;

            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.x -= (Screen.width/ 2.0f);
            screenPos.y -= (Screen.height / 2.0f);
            screenPos.y -= Screen.height/22f;
            _tipLabel.transform.localPosition = screenPos;
        }
    }

    public void PickupDropItem()
    {
        if (_carriedTool != null)
        {
            _carriedTool.Drop(transform.position);
            _overTool = _carriedTool;
            _carriedTool = null;
            DropItem.Play();
        }
        else if (_overTool && !_overTool.Carried)
        {
            _carriedTool = _overTool;
            _carriedTool.PickUp(transform);
            PickUpItem.Play();
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
            _tipLabel.text = "(B) Pickup/drop item";
        }

        if(col.gameObject.tag == "DiningRoom") {
            InDiningRoom = true;
        }
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        var tool = col.GetComponent<Tool>();
        if (tool == _overTool && tool != null)
        {
            _overTool = null;
            if(_carriedTool == null || _carriedTool.Type != Tool.ToolType.Extinguisher)
                _tipLabel.text = "";
        }

        if(col.gameObject.tag == "DiningRoom") {
            InDiningRoom = false;
        }
    }

    public void ConsumeTool() {
        Destroy(_carriedTool.gameObject);
        _carriedTool = null;
    }
}
