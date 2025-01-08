using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum INKMARKTYPE
{
    BASICATTACK,
    DASH,
    INKSKILL,
    INTERACTIVEOBJECT
}

/// <summary>
/// 잉크 마크를 세팅하는 클래스
/// 잉크 마크 프리팹을 여러개 쓰면 문제점.
/// 잉크 마크를 되돌려서 줬을 때 그 오브젝트가 해당하는 타입의 오브젝트가 아닐 수 있음
/// ex. 잉크 대쉬의 결과 대쉬 마크 프리팹을 생성한 뒤에 오브젝트 풀에반환
/// -> 그러면 해당 프리팹은 잉크 대쉬의 형상을 띠고 있음
/// -> 차후 잉크 스킬의 결과로 잉크 마크가 필요할 때 받은 오브젝트가 잉크 대쉬 마크를 프리팹화 했던 객체라면
/// 초기화가 필요함
/// 결론적으로 Pool을 사용하는게 이득일까??
/// 
/// ->만약 잉크마크 세터에서 모든걸 관장한다고 치고, 잉크 마크 생성시(활성화할시)
/// 데이터에 따라 잉크 마크를 세팅하고, 합성시에도 마크 세터에서 데이터를 불러와서 해야하나..?
/// 고민해야될 부분.
/// </summary>
public class InkMarkSetter : MonoBehaviour
{
    public InkMarkData[] inkMarksDatas; // 0: BA, 1: Dash, 2: Skill, 3: InteractiveObject

    public void SetInkMarkScale(INKMARKTYPE inkType, Transform inkMarkTransform)
    {
        switch (inkType)
        {
            case INKMARKTYPE.BASICATTACK:
                inkMarkTransform.localScale = inkMarksDatas[0].scale;
                break;
            case INKMARKTYPE.DASH:
                inkMarkTransform.localScale = inkMarksDatas[1].scale;
                break;
            case INKMARKTYPE.INKSKILL:
                inkMarkTransform.localScale = inkMarksDatas[2].scale;
                break;
            case INKMARKTYPE.INTERACTIVEOBJECT:
                inkMarkTransform.localScale = inkMarksDatas[3].scale;
                break;
        }
    }

    public void SetInkMarkSprite(InkType inkType, Sprite inkMarkSprite)
    {
        switch (inkType)
        {
            case InkType.RED:
                break;
            case InkType.GREEN:
                break;
            case InkType.BLUE:
                break;
        }
    }
}
