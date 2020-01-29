using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DBTogRPC.XamarinForms.Models;
using DBTogRPC.XamarinForms.Views;
using DBTogRPC.XamarinForms.ViewModels;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace DBTogRPC.XamarinForms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);


            var channel = new Channel("127.0.0.1:15000", ChannelCredentials.Insecure);

            var dbTogRPCServiceClient = new DBTogRPCService.DBTogRPCServiceClient(channel);

            var personKey = new Guid("07b10373-0487-4281-b768-81fdc48c0318");

            var addressKey = new Guid("669c71bf-7c4e-4536-9642-bce10f22b7bd");

            //Save the Person on the server
            var reply = dbTogRPCServiceClient.Save(
                new SaveRequest
                {
                    TypeName = "Person",
                    DTO =
                    Any.Pack
                    (
                        new Person
                        {
                            PersonKey = personKey.ToString(),
                            FirstName = "you",
                            BillingAddress = new Address
                            {
                                AddressKey = addressKey.ToString()
                            }

                        }
                   )
                });

            //Load the Person from the server
            var any = dbTogRPCServiceClient.Get(new DTORequest { TypeName = "Person", KeyValue = personKey.ToString() });
            var person = any.Unpack<Person>();

            await DisplayAlert("First Name", person.FirstName, "OK");
        }
    }
}