namespace ServiceCommons.Utils;

public static class OwnershipUtils
{
    public static bool HasOwnership(Guid userId, Guid requestingUserId)
    {
        return userId == requestingUserId;
    }
}