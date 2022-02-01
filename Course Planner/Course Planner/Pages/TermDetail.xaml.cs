using Course_Planner.Data_Models;
using Course_Planner.SQLite_Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Course_Planner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermDetail : ContentPage
    {
        //Variables
        private Term selectedTerm; //Term
        private SQLiteConnection conn; //SQL Connection
        private ObservableCollection<Course> courselist; //CourseList

        //Constructor
        public TermDetail(Term term)
        {
            InitializeComponent();
            selectedTerm = term; //Set selected term to incomming term
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
        }

        //On page appearing
        protected override void OnAppearing()
        {
            //Fill the Term Details
            txtTermTitle.Text = selectedTerm.Title;
            startDate.Date = selectedTerm.StartDate;
            endDate.Date = selectedTerm.EndDate;

            //Get the courseList from the selectedTerm
            courselist = new ObservableCollection<Course>(conn.Query<Course>($"Select * from Courses where term = '{selectedTerm.Id}' Order By StartDate"));

            courseListView.ItemsSource = courselist; //Set courseListView.ItemSource to courseList
            base.OnAppearing();
        }

        #region Events
        //CourseListView Items Tap Event
        private async void courseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (btnSave.IsVisible == true)
            {
                //Error Please information not saved
                await DisplayAlert("Error.", "Please save the course information first.", "Ok");
                return;
            }
            else
            {
                //Create a new course andset it to implicitly cast e.Item
                Course selectedCourse = (Course)e.Item;
                //Disable the UI for editing
                DisableUI();

                //Navigate to a new CourseDetail sending selectedCourse, and selectedTerm
                await Navigation.PushModalAsync(new CourseDetail(selectedCourse, selectedTerm));
            }
        }
        #endregion

        #region Buttons
        //Back Button
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            //Navigate back a page
            await Navigation.PopModalAsync();
        }

        //Drop Term
        private async void btnDrop_Clicked(object sender, EventArgs e)
        {
            //Alert User
            var alert = await DisplayAlert("Alert", $"Are you sure you would like to drop term {selectedTerm.Title} this will delete the courses and assessments associated with this term as well.", "Yes", "No");
            //Check Yes
            if(alert)
            {
                //Delete selectedTerm
                conn.Delete(selectedTerm);

                //Delete all information pertaining to the selected term
                //Delete Selected Term Courses
                //Loop through courses
                foreach (Course course in courselist)
                {
                    //Create a list of the courses assessments
                    ObservableCollection<Assessment> assessmentList = new ObservableCollection<Assessment>(conn.Query<Assessment>($"Select * from Assessments where Course = '{course.Id}'"));
                    //Loop through assessments
                    foreach(Assessment assessment in assessmentList)
                    {
                        //Delete Assessment
                        conn.Delete(assessment);
                    }
                    //Delete course
                    conn.Delete(course);
                }
                //Delete Selected Term Assessments

                //Navigate back a page
                await Navigation.PopModalAsync();
            }
        }

        //Add Button
        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            //Check if courseList is = 6
            if(courselist.Count == 6)
            {
                //Error course limit is 6 per term
                await DisplayAlert("Error.", "Course limit is 6 per term, please drop a course before adding a new one.", "Ok");
            }
            else
            {
                //Navigate to a new courseDetail page sending the selected term
                await Navigation.PushModalAsync(new CourseDetail(selectedTerm));
            }
        }
        
        //Edit button
        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            //Enable UI for editing
            EnableUI();
        }

        //Save Term
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            //Check if the term is null
            if(txtTermTitle.Text == null)
            {
                //Error title cannot be null
                await DisplayAlert("Error.", "Title cannot be left blank!", "Ok");
                return;
            }
            else
            {
                //Check that start date is not after end date
                if(startDate.Date > endDate.Date)
                {
                    //Error invalid dates
                    await DisplayAlert("Error.", "Start date cannot be after the End date!", "Ok");
                    return;
                }
                else
                {
                    //Set the terms information
                    selectedTerm.Title = txtTermTitle.Text;
                    selectedTerm.StartDate = startDate.Date;
                    selectedTerm.EndDate = endDate.Date;

                    //Update the term
                    conn.Update(selectedTerm);
                }
            }
            //Disable the UI to prevent editing
            DisableUI();
        }
        #endregion

        #region Methods
        //Enable the UI
        private void EnableUI()
        {
            //Set visability
            lblEditInfo.IsVisible = true;
            btnEdit.IsVisible = false;
            btnSave.IsVisible = true;

            //Set enabled
            startDate.IsEnabled = true;
            endDate.IsEnabled = true;
            txtTermTitle.IsEnabled = true;
        }

        //Disable the UI
        private void DisableUI()
        {
            //Set visability
            lblEditInfo.IsVisible = false;
            btnEdit.IsVisible = true;
            btnSave.IsVisible = false;

            //Set enabled
            startDate.IsEnabled = false;
            endDate.IsEnabled = false;
            txtTermTitle.IsEnabled = false;
        }
        #endregion
    }
}