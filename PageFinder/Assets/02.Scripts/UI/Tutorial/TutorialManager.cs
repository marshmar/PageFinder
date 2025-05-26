using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private PlayerAttackController attackController;
    [SerializeField] private GameObject tutorialA;
    [SerializeField] private GameObject tutorialB;
    [SerializeField] private GameObject targetPanel;

    // A Text
    private string textControl = "WASD�� ����������!";
    private string textBasicAttack1 = "���� ����ü�� �� ���� �׸��� �Ÿ��� �־�..";
    private string textBasicAttack2 = "���� ���콺 Ŭ������ ��������";
    private string textBasicAttack3 = "���߾�, �ٷ� �װž�!";
    private string textInkDash1 = "�̹��� space�ٸ� ���� ����ũ ��á��� ����غ���";
    private string textInkDash2 = "��~ ������ ������? ��ũ ��ô� ������ �ڸ��� ��� ��ũ�� ����";
    private string textReward1 = "�� ���� ���� �������� �Ѿ �ð��̾�";
    private string textReward2 = "�����ر��� ����, ��ũ��Ʈ�� ��ȭ �� �뺻�� �������̾�";
    private string textReward3 = "�� �� �ϳ��� �����ؼ� ��������! ���� ���� ���ϰ� ������ٰž�";
    private string textPageSelection = "���� ��θ� ��������. ��Ŭ������ �������� ������ ��";

    // B Text
    private string textInkSkill1 = "E Ű�� ��ų�� ����� �� �־�! EŰ�� ��� ���� ��� ���� ������� ���� ��������. �����Ѹ�ŭ ���� ���� ��ũ�� ����ϴϱ� ������ ����� �ʿ���";
    private string textInkSkill2 = "���Ҿ�! ��ũ ��ų�� ���� ������ ��ũ�� �����ϴµ� ������! �������� ������ ū ������ �ɰž�.";
    private string textInkFusion1 = "�ٸ� ���� ��ũ�� ���� ���� �̻����� ����ġ�� �ó����� �߻���. �����÷��� ��ũ���� ������ �¼� �ð��̾�";
    private string textInkFusion2 = "���ҹٴ١��� ���� ��ũ�� �ʷ� ��ũ�� �������� �߻���. �ҹٴ� ������ ������ ���� ���ظ� �ް� ����. ���������� �ʷ� ��ũ�� �Ķ� ��ũ�� �������� �߻���. ���� ���� ������ ���� ü���� ȸ���� �� �־�.";
    private string textInkFusion3 = "���������� �Ȱ���. �Ȱ��� �Ķ����� ������ ��ũ�� �������� �߻���. �Ȱ��� ������ ���������� ���� �Ⱥ��� �츮�� ���� ���ϰ� �� �� ���� �������� ����";

    public void SendA(string text, float duration)
    {
        var A = Instantiate(tutorialA, targetPanel.transform);
        A.GetComponentInChildren<TMP_Text>().text = text;
        Destroy(A, duration);
    }

    public void SendAToPageMap(float duration, GameObject pageMap)
    {
        var A = Instantiate(tutorialA, pageMap.transform);
        A.GetComponentInChildren<TMP_Text>().text = textPageSelection;
        Destroy(A, duration);
    }

    private void SendAToReward(string text, float duration, GameObject reward)
    {
        var A = Instantiate(tutorialA, reward.transform);
        A.GetComponentInChildren<TMP_Text>().text = text;
        Destroy(A, duration);
    }

    public void SendB()
    {
        Instantiate(tutorialB, targetPanel.transform);
    }

    private void Start()
    {
        StartCoroutine(PlayTutorial());
    }

    IEnumerator PlayTutorial()
    {
        // Control
        SendA(textControl, 4f);
        
        // Basic Attack
        yield return new WaitForSeconds(5f);
        GameData.Instance.SpawnEnemies();
        SendA(textBasicAttack1, 2f);
        yield return new WaitForSeconds(2.5f);
        SendA(textBasicAttack2, 4f);
        yield return new WaitForSeconds(5f);
        SendA(textBasicAttack3, 2f);

        // Ink Dash
        yield return new WaitForSeconds(2.5f);
        SendA(textInkDash1, 4f);
        yield return new WaitForSeconds(4f);
        SendA(textInkDash2, 3f);
        
        // Reward
        yield return new WaitForSeconds(4f);
        SendA(textReward1, 2f);
        yield return new WaitUntil(() => rewardPanel.activeInHierarchy);
        SendAToReward(textReward2, 1.5f, rewardPanel);
        yield return new WaitForSeconds(2f);
        SendAToReward(textReward3, 1f, rewardPanel);
    }
}