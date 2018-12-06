namespace LibProShip.Infrastructure.Configs
{
    public interface IDynamicConfig
    {
        string GetConfig(string key);
        int GetConfigAsInt(string key);
        float GetConfigAsFloat(string key);
        void SetConfig(string key, string value);
        void SetConfig(string key, int value);
        void SetConfig(string key, float value);
    }
}