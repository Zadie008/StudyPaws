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
        if (Session["userID"] == null)
        {
            Session["userID"] = 1; // TESTING ONLY!!!!!!
        }

        if (!IsPostBack)
        {
            if (Session["userID"] != null)
            {
                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (OleDbConnection con = new OleDbConnection(cs))
                {
                    string command = "SELECT [timerDateCreated] AS [Date Created], [timerTitle] AS Title, [timerTag] AS Tag, [timerDuration] AS Duration FROM [Timer] WHERE userID = @id ORDER BY [timerDateCreated] DESC";

                    OleDbCommand cmd = new OleDbCommand(command, con);
                    cmd.Parameters.AddWithValue("@id", Session["userID"]);

                    con.Open();
                    OleDbDataReader collection = cmd.ExecuteReader();
                    GridView1.DataSource = collection;
                    GridView1.DataBind();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A700_Create-study-session_2.aspx");
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {

    }
}