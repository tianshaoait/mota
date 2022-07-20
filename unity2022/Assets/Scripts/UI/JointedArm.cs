using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class JointedArm : ScrollRect, IPointerDownHandler
{
    public Action<Vector2> onDragCb;
    public Action onStopCb;

    protected float mRadius = 0f;

    private Transform trans;
    private RectTransform bgTrans;
    private Camera uiCam;
    private Vector3 originalPos;

    protected override void Awake()
    {
        base.Awake();
        trans = transform;
        bgTrans = trans.Find("bg") as RectTransform;
        uiCam = GameObject.Find("UICamera").GetComponent<Camera>();
        originalPos = trans.localPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //松手时，摇杆复位
            trans.localPosition = originalPos;
            this.content.localPosition = Vector3.zero;
        }
    }

    protected override void Start()
    {
        base.Start();
        //计算摇杆块的半径
        mRadius = bgTrans.sizeDelta.x * 0.5f;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
            SetContentAnchoredPosition(contentPostion);
        }
        Debug.Log("摇杆滑动，方向：" + contentPostion);

        if (null != onDragCb)
            onDragCb(contentPostion);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        Debug.Log("摇杆拖动结束");
        if (null != onStopCb)
            onStopCb();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //点击到摇杆的区域，摇杆移动到点击的位置
        trans.position = uiCam.ScreenToWorldPoint(eventData.position);
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, 0);
    }
}
