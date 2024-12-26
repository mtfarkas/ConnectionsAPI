namespace ConnectionsAPI.Utility;
public static class EnvironmentUtility
{
    public static bool IsContainer =>
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}
