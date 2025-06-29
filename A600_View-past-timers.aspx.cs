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
        Response.Redirect("A100_Create-timer.aspx");
    }

    public string FormatDuration(object totalSecondsObj)
    {
        if (totalSecondsObj == null || totalSecondsObj == DBNull.Value)
            return "00:00:00";

        int totalSeconds;
        if (int.TryParse(totalSecondsObj.ToString(), out totalSeconds))
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }

        return "00:00:00";
    }
}