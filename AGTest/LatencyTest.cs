using System;
namespace AGTest
{
	public class LatencyTest
	{
        TcpClient cl;
        [SetUp] //Inicializa el cliente para cada test, lo hace una vez por test!
        public void Setup()
        {
            if (cl == null)
            {
                cl = new TcpClient();
                cl.Connect("127.0.0.1", 5008);
            }
        }


        //Prueba latencia en el servidor, recomendable testeo en diferente terminal
        [TestCase(1000, TestName = "Test 1 sec latency")]
        [TestCase(500, TestName = "Test 05 sec latency")]
        [TestCase(250, TestName = "Test 025 sec latency")]
        [TestCase(100, TestName = "Test 01 sec latency")]
        [TestCase(50, TestName = "Test 005 sec latency")]
        [TestCase(20, TestName = "Test 002 sec latency")]
        public void LatencyMultipleTest(int msTarget)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            {
                Byte[] data = new Byte[256];
                NetworkStream stream = cl.GetStream();

                while (stream.DataAvailable)
                {
                    stream.Read(data, 0, data.Length); //Limpiando el stream buffer
                }

                data = System.Text.Encoding.ASCII.GetBytes(Headers.GET_VERSION_AG);
                stream.Write(data, 0, data.Length);

                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            }
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            Assert.That(elapsedMs, Is.LessThanOrEqualTo(msTarget), "Slowly");
        }


    }
}

