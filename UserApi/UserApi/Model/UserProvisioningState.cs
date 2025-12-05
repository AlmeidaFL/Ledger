namespace UserApi.Model;

public class UserProvisioningState
{
    public Guid UserId { get; set; }

    public bool UserCreatedReceived { get; set; }
    public bool FinancialAccountCreatedReceived { get; set; }
    public uint RowVersion { get; set; }
}