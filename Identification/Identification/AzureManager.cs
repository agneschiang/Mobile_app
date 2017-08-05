using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identification
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Shapedetail>ShapedetailInformation;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://shapeid.azurewebsites.net");
            this.ShapedetailInformation = this.client.GetTable<Shapedetail>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }
        public async Task<List<Shapedetail>> GetShapeInformation()
        {
            return await this.ShapedetailInformation.ToListAsync();
        }

        public async Task PostShapeInformation(Shapedetail ShapedetailInformation)
        {
            await this.ShapedetailInformation.InsertAsync(ShapedetailInformation);
        }
    }
}
