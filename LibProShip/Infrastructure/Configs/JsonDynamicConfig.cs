namespace LibProShip.Infrastructure.Configs
{
    public abstract class JsonDynamicConfig : IDynamicConfig
    {
        protected JsonDynamicConfig(string defaultJson)
        {
            
        }
        public string GetConfig(string key)
        {
            throw new System.NotImplementedException();
        }

        public int GetConfigAsInt(string key)
        {
            throw new System.NotImplementedException();
        }

        public float GetConfigAsFloat(string key)
        {
            throw new System.NotImplementedException();
        }

        public void SetConfig(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public void SetConfig(string key, int value)
        {
            throw new System.NotImplementedException();
        }

        public void SetConfig(string key, float value)
        {
            throw new System.NotImplementedException();
        }
    }
}