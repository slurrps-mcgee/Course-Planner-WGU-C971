using Course_Planner.Data_Models;
using Course_Planner.SQLite_Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Course_Planner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetail : ContentPage
    {
        //Variables
        private Course selectedCourse; //Selected Course
        private Term selectedTerm; //Selected Term
        private SQLiteConnection conn; //SQL Connection

        //Constructor Edit Course
        public CourseDetail(Course course, Term term)
        {
            InitializeComponent();
            selectedTerm = term; //Set selectedTerm to incomming term
            selectedCourse = course; //Set selectedCourse to incomming course
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
        }

        //Constructor New Course
        public CourseDetail(Term term)
        {
            InitializeComponent();
            selectedTerm = term; //Set selectedTerm to incomming term
            selectedCourse = new Course(); //Set selectedCourse to new Course
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection

            EnableUI(); //Enable UI Editing
        }

        //On page appearing
        protected override void OnAppearing()
        {
            //check if selectedCourse courseName is not null
            if(selectedCourse.CourseName != null)
            {
                //Fill Page information
                courseName.Text = selectedCourse.CourseName;
                startDate.Date = selectedCourse.StartDate;
                endDate.Date = selectedCourse.EndDate;
                courseStatus.SelectedItem = selectedCourse.Status;
                txtInstructorName.Text = selectedCourse.InstructorName;
                txtInstructorPhone.Text = PhoneNumberFormatter(selectedCourse.InstructorPhone);
                txtInstructorEmail.Text = selectedCourse.InstructorEmail;
                notificationSwitch.IsToggled = Convert.ToBoolean(selectedCourse.NotificationEnabled);
                notes.Text = selectedCourse.Notes;
            }
            else
            {
                //Set date pickers to current date
                startDate.Date = DateTime.Now;
                endDate.Date = DateTime.Now;
            }
            
            base.OnAppearing();
        }

        //DONE
        #region Events
        //Instructor Phone field TextChanged event
        private void txtInstructorPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set the fields text to the PhoneNumberFormatter method sending the original text
            txtInstructorPhone.Text = PhoneNumberFormatter(txtInstructorPhone.Text);
        }
        #endregion

        //DONE
        #region Buttons
        //Back button
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            //Navigate back a page
            await Navigation.PopModalAsync();
        }

        //Edit button
        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            //Enable UI editing
            EnableUI();
        }

        //Save button
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            //Check that the pages information is not blank
            if(txtInstructorName.Text == null || txtInstructorEmail.Text == null || 
                txtInstructorPhone.Text == null || courseStatus.SelectedItem == null || 
                courseName.Text == null)
            {
                //Error blank information
                await DisplayAlert("Error.", "Please fill in all requested information before saving.", "Ok");
                return;
            }
            else
            {
                //Verify the email is a valid email
                if(!VerifyEmail(txtInstructorEmail.Text))
                {
                    //Error Invalid email
                    await DisplayAlert("Error.", "Invalid email.", "Ok");
                    return;
                }
                else
                {
                    //Check that the start date is before the end date
                    if (startDate.Date > endDate.Date)
                    {
                        //Error Invalid dates
                        await DisplayAlert("Error.", "Course start date cannot be after the end date.", "Ok");
                        return;
                    }
                    else
                    {
                        //Now we check if we are updating or inserting a new course
                        //Check if new or updating
                        if (selectedCourse.CourseName == null)
                        {
                            //New Course Information to Insert

                            //Set Course Information
                            selectedCourse.CourseName = courseName.Text;
                            selectedCourse.StartDate = startDate.Date;
                            selectedCourse.EndDate = endDate.Date;
                            selectedCourse.Status = (string)courseStatus.SelectedItem;
                            selectedCourse.InstructorName = txtInstructorName.Text;
                            selectedCourse.InstructorPhone = txtInstructorPhone.Text;
                            selectedCourse.InstructorEmail = txtInstructorEmail.Text;
                            selectedCourse.NotificationEnabled = Convert.ToInt32(notificationSwitch.IsToggled);
                            selectedCourse.Notes = notes.Text;
                            selectedCourse.Term = selectedTerm.Id;

                            //Insert new course
                            conn.Insert(selectedCourse);
                            //Navigate back a page after inserting a new course
                            await Navigation.PopModalAsync();
                        }
                        else
                        {
                            //Update Course Information

                            //Set Course Information
                            selectedCourse.CourseName = courseName.Text;
                            selectedCourse.StartDate = startDate.Date;
                            selectedCourse.EndDate = endDate.Date;
                            selectedCourse.Status = (string)courseStatus.SelectedItem;
                            selectedCourse.InstructorName = txtInstructorName.Text;
                            selectedCourse.InstructorPhone = txtInstructorPhone.Text;
                            selectedCourse.InstructorEmail = txtInstructorEmail.Text;
                            selectedCourse.NotificationEnabled = Convert.ToInt32(notificationSwitch.IsToggled);
                            selectedCourse.Notes = notes.Text;
                            selectedCourse.Term = selectedTerm.Id;

                            //Update Course
                            conn.Update(selectedCourse);
                            //await Navigation.PopModalAsync();
                        }
                    }
                }
            }
            //Disable the UI for editing
            DisableUI();
        }

        //Bottom Buttons
        //Drop the course
        private async void btnDrop_Clicked(object sender, EventArgs e)
        {
            //Verify Alert for user
            var alert = await DisplayAlert("Alert", $"Are you sure you would like to drop course {selectedCourse.CourseName}", "Yes", "No");
            //Check if Yes
            if (alert)
            {
                //Delete course
                conn.Delete(selectedCourse);
                //Navigate back a page
                await Navigation.PopModalAsync();
            }
        }

        //Share Notes async button
        private async void btnShare_Clicked(object sender, EventArgs e)
        {
            //Provided from https://docs.microsoft.com/en-us/xamarin/essentials/share?tabs=android
            await Share.RequestAsync(new ShareTextRequest
            {
                //Set Text to notes.Text
                Text = notes.Text,
                //Set title
                Title = $"{courseName.Text} course notes"
            });
        }

        //Open Course Assessments
        private async void btnAssessments_Clicked(object sender, EventArgs e)
        {
            //Check if the save button is visable signalling information has not saved
            if(btnSave.IsVisible == true)
            {
                //Error Please save first
                await DisplayAlert("Error.", "Please save the course information first.", "Ok");
                return;
            }
            else
            {
                //DisableUI for editing
                DisableUI();
                //Navigate to new AssessmentsPage sending the selectedCourse
                await Navigation.PushModalAsync(new AssessmentsPage(selectedCourse));
            }
        }
        #endregion

        //DONE
        #region Methods
        //Enables the UI for editing
        private void EnableUI()
        {
            //Set visibility of controls
            btnEdit.IsVisible = false;
            btnSave.IsVisible = true;
            lblEditInfo.IsVisible = true;

            //Make fields editable
            courseName.IsEnabled = true;
            startDate.IsEnabled = true;
            endDate.IsEnabled = true;
            courseStatus.IsEnabled = true;
            txtInstructorName.IsReadOnly = false;
            txtInstructorPhone.IsReadOnly = false;
            txtInstructorEmail.IsReadOnly = false;
            notificationSwitch.IsEnabled = true;
            notes.IsReadOnly = false;
        }
        //Disables the UI for editing
        private void DisableUI()
        {
            //Set visibility of controls
            btnEdit.IsVisible = true;
            btnSave.IsVisible = false;
            lblEditInfo.IsVisible = false;

            //Make fields non editable
            courseName.IsEnabled = false;
            startDate.IsEnabled = false;
            endDate.IsEnabled = false;
            courseStatus.IsEnabled = false;
            txtInstructorName.IsReadOnly = true;
            txtInstructorPhone.IsReadOnly = true;
            txtInstructorEmail.IsReadOnly = true;
            notificationSwitch.IsEnabled = false;
            notes.IsReadOnly = true;
        }

        //Verifies the incoming string if it is an email format
        private bool VerifyEmail(string email)
        {
            //Regex to check for email
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

            //Check incoming string for a match
            Match match = emailRegex.Match(email);

            //Check if match is a success
            if(match.Success)
            {
                //Return true
                return true;
            }
            //Return false
            return false;
        }

        //Format the incomming string and returning it formatted as a phone number
        private string PhoneNumberFormatter(string value)
        {
            //Regex to make sure incomming string is all digits
            value = new Regex(@"\D").Replace(value, string.Empty);

            //Check String length
            if (value.Length > 3 & value.Length < 7)
            {
                //format string
                value = string.Format("{0}-{1}", value.Substring(0, 3), value.Substring(3, value.Length - 3));
                return value; //return string
            }
            //Check String length
            if (value.Length > 6 & value.Length < 11)
            {
                //format string
                value = string.Format("({0})-{1}-{2}", value.Substring(0, 3), value.Substring(3, 3), value.Substring(6));
                return value; //return string
            }
            //Check String length
            if (value.Length > 10)
            {
                //remove 1 character
                value = value.Remove(value.Length - 1, 1);
                //Format string
                value = string.Format("({0})-{1}-{2}", value.Substring(0, 3), value.Substring(3, 3), value.Substring(6));
                return value; //return string
            }
            return value; //return string
        }
        #endregion
    }
}