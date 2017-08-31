using API_MobileUser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace API_MobileUser
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

       

        private async void Login_Clicked(object sender, EventArgs e)
        {
            this.IsBusy = true;
            HttpClient client = new HttpClient();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            
            parameters.Add("grant_type", "password");
            parameters.Add("username", UserName.Text);
            parameters.Add("password", Password.Text);

            try
            {
                HttpResponseMessage result = await client.PostAsync(Constants.LOGIN_API, new FormUrlEncodedContent(parameters));

                if (result.IsSuccessStatusCode)
                {
                    string jsonResult = await result.Content.ReadAsStringAsync();
                    // TokenResult is a custom model class for deserialization of the Token Endpoint
                    // Be sure to include Newtonsoft.Json from NuGet
                    var resultObject = JsonConvert.DeserializeObject<TokenModel>(jsonResult);
                    App.Token = "Bearer " + resultObject.Access_Token;

                    client.DefaultRequestHeaders.Add("Authorization", App.Token);
                    var profile = await client.GetAsync(Constants.USER_PROFILE_API);

                    var jsonProfile = await profile.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserInfoViewModel>(jsonProfile);

                    App.CurrentUser = user;

                    await Navigation.PushModalAsync(new UserProfilePage(user));


                }
                else
                {
                    MessagingCenter.Send(this, "Login failed");
                }

                this.IsBusy = false;

            }
            catch (Exception ex)
            {
                string debugBreak = ex.ToString();
            }
        }

        private async void SignUp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegisterPage());

        }


    }
}
