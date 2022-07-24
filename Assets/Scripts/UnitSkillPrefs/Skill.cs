using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum skillType {AD,AP,Heal,Shield,Burn,Cold,Poison,Mix}//��������
public enum heroAttribute {Atk}//����

[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("�ı�����")]
    public string skillName;//������
    public string description;//��������

    [Header("��������")]
    public bool use;//�Ƿ����ʹ��
    public skillType type;//��������
    public int skillTired;//����ƣ��

    [Header("�ж��Ƿ��Ƕ�����ʹ��")]
    public bool myself; //�ж��Ƿ��Ƕ�����ʹ��

    [Header("�ж��Ƿ��ȫ�����ʹ��")]
    public bool allEnemies;//�ж��Ƿ��ȫ�����ʹ��

    [Header("�ж��Ƿ�Ϊ������δ��ѡ���ǵз�")]
    public bool players;//�ж��Ƿ�Լ���ʹ�ã��������ǵз�

    [Header("��������ΪMix��ʱ�����ã��Ӽ�������(ʵ�ֶ���˺�������ֵ�˺�)")]
    public List<Skill> moreSkill;

    [Header("������ֵ����")]
    public int baseInt;//���ܻ�����
    public List<heroAttribute> attribute;//�������������б�
    public List<float> addition;//�ӳ��б�
    public int pointNum;//����Ŀ������

    private int finalAddition(Unit unit)//�������沿��
    {
        if(attribute == null)
            return 0;
        float add;
        int sum=0;
        for(int i=0;i<attribute.Count;i++)
        {
            if(attribute[i] == heroAttribute.Atk)
            {
                add = (float)unit.Atk * addition[i];
                sum = (int)(sum + add);
            }
        }
        return sum;
    }

    public int finalPoint(Unit unit)//����������ֵ����unit�к��������ж�����Ȼ��ִ�ж�Ӧ����
    {
        return baseInt+finalAddition(unit);
    }

}
