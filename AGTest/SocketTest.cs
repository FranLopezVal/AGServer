using System.IO;
using AGServer.AG;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace AGTest;

public class Tests
{
    /// <summary>
    /// Las pruebas se deben hacer con una instancia de el servidor AG activa.
    /// </summary>


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

    [Test] //Prueba la conexion y la respuesta del servidor.
    public void ConnectingTest()
    {
        if (cl == null)
        {
            cl = new TcpClient();
            cl.Connect("127.0.0.1", 5008);
        }

        NetworkStream stream = cl.GetStream();

        Byte[] data = new Byte[256];
        String responseData = String.Empty;
        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

        string header = AGController.GetHeader(responseData);
        Assert.That(header, Is.EqualTo("CONNECTED"), "not response connected");
    }

    //Prueba respuesta a cabezeras simples
    [TestCase(Headers.ERR_HEADER,Description ="Test error header",TestName ="Error header")]
    [TestCase(Headers.GET_VERSION_AG, Description = "Test version header",TestName ="Response version")]
    public void MultipleSimpleHeaderResponseTest(string hd)
    {
        Byte[] data = new Byte[256];
        NetworkStream stream = cl.GetStream();

        while (stream.DataAvailable)
        {
            stream.Read(data, 0, data.Length); //Limpiando el stream buffer
        }

        data = System.Text.Encoding.ASCII.GetBytes(hd);
        stream.Write(data, 0, data.Length);

        data = new Byte[256];
        String responseData = String.Empty;
        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

        string header = AGController.GetHeader(responseData);
        Assert.That(header, Is.EqualTo(hd), $"not response like as {hd}");
    }

    //Prueba latencia en el servidor, recomendable testeo en diferente terminal
    [TestCase(1000, TestName = "Test 1 sec latency")]
    [TestCase(500, TestName = "Test 05 sec latency")]
    [TestCase(250, TestName = "Test 025 sec latency")]
    [TestCase(100, TestName = "Test 01 sec latency")]
    [TestCase(50, TestName = "Test 005 sec latency")]
    [TestCase(20, TestName = "Test 002 sec latency")]
    public void LatencyTest(int msTarget)
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


    [Test]//Prueba de modo mantenimiento 
    public void MaintenanceTest()//Esta prueba solo funciona en caso de que el srv este en mantenimiento
    {
        if (cl == null)
        {
            cl = new TcpClient();
            cl.Connect("127.0.0.1", 5008);
        }

        NetworkStream stream = cl.GetStream();
        
        Byte[] data = new Byte[256];
        String responseData = String.Empty;
        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

        string header = AGController.GetHeader(responseData);
        Assert.That(header, Is.EqualTo(Headers.MAINTENANCE), "not response maintenance");
       
    }
}
