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

namespace Course_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermAdd : ContentPage
    {
        //Variables
        private SQLiteConnection conn; //Holds the SQLite connection

        //Constructor
        public TermAdd()
        {
            InitializeComponent();
            conn = DependencyService.Get<Isqlite>().GetConnection(); //Setup the SQLite Connection using the interface
        }

        #region Buttons
        //Back Button
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            //Navigate back a page
            await Navigation.PopModalAsync();
        }

        //Save button
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            //Create a new blank term
            Term term = new Term();

            //Fill in the terms data
            term.Title = txtTitle.Text;
            term.StartDate = startDate.Date;
            term.EndDate = endDate.Date;

            //Check that the title is not empty
            if (term.Title == null)
            {
                //Error Title cannot be blank
                await DisplayAlert("Error.", "Title cannot be left blank!", "Ok");
                return;
            }
            else
            {
                //Check that the start date is not greater than the end date
                if (term.StartDate > term.EndDate)
                {
                    //Error invalid dates
                    await DisplayAlert("Error.", "Start date cannot be after the End date!", "Ok");
                    return;
                }
                else
                {
                    //Insert new Term into the table
                    conn.Insert(term);

                    //Navigate back a page
                    await Navigation.PopModalAsync();
                }
            }
        }
        #endregion
    }
}