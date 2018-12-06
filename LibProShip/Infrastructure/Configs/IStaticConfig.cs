namespace LibProShip.Infrastructure.Configs
{
    public interface IStaticConfig
    {
        string GetConfig(string key);
        int GetConfigAsInt(string key);
        float GetConfigAsFloat(string key);
    }
}