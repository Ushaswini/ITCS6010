using API_MobileUser.Models;
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
    public partial class EditProfilePage : ContentPage
    {

        UserInfoViewModel user;
        public EditProfilePage(UserInfoViewModel user)
        {
            InitializeComponent();
            this.user = user;
            InitilizeData();
        }

        private void InitilizeData()
        {
            Name.Text = user.Name;
            Address.Text = user.Address;
            Age.Text = user.Age;
            Weight.Text = user.Weight;
            Email.Text = user.Email;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            // save in settings as well

            var user = new Models.UserInfoViewModel
            {
                Name = Name.Text,
                Age = Age.Text,
                Weight = Weight.Text,
                Address = Address.Text,
                Email = Email.Text,
                

            };

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", App.Token);

                HttpResponseMessage result = await client.PostAsync(Constants.UPDATE_PROFILE_API, new FormUrlEncodedContent(user.ToMap()));

                if (result.IsSuccessStatusCode)
                {
                    //var jsonResult = await result.Content.ReadAsStringAsync();
                    //var editedUser = JsonConvert.DeserializeObject<UserInfoViewModel>(jsonResult);

                    App.CurrentUser = user;
                   // await Navigation.PushModalAsync(new UserProfilePage(App.CurrentUser));
                    await App.Navigation.PopModalAsync();
                }
                else
                {
                    MessagingCenter.Send(this, "Saving failed");
                }
            }catch(Exception exp)
            {
               
            }

            
            
        }
    }
}