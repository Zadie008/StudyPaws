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
                    string command = "SELECT StudySession.sessionStart AS [Start Time], StudySession.sessionTitle AS Title, StudySession.sessionTag AS Tag, StudySession.sessionDuration AS Duration FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE StudySessionParticipants.userID = ? ORDER BY StudySession.sessionStart DESC";

                    OleDbCommand cmd = new OleDbCommand(command, con);
                    cmd.Parameters.AddWithValue("?", Convert.ToInt32(Session["userID"]));
                    //cmd.Parameters.AddWithValue("?", DateTime.Now);

                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    GridView1.DataSource = reader;
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
        Response.Redirect("A700_Create-study-session.aspx");
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

    protected void btnApplyFilters_Click(object sender, EventArgs e)
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string userID = Session["userID"].ToString();
        string dateFilter = txtFilterDate.Text;
        string tagFilter = ddlFilterTag.SelectedValue;

        List<string> conditions = new List<string>();
        List<OleDbParameter> parameters = new List<OleDbParameter>();

        conditions.Add("StudySessionParticipants.userID = ?");
        parameters.Add(new OleDbParameter("userID", Convert.ToInt32(userID)));

        //conditions.Add("StudySession.sessionEnd < ?");
        //parameters.Add(new OleDbParameter("sessionEnd", DateTime.Now));

        if (!string.IsNullOrEmpty(dateFilter))
        {
            string formattedDate = DateTime.Parse(dateFilter).ToString("yyyy-MM-dd");
            conditions.Add("Format(StudySession.sessionStart, 'yyyy-mm-dd') = ?");
            parameters.Add(new OleDbParameter("sessionDate", formattedDate));
        }

        if (!string.IsNullOrEmpty(tagFilter))
        {
            conditions.Add("StudySession.sessionTag = ?");
            parameters.Add(new OleDbParameter("sessionTag", tagFilter));
        }

        string whereClause = string.Join(" AND ", conditions);

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string query = "SELECT StudySession.sessionStart AS [Start Time], StudySession.sessionTitle AS Title, StudySession.sessionTag AS Tag, StudySession.sessionDuration AS Duration FROM StudySession INNER JOIN StudySessionParticipants ON StudySession.sessionID = StudySessionParticipants.sessionID WHERE " + whereClause + " ORDER BY StudySession.sessionStart DESC";

            OleDbCommand cmd = new OleDbCommand(query, con);

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);
            }

            con.Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            GridView1.DataSource = reader;
            GridView1.DataBind();
        }
    }
}