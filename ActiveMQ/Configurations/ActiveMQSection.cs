using System.Configuration;

namespace ActiveMQ.Configurations
{
    public sealed class ActiveMQSection : ConfigurationSection
    {
        [ConfigurationProperty("ActiveMQDBs")]
        [ConfigurationCollection(typeof(ActiveMQDB), AddItemName = "ActiveMQDB")]
        public ActiveMQDBCollection ActiveMQDBs
        {
            get
            { return (ActiveMQDBCollection)this["ActiveMQDBs"]; }
            set
            { this["ActiveMQDBs"] = value; }
        }

        public ActiveMQDB GetDB(string Topic)
        {
            return this.ActiveMQDBs[Topic];
        }
    }

    public class ActiveMQDBCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ActiveMQDB();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ActiveMQDB)element).Topic;
        }

        public new ActiveMQDB this[string Topic]
        {
            get { return (ActiveMQDB)BaseGet(Topic); }
        }
    }

    public class ActiveMQDB : ConfigurationElement
    {
        [ConfigurationProperty("Topic", IsRequired = true)]
        public string Topic
        {
            get
            {
                return this["Topic"].ToString();
            }
            set
            {
                this["Topic"] = value;
            }
        }

        [ConfigurationProperty("Url", IsRequired = true)]
        public string Url
        {
            get
            {
                return this["Url"].ToString();
            }
            set
            {
                this["Url"] = value;
            }
        }

        [ConfigurationProperty("UserName")]
        public string UserName
        {
            get
            {
                return this["UserName"].ToString();
            }
            set
            {
                this["UserName"] = value;
            }
        }


        [ConfigurationProperty("Password")]
        public string Password
        {
            get
            {
                return this["Password"].ToString();
            }
            set
            {
                this["Password"] = value;
            }
        }
    }

}
