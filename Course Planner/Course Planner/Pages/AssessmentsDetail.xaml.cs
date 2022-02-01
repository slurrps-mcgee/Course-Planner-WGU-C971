using Course_Planner.Data_Models;
using Course_Planner.SQLite_Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Course_Planner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentsDetail : ContentPage
    {
        //Variables 
        private Course selectedCourse; //Holds current selected course
        private Assessment selectedAssessment; //Holds current selected assessment
        private SQLiteConnection conn; //SQL Connection

        //Constructors
        //Edit Assessment
        public AssessmentsDetail(Course course, Assessment assessment)
        {
            InitializeComponent();
            selectedCourse = course; //Set selected course to incomming course
            selectedAssessment = assessment; //Set selected assessment to incomming assessment
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
        }
        //New Assessment
        public AssessmentsDetail(Course course)
        {
            InitializeComponent();
            selectedCourse = course; //Set selected course to incomming course
            selectedAssessment = new Assessment(); //Set selected assessment to new instance of assessment
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
            EnableUI(); //Enable UI for editing on New Assessment
        }

        //Handles page appearing
        protected override void OnAppearing()
        {
            //Check if the selected assessment Title is not null
            if(selectedAssessment.Title != null)
            {
                //If so fill out the pages information of the selected assessment

                txtAssessmentTitle.Text = selectedAssessment.Title;
                startDate.Date = selectedAssessment.StartDate;
                endDate.Date = selectedAssessment.EndDate;
                assessmentType.SelectedItem = selectedAssessment.Type;
                notificationSwitch.IsToggled = Convert.ToBoolean(selectedAssessment.NotificationEnabled);
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
        #region Buttons
        //Back Button
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            //Disable UI to stop editing
            DisableUI();
            //Navigate back a page
            await Navigation.PopModalAsync();
            
        }

        //Edit Button
        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            //Enable UI for editing
            EnableUI();
        }

        //Save Assessment Async
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            //Save Assessment new or update
            if(txtAssessmentTitle.Text == null || assessmentType.SelectedItem == null)
            {
                //Error Title cannot be blank
                await DisplayAlert("Error.", "Assessment title and type cannot be left blank.", "Ok");
                return;
            }
            else
            {
                //Check if the selected Assessment Type already exists
                if (selectedAssessment.Type != assessmentType.SelectedItem.ToString() && CheckAssessmentType(assessmentType.SelectedItem.ToString()) == 1)
                {
                    //Error Assessment of Type selected already exists
                    await DisplayAlert("Error.", $"Assessment of type {assessmentType.SelectedItem.ToString()} already exists! Please select another type.", "Ok");
                    return;
                }
                else
                {
                    //Check if new or updating by checking if the selected assessment Title is null
                    if (selectedAssessment.Title == null)
                    {
                        //New Course Insert
                        selectedAssessment.Title = txtAssessmentTitle.Text;
                        selectedAssessment.StartDate = startDate.Date;
                        selectedAssessment.EndDate = endDate.Date;
                        selectedAssessment.Type = assessmentType.SelectedItem.ToString();
                        selectedAssessment.NotificationEnabled = Convert.ToInt32(notificationSwitch.IsToggled);
                        selectedAssessment.Course = selectedCourse.Id;

                        conn.Insert(selectedAssessment);
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        //Update course
                        selectedAssessment.Title = txtAssessmentTitle.Text;
                        selectedAssessment.StartDate = startDate.Date;
                        selectedAssessment.EndDate = endDate.Date;
                        selectedAssessment.Type = assessmentType.SelectedItem.ToString();
                        selectedAssessment.NotificationEnabled = Convert.ToInt32(notificationSwitch.IsToggled);
                        selectedAssessment.Course = selectedCourse.Id;

                        conn.Update(selectedAssessment);
                        //await Navigation.PopModalAsync();
                    }
                }
            }
            //Disable the UI for editing
            DisableUI();
        }

        //Drop Assessment async button
        private async void btnDrop_Clicked(object sender, EventArgs e)
        {
            //Alert to user to verify they want to delete the assessment
            var alert = await DisplayAlert("Alert", $"Are you sure you would like to drop assessment {selectedAssessment.Title}", "Yes", "No");
            //Check answser
            if (alert)
            {
                //Delete assessment
                conn.Delete(selectedAssessment);
                //Navigate back a page
                await Navigation.PopModalAsync();
            }
        }
        #endregion

        //DONE
        #region Method
        //Enables UI for editing
        private void EnableUI()
        {
            //Set visibility
            btnEdit.IsVisible = false;
            btnSave.IsVisible = true;

            //Set enabled
            txtAssessmentTitle.IsEnabled = true;
            startDate.IsEnabled = true;
            endDate.IsEnabled = true;
            assessmentType.IsEnabled = true;
            notificationSwitch.IsEnabled = true;
        }

        //Disables UI so no edits can be made
        private void DisableUI()
        {
            //Set visibility
            btnEdit.IsVisible = true;
            btnSave.IsVisible = false;

            //Set enabled
            txtAssessmentTitle.IsEnabled = false;
            startDate.IsEnabled = false;
            endDate.IsEnabled = false;
            assessmentType.IsEnabled = false;
            notificationSwitch.IsEnabled = false;
        }

        //Checks if an assessment of that type already exists in the selected course
        private int CheckAssessmentType(string type)
        {
            //Try catch to check if the row exists
            try
            {
                //Exists
                conn.Query<int>($"Select * from Assessments where Course = '{selectedCourse.Id}' and Type = '{type}'").Single();
                return 1;
            }
            catch
            {
                //Does not exist
                return 0;
            }
        }
        #endregion
    }
}