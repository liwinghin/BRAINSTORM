using System.Collections.Generic;
using UnityEngine;

public class HSBCBank : MonoBehaviour, IBankAccount
{
    public Dictionary<int, int> accounts;

    private void Start()
    {
        accounts = new Dictionary<int, int>();
        accounts.TryAdd(0, 100);
        accounts.TryAdd(1, 200);
        accounts.TryAdd(2, 300);
        accounts.TryAdd(3, 400);
        accounts.TryAdd(4, 500);
    }

    public bool Deposit(int id, int amount, out string result)
    {
        result = string.Empty;

        if (amount <= 0)
        {
            result = "存款金額必須大於0";
            return false;
        }
        if (accounts.ContainsKey(id))
        {
            accounts[id] += amount;
            result = $"帳戶餘額: {accounts[id]}";
            return true;
        }
        else
        {
            result = "找不到帳戶";
            return false;
        }
    }
    public bool Withdraw(int id, int amount, out string result)
    {
        result = string.Empty;

        if (amount <= 0)
        {
            result = "提款金額必須大於0";
            return false;
        }
        if (accounts.ContainsKey(id))
        {
            if((accounts[id] - amount) >= 0)
            {
                accounts[id] -= amount;
                result = $"帳戶餘額: {accounts[id]}";
                return true;
            }
            else
            {
                result = "餘額不足";
                return false;
            }
        }
        else
        {
            result = "找不到帳戶";
            return false;
        }
    }
    public bool Checkbalance(int id, out string result)
    {
        result = string.Empty;

        if (accounts.ContainsKey(id))
        {
            result = $"帳戶餘額: {accounts[id]}";
            return true;
        }
        else
        {
            result = "找不到帳戶";
            return false;
        }
    }
}