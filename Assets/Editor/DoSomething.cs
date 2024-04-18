using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HSBCBank))]
public class DoSomething : Editor
{
    private int id = 0;
    private int amount = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        id = EditorGUILayout.IntField("ID: ", id);
        amount = EditorGUILayout.IntField("Amount: ", amount);

        GUILayout.Space(10);

        if (GUILayout.Button("Deposit"))
        {
            Deposit(id, amount);
        }
        GUILayout.Space(10);

        if (GUILayout.Button("Withdraw"))
        {
            Withdraw(id, amount);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Check Balance"))
        {
            CheckBalance(id);
        }
    }

    private void Deposit(int id, int amount)
    {
        var result = string.Empty;
        HSBCBank targetComponent = (HSBCBank)target;
        targetComponent.Deposit(id, amount, out result);
        Debug.Log(result);
    }
    private void Withdraw(int id, int amount)
    {
        var result = string.Empty;
        HSBCBank targetComponent = (HSBCBank)target;
        targetComponent.Withdraw(id, amount, out result);
        Debug.Log(result);
    }
    private void CheckBalance(int id)
    {
        var result = string.Empty;
        HSBCBank targetComponent = (HSBCBank)target;
        targetComponent.Checkbalance(id, out result);
        Debug.Log(result);
    }
}