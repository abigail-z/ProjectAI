public abstract class Decision<Client, T>
{
    public abstract T Evaluate(Client client);
}

public class DecisionQuery<Client, ReturnType> : Decision<Client, ReturnType>
{
    public Decision<Client, ReturnType> Positive { get; set; }
    public Decision<Client, ReturnType> Negative { get; set; }
    public DecisionTest Test { get; set; }

    public override ReturnType Evaluate(Client client)
    {
        bool result = Test(client);

        if (result)
        {
            return Positive.Evaluate(client);
        }
        else
        {
            return Negative.Evaluate(client);
        }
    }

    public delegate bool DecisionTest(Client client);
}

public class DecisionResult<Client, ReturnType> : Decision<Client, ReturnType>
{
    public DecisionResult(ReturnType result)
    {
        this.result = result;
    }
    private readonly ReturnType result;
    public override ReturnType Evaluate(Client client)
    {
        return result;
    }
}
