using Apache.NMS;
using ActiveMQ.Configurations;
using System;
using System.Configuration;

namespace ActiveMQ
{
    public abstract class BaseActiveMQClient
    {
        protected static readonly ActiveMQSection section;
        ///// <summary>
        ///// 缓存IConnection（如果公用一个Connection，并发时无法控制Connection的连接状态，因此不缓存）
        ///// </summary>
        //private static readonly ConcurrentDictionary<string, IConnection> connections = new ConcurrentDictionary<string, IConnection>();

        static BaseActiveMQClient()
        {
            section = ConfigurationManager.GetSection("ActiveMQSection") as ActiveMQSection;
        }

        protected ActiveMQDB ActiveMQDB;
        private IConnectionFactory connectionFactory;
        private object syncRoot = new object();
        /// <summary>
        /// 接收完消息，需要马上Stop，否则会有Apache.NMS.ActiveMQ.IOException: 
        /// Channel was inactive for too long:的异常
        /// </summary>
        protected IConnection connection;
        protected abstract string Topic { get; }

        /// <summary>
        /// 异常计数
        /// </summary>
        private int exceptionCount = 0;
        /// <summary>
        /// 最大异常数量，达到后重置connection
        /// </summary>
        private readonly int maxExceptionCount = 5;

        public BaseActiveMQClient()
        {
            if (section == null)
                throw new ActiveMQException("未找到ActiveMQ配置信息");

            this.ActiveMQDB = section.ActiveMQDBs[Topic];

            if (this.ActiveMQDB == null)
                throw new ActiveMQException(string.Format("未找到配置信息，Topic：{0}", Topic));

            this.connectionFactory = new NMSConnectionFactory(this.ActiveMQDB.Url);

            this.connection = CreateConnection();
        }

        private IConnection CreateConnection()
        {
            IConnection conn = connectionFactory.CreateConnection(this.ActiveMQDB.UserName, this.ActiveMQDB.Password);
            conn.ConnectionInterruptedListener += conn_ConnectionInterruptedListener;
            return conn;
        }

        void conn_ConnectionInterruptedListener()
        {
            ResetConnection();
        }

        private void ResetConnection()
        {
            lock (syncRoot)
            {
                if (this.connection != null)
                {
                    this.connection.Dispose();
                }

                this.connection = CreateConnection();
            }
        }

        protected ISession CreateSession(AcknowledgementMode acknowledgementMode = AcknowledgementMode.AutoAcknowledge)
        {
            try
            {
                if (!connection.IsStarted)
                    connection.Start();

                return this.connection.CreateSession(acknowledgementMode);
            }
            catch (Apache.NMS.ActiveMQ.IOException ex)
            {
                CounterException();
                throw ex;
            }
            catch (Apache.NMS.ActiveMQ.ConnectionClosedException ex)
            {
                CounterException();
                throw ex;
            }
            catch (Exception ex)
            {
                CounterException();
                throw ex;
            }
        }

        private void CounterException()
        {
            System.Threading.Interlocked.Increment(ref exceptionCount);
            if (System.Threading.Interlocked.CompareExchange(ref exceptionCount, 0, maxExceptionCount) == maxExceptionCount)
                ResetConnection();
        }
    }
}
