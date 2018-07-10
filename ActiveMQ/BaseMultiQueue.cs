using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Newtonsoft.Json;
using System;

namespace ActiveMQ
{
    /// <summary>
    /// 多队列
    /// </summary>
    public abstract class BaseMultiQueue : BaseActiveMQClient
    {
        /// <summary>
        /// 默认构造函数：接收消息时超时时间为2s
        /// </summary>
        public BaseMultiQueue()
            : base()
        {
        }

        /// <summary>
        /// 传入 接收消息的超时时间
        /// </summary>
        /// <param name="timeOut"></param>
        public BaseMultiQueue(TimeSpan timeOut)
            : this()
        {
            this.timeOut = timeOut;
        }

        private TimeSpan timeOut = new TimeSpan(0, 0, 2);

        public string ReceiveMessage(string queueName)
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;

                if (message != null)
                    return message.Text;
            }
            connection.Stop();
            return null;
        }

        public T ReceiveMessage<T>(string queueName) where T : class
        {
            T t;
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    t = default(T);
                else
                    t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Text);
            }
            connection.Stop();
            return t;
        }

        public T ReceiveMessage<T>(string queueName, out string originMessage) where T : class
        {
            originMessage = null;

            using (ISession session = CreateSession(AcknowledgementMode.ClientAcknowledge))
            {
                var queue = session.GetQueue(queueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    return default(T);

                originMessage = message.Text;
                try
                {
                    T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Text);
                    message.Acknowledge();
                    return t;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Stop();
                }
            }
        }

        public void SendTextMessage<T>(string queueName, params T[] msgs) where T : class
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var producer = session.CreateProducer(queue);

                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                foreach (var item in msgs)
                {
                    producer.Send(new ActiveMQTextMessage(JsonConvert.SerializeObject(item, serializerSettings)));
                }
            }
            connection.Stop();
        }

        public void SendTextMessage(string queueName, params string[] msgs)
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var producer = session.CreateProducer(queue);

                foreach (var msg in msgs)
                {
                    producer.Send(new ActiveMQTextMessage(msg));
                }
            }
            connection.Stop();
        }
    }
}
