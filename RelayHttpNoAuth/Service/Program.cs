
namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.ServiceModel.Web;
    using System.IO;
    using System.Threading.Tasks;
    using System.Drawing.Imaging;
    using System.Drawing;

    [ServiceContract]
    class Program : IHttpListenerSample
    {
        static Image bitmap = System.Drawing.Image.FromFile("image.jpg");

        public async Task Run(string httpAddress, string listenToken)
        {
            using (var host = new WebServiceHost(GetType()))
            {
                host.AddServiceEndpoint(GetType(), 
                        new WebHttpRelayBinding(
                            EndToEndWebHttpSecurityMode.None, 
                            RelayClientAuthenticationType.None), httpAddress)
                    .EndpointBehaviors.Add(
                        new TransportClientEndpointBehavior(
                            TokenProvider.CreateSharedAccessSignatureTokenProvider(listenToken)));

                host.Open();

                Console.WriteLine("Starting a browser to see the image: ");
                Console.WriteLine(httpAddress + "/Image");
                Console.WriteLine();
                // launching the browser
                System.Diagnostics.Process.Start(httpAddress + "/Image");
                Console.WriteLine("Press [Enter] to exit");
                Console.ReadLine();

                host.Close();
            }
        }

        [OperationContract, WebGet]
        Stream Image()
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
            return stream;
        }
    }
}