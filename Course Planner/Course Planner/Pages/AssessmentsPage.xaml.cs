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
    public partial class AssessmentsPage : ContentPage
    {
        //Variables
        private Course selectedCourse; //Holds the incomming selected course
        private Assessment selectedAssessment; //Holds the assessment to be selected from the list
        private SQLiteConnection conn; //SQL Connection
        private ObservableCollection<Assessment> assessmentList; //Holds a list of the assessments for the selected course

        //Constructor
        public AssessmentsPage(Course course)
        {
            InitializeComponent();
            selectedCourse = course; //Set selected course to incomming course
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
        }

        //Handles page appearing
        protected override void OnAppearing()
        {
            txtCourseTitle.Text = selectedCourse.CourseName; //Set the title to the course name

            //Fill the assessment list with the query as an observable collection
            assessmentList = new ObservableCollection<Assessment>(conn.Query<Assessment>($"Select * from Assessments where Course = '{selectedCourse.Id}' Order By StartDate"));

            //Set assessmentListView itemsource to the observable collection assessmentList
            assessmentListView.ItemsSource = assessmentList;
            base.OnAppearing();
        }

        #region Events
        //ListView itemtapped event
        private async void assessmentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //Set selected assessment to a explicit cast of e.item
            selectedAssessment = (Assessment)e.Item;
            //Navigate to a new assessmentDetail page sending the selected couse and selected assessment
            await Navigation.PushModalAsync(new AssessmentsDetail(selectedCourse, selectedAssessment));
        }
        #endregion

        #region Buttons
        //Back button
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            //Navigate back a page
            await Navigation.PopModalAsync();
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            //Check that there is not two assessments
            if(assessmentList.Count >= 2)
            {
                //Error Assessment limit is 2 per course
                await DisplayAlert("Error.", "Assessment limit per course is 2 please drop an assement before adding a new one.", "Ok");
                return;
            }

            //Navigate to a new AssessmentDetail page sending it only the selected course
            await Navigation.PushModalAsync(new AssessmentsDetail(selectedCourse));
        }
        #endregion
    }
}