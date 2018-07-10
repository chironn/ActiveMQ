using ActiveMQ;
using System;
using utilities;

namespace CustomerMQClient
{
    class Program
    {
        static string modeSelect = string.Empty;
        static void Main(string[] args)
        {
            Console.Title = "ACtiveMQ消费者(输入“exit”退出程序)";
            Console.WriteLine("请选择是否保存接收数据至本地？（保存请输入：T）");
            modeSelect = Console.ReadLine();
            Console.WriteLine("ACtiveMQ数据消费开启...");
            MultiQueueTest();

            Console.ReadLine();
        }
        static void MultiQueueTest()
        {
            TestMultiQueue queue = new TestMultiQueue();

            string queueName = System.Configuration.ConfigurationManager.AppSettings["queueName"];
            TestSendAndReceiveMessage(queue, queueName);
        }

        
        private static void TestSendAndReceiveMessage(TestMultiQueue queue, string queueName)
        {
            Console.WriteLine("正在等待数据接入...");
            while (true)
            {
                var msg = queue.ReceiveMessage(queueName);
                if (string.IsNullOrEmpty(msg))
                {
                    continue;
                }
                Console.WriteLine("队列：{0} 收到消息：{1}", queueName, msg);
                if (modeSelect.ToUpper() == "T")
                {
                    WriteLog.WriteDataLog(msg);
                }
               
                System.Threading.Thread.Sleep(1000);
            }
        }

        
        class TestMultiQueue : BaseMultiQueue
        {
            protected override string Topic
            {
                get { return "saic16"; }
            }
        }
       
    }
}
