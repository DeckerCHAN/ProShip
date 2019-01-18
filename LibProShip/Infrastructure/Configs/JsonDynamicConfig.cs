using System;
using System.IO;

namespace LibProShip.Infrastructure.Configs
{
    public abstract class JsonDynamicConfig : IDynamicConfig
    {
        protected JsonDynamicConfig(string defaultJson)
        {
        }

        protected JsonDynamicConfig(Stream defaultJsonStream)
        {
        }

        public string GetConfig(string key)
        {
            throw new NotImplementedException();
        }

        public int GetConfigAsInt(string key)
        {
            throw new NotImplementedException();
        }

        public float GetConfigAsFloat(string key)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(string key, int value)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(string key, float value)
        {
            throw new NotImplementedException();
        }
    }
}