using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Popups;
using pc_mdm.Classes;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace pc_mdm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class enrolment_step22 : Page
    {
        public enrolment_step22()
        {
            this.InitializeComponent(); 
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var enrol         = (enrolment)e.Parameter;
            prefirstname.Text = enrol.firstname;
            prelastname.Text  = enrol.lastname;
            preemail.Text     = enrol.email;
            title.Text        = enrol.title;
        }

        private async void join_family_member(object sender, RoutedEventArgs e)
        {          
            members members_details = new members();
            request request_details = new request();
            var azure_data = new azure_script();
            request_details.Id = "2";
            

            if (group_name.Text != "")
            {
                string thefamily = group_name.Text;
                var total_members = members_details.Readmembers();
                //  var member1 = members_details.Readthemembers(thefamily);  // check group name locally
                members_details.DeleteAllmembers();                           // Erasing the current database
                request_details.DeleteAllrequest();
                string title1    = title.Text;
                var themember = new members(prefirstname.Text, prelastname.Text, preemail.Text, thefamily, " ", "normal", " ", " ", 0, 0, "at home", "on", title1);
                // last member inserted 
                themember.Id = "2";
                themember.members_family_name = thefamily;
                azure_data.lastmembersid(themember);
                azure_data.lastrequestsid(request_details); 

                // Check data in the cloud               
                azure_data.check_family(themember);  // check if the family name exists in the cloud 
                ContentDialog cd = new ContentDialog();
                cd.Title = "Azure";
                cd.Content = "The data are being processed.  Wait... ";
                var t1 = cd.ShowAsync();
                await Task.Delay(TimeSpan.FromSeconds(15));
                t1.Cancel();


                //MessageDialog messageDialog1 = new MessageDialog(themember1.Id + "==" + request_details.Id);//Text should not be empty    
               // await messageDialog1.ShowAsync();
                
                if (themember.members_family_name.Length == 0)
                {
                   
                    themember.members_family_name = thefamily; 
                   // int last_member_id            = members_details.Insert(themember); // create the new space and clear the old's request.  
                    members_details.Insert(themember); // create the new space and clear the old's request.  
                    //insertion of the member in to the server.  
                    azure_data.Insertmembers(themember);

                    var myrequest2 = new request(int.Parse(themember.Id), "all", "Welcome " + prefirstname.Text.Trim(), DateTime.Now.ToString(), 1, "welcome", 0, "", 0, 0, thefamily);
                    //  taking the last request id    
                    myrequest2.Id = request_details.Id;                  

                    myrequest2.Insert(myrequest2);
                    //insertion of the first request in to the server.                      
                    azure_data.Insertrequests(myrequest2);

                    var myrequest1 = new request(int.Parse(themember.Id), "all", prefirstname.Text.Trim() + ": would like to join the  group", DateTime.Now.ToString(), 0, "join", 0, "", 0, 0, thefamily);
                    myrequest1.Id = (int.Parse(request_details.Id) + 1).ToString();

                    // MessageDialog messageDialog = new MessageDialog(myrequest2.Id + "==" + myrequest1.Id);//Text should not be empty    
                    // await messageDialog.ShowAsync();
                    myrequest1.Insert(myrequest1);  // insert request to be in the family 
                
                    //insertion of the second request in to the server.                      
                    azure_data.Insertrequests(myrequest1);

                    if (total_members.Count == 0)
                    {
                        ApplicationState.SetValue("userid", int.Parse(themember.Id));
                        Frame.Navigate(typeof(start1));//after add contact redirect to contact listbox page  
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Start again...");//Text should not be empty    
                        await messageDialog.ShowAsync();
                        Application.Current.Exit();
                    }
                      
                }
                else
                {
                    MessageDialog messageDialog = new MessageDialog("Please enter a valid  name");//Text should not be empty    
                    await messageDialog.ShowAsync();
                }    
                
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Please enter a valid  name");//Text should not be empty    
                await messageDialog.ShowAsync();
            }          
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
