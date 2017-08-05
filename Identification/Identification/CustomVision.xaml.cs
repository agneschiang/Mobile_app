using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Identification.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Identification
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            TagLabel.Text = "";
            PredictionLabel.Text = "";
            Colour.Text = "";

            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "0c1fe7a71c62415684f9bd5d37d7541a");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/5dbc398e-f411-40eb-be83-3e50d121b31c/image?iterationId=7efce6c6-d54c-4194-b0cf-e9508c10e3e6";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    JObject rss = JObject.Parse(responseString);

                    //Querying with LINQ
                    //Get all Prediction Values
                    var Probability = from p in rss["Predictions"] select (int)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];
                    List<string> list = new List<string>();


                    //Truncate values to labels in XAML
                    foreach (var item in Tag)
                    {
                        list.Add(item);
                        TagLabel.Text += item + ": \n";
                    }

                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += item + "\n";
                    }

                    Shapedetail model = new Shapedetail()
                    {
                        Tag = list[0]
                    };

                    await AzureManager.AzureManagerInstance.PostShapeInformation(model);


                    //EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    //double max = responseModel.Predictions.Max(m => m.Probability);

                    //TagLabel.Text = (max >= 0.5) ? "Shape" : "Not a Shape";

                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}