using Course_Planner.Data_Models;
using Course_Planner.Pages;
using Course_Planner.SQLite_Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.LocalNotifications;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Course_Planner
{
    public partial class MainPage : ContentPage
    {
        //Variables
        private SQLiteConnection conn; //SQL Connection

        //Lists
        private ObservableCollection<Term> termList; //Terms Lists
        private List<Course> courseList; //Couse Lists
        private ObservableCollection<Assessment> assessmentList; //Assessment Lists

        //Constructor
        public MainPage()
        {
            InitializeComponent();

            conn = DependencyService.Get<Isqlite>().GetConnection(); //Set SQL Connection
            InitializeDB(); //Initialize the database
            PushNotifications(); //Check and send push notifications
        }

        //Handles OnApearing of the page
        protected override void OnAppearing()
        {
            termList = new ObservableCollection<Term>(conn.Table<Term>().ToList()); //Get list of Terms

            //########################################
            //Dummy Data if there is no data in the DB
            //TESTING Purposes
            if (!termList.Any())
            {
                DummyData();
            }
            //########################################


            termListView.ItemsSource = termList; //Set the listview datasource to the termList list
            base.OnAppearing();
        }

        #region Buttons
        //Go to Term Add Page
        private async void btnAddTerm_Clicked(object sender, EventArgs e)
        {
            //Navigate to a new TermAdd page
            await Navigation.PushModalAsync(new TermAdd());
        }
        #endregion

        #region Events
        //Handles Items in list being tapped
        private async void termListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //Create a new Term called term and set it to a implicit cast of (Term)e.item
            Term term = (Term)e.Item;
            //Navigate to a new TermDetail window sending in the term variable
            await Navigation.PushModalAsync(new TermDetail(term));
        }
        #endregion

        #region Methods
        //Create Tables
        private void InitializeDB()
        {
            //Create the Tables for the SQLite DB if they do not exist
            conn.CreateTable<Term>();
            conn.CreateTable<Course>();
            conn.CreateTable<Assessment>();
        }

        //Send Push Notification to user 
        //https://github.com/spencermswift/Course-Planner as template
        private void PushNotifications()
        {
            //Fill Courses
            courseList = conn.Table<Course>().ToList();

            //Counts the notifications
            int notifications = 0;
            //Foreach to loop through the course list
            foreach (Course course in courseList)
            {
                //Check if notifications are enabled for the course
                if (course.NotificationEnabled == 1)
                {
                    notifications++; //Increase notifications
                    //Check if start date is today
                    if (course.StartDate == DateTime.Today)
                        //Show notification
                        CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} begins today!", notifications);
                    //Check if end date is today
                    if (course.EndDate == DateTime.Today)
                        //Show notification
                        CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} ends today!", notifications);
                }

                //Check course assessments per course in the loop
                assessmentList = new ObservableCollection<Assessment>(conn.Query<Assessment>($"Select * from Assessments where Course = '{course.Id}'"));
                //For each loop for assessment list
                foreach (Assessment assessment in assessmentList)
                {
                    //Check if notifications are enabled for the assessment
                    if (assessment.NotificationEnabled == 1)
                    {
                        notifications++;//Increase notifications
                        //Chheck if start date is today
                        if (assessment.StartDate == DateTime.Today)
                            //Show notification
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} {assessment.Title} {assessment.Type} begins today!", notifications);
                        //Check if end date is today
                        if (assessment.EndDate == DateTime.Today)
                            //Show notification
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseName} {assessment.Title} {assessment.Type} ends today!", notifications);
                    }
                }
            }
        }

        //TESTING DATA
        //Fills the SQLDB with test data
        private void DummyData()
        {
            var newTerm = new Term();
            newTerm.Title = "Term 1";
            newTerm.StartDate = new DateTime(2022, 01, 26);
            newTerm.EndDate = new DateTime(2022, 02, 28);
            conn.Insert(newTerm);
            //_termList.Add(newTerm);

            #region Courses
            var course1 = new Course();
            course1.CourseName = "Mobile Applications";
            course1.StartDate = new DateTime(2022, 01, 31);
            course1.EndDate = new DateTime(2022, 02, 28);
            course1.Status = "In-Progress";
            course1.InstructorName = "Kenneth Lamb";
            course1.InstructorPhone = "518-955-7313";
            course1.InstructorEmail = "klam114@wgu.edu";
            course1.NotificationEnabled = 1;
            course1.Notes = "This class is awesome!";
            course1.Term = newTerm.Id;
            conn.Insert(course1);

            #endregion

            #region Assessments for course 1
            var newObjectiveAssessment = new Assessment();
            newObjectiveAssessment.Title = "Test Assessment 1";
            newObjectiveAssessment.StartDate = new DateTime(2022, 01, 31);
            newObjectiveAssessment.EndDate = new DateTime(2022, 02, 28);
            newObjectiveAssessment.Course = course1.Id;
            newObjectiveAssessment.Type = "Objective";
            newObjectiveAssessment.NotificationEnabled = 1;
            conn.Insert(newObjectiveAssessment);

            var newPerformanceAssessment = new Assessment();
            newPerformanceAssessment.Title = "Test Assessment 2";
            newPerformanceAssessment.StartDate = new DateTime(2022, 01, 31);
            newPerformanceAssessment.EndDate = new DateTime(2022, 02, 28);
            newPerformanceAssessment.Course = course1.Id;
            newPerformanceAssessment.Type = "Performance";
            newPerformanceAssessment.NotificationEnabled = 1;
            conn.Insert(newPerformanceAssessment);
            #endregion

            termList = new ObservableCollection<Term>(conn.Table<Term>().ToList()); //Get the new list of terms
            PushNotifications();
        }

        //Used to remove the data in the db
        //For Testing purposes ONLY
        private void DropDB()
        {
            //Drop the Tables for the SQLite DB
            conn.DropTable<Term>();
            conn.DropTable<Course>();
            conn.DropTable<Assessment>();
        }
        #endregion
    }
}
