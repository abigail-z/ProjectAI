public abstract class Decision<Client, ReturnType>
{
    public abstract ReturnType Evaluate(Client client);
}

public class DecisionQuery<Client, ReturnType> : Decision<Client, ReturnType>
{
    public Decision<Client, ReturnType> positive;
    public Decision<Client, ReturnType> negative;
    public DecisionTest Test { get; set; }

    public override ReturnType Evaluate(Client client)
    {
        bool result = Test(client);

        if (result)
        {
            return positive.Evaluate(client);
        }
        else
        {
            return negative.Evaluate(client);
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
