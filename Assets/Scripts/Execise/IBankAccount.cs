public interface IBankAccount
{
    public bool Deposit(int id, int amount, out string result);
    public bool Withdraw(int id, int amount, out string result);
    public bool Checkbalance(int id, out string result);
}
