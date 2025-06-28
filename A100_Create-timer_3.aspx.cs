using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A100_Create-timer_2.aspx");
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (Session["userID"] != null)
            {
                string cs;
                cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                OleDbConnection con = new OleDbConnection(cs);

                string command = "INSERT INTO [Timer] ([timerTitle], [timerTag], [timerDuration], [userID]) VALUES (@title, @tag, @duration, @id)";

                OleDbCommand cmd = new OleDbCommand(command, con);
                cmd.Parameters.AddWithValue("@title", Session["timerTitle"]);
                cmd.Parameters.AddWithValue("@tag", Session["timerTag"]);
                cmd.Parameters.AddWithValue("@duration", txtTimeMinutes.Text + "" + txtTimeSeconds.Text); /* FIX: has to fetch from 2 textboxes and concatinate*/
                cmd.Parameters.AddWithValue("@id", Session["userID"]);

                con.Open();
                int code = cmd.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        

        Response.Redirect("A200_View-timer.aspx");
    }
}