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
                LoadFriends();
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

    private void LoadFriends(string searchTerm = "")
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        using (OleDbConnection con = new OleDbConnection(cs))
        {
            string command = "SELECT username, iconNum FROM Users WHERE userID IN (SELECT IIF(userIDfrom = @id, userIDto, userIDfrom) FROM FriendsList WHERE userIDfrom = @id OR userIDto = @id)";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                command += " AND username LIKE @search";
            }

            OleDbCommand cmd = new OleDbCommand(command, con);
            cmd.Parameters.AddWithValue("@id", Session["userID"]);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
            }

            con.Open();
            OleDbDataReader rdr = cmd.ExecuteReader();
            GridView1.DataSource = rdr;
            GridView1.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        LoadFriends(txtSearch.Text);
    }

    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        LoadFriends(txtSearch.Text);
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ToggleInvite")
        {
            string friendUsername = e.CommandArgument.ToString();
            List<string> invitedFriends = Session["invitedFriends"] as List<string> ?? new List<string>();

            if (invitedFriends.Contains(friendUsername))
            {
                invitedFriends.Remove(friendUsername); // toggle off
            }
            else
            {
                invitedFriends.Add(friendUsername); // toggle on
            }

            Session["invitedFriends"] = invitedFriends;
            LoadFriends(txtSearch.Text); // reload with updated icons
        }
    }

    public string GetProfileImageUrl(object iconNum)
    {
        int num = Convert.ToInt32(iconNum);
        switch (num)
        {
            case 1: return "Images/ProfilePictures/CatPfp.png";
            case 2: return "Images/ProfilePictures/DogPfp.png";
            case 3: return "Images/ProfilePictures/BunnyPfp.png";
            case 4: return "Images/ProfilePictures/CowPfp.png";
            case 5: return "Images/ProfilePictures/UnicornPfp.png";
            default: return "Images/ProfilePictures/CatPfp.png";
        }
    }

    public string GetAddButtonImage(string username)
    {
        var list = Session["invitedFriends"] as List<string> ?? new List<string>();
        return list.Contains(username) ? "Icons/icons8-check-white-96.png" : "Icons/icons8-add-new-white-96.png";
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        List<string> invitedFriends = Session["invitedFriends"] as List<string>;
        if (invitedFriends == null || invitedFriends.Count == 0)
        {
            // add validation!!
            return;
        }

        Session["selectedFriends"] = invitedFriends;

        // You can also generate the studySessionID here if needed
        Response.Redirect("A700_Create-study-session_4.aspx");
    }
}