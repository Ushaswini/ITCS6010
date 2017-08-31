using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace API_MobileUser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {

            var user = new Models.RegisterBindingModel
            {
                Name = Name.Text,
                Age = Age.Text,
                Weight=Weight.Text,
                Address=Address.Text,
                Email= Email.Text,
                Password=Password.Text,
                ConfirmPassword=ConfirmPassword.Text

            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", App.Token);

            HttpResponseMessage result = await client.PostAsync(Constants.REGISTER_API, new FormUrlEncodedContent(user.ToMap()));

            var jsonResult = await result.Content.ReadAsStringAsync();
            //TODO Navigate to login page
        }
    }
}