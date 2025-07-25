using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["SelectedIcon"] = "";

            if (Session["UserID"] != null)
            {
                string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                GetUserProfileIcon(cs, Session["UserID"].ToString());
            }
        }

        pnlConfirmPfpf.Visible = false;
        RefreshSelectedIcon();
    }

    protected void btnBackProfile_Click(object sender, EventArgs e)
    {
        Response.Redirect("C100-C500_Profile.aspx");
    }

    protected void btnChangeIcon_Click(object sender, EventArgs e)
    {
        string selectedValue = selectedIcon.Value;

        if (!string.IsNullOrEmpty(selectedValue))
        {
            int iconNum = int.Parse(selectedValue);

            if (Session["UserID"] != null)
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                try
                {
                    string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                    using (OleDbConnection con = new OleDbConnection(cs))
                    {
                        string query = "UPDATE [Users] SET [iconNum] = ? WHERE [userID] = ?";
                        using (OleDbCommand cmd = new OleDbCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("?", iconNum);
                            cmd.Parameters.AddWithValue("?", userId);
                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            con.Close();

                            if (rowsAffected > 0)
                            {
                                pnlConfirmPfpf.Visible = true;
                                GetUserProfileIcon(cs, Session["UserID"].ToString());
                            }
                            else
                            {
                                Response.Write("<script>alert('No rows updated. UserID might be incorrect.');</script>");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Database error: " + ex.Message.Replace("'", "\\'") + "');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('UserID is missing from session.');</script>");
            }
        }
        else
        {
            Response.Write("<script>alert('No icon selected.');</script>");
        }
    }

    protected void SelectIcon_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton clickedButton = sender as ImageButton;

        if (clickedButton != null)
        {
            selectedIcon.Value = clickedButton.CommandArgument;
            ViewState["SelectedIcon"] = clickedButton.CommandArgument;
            RefreshSelectedIcon();
        }
    }

    private void RefreshSelectedIcon()
    {
        // Reset all button styles
        btnCat.CssClass = "iconItem circle-cat";
        btnDog.CssClass = "iconItem circle-dog";
        btnBunny.CssClass = "iconItem circle-bunny";
        btnCow.CssClass = "iconItem circle-cow";
        btnUnicorn.CssClass = "iconItem circle-unicorn";

        string selected; // Declare the variable first

        if (ViewState["SelectedIcon"] != null)
        {
            selected = ViewState["SelectedIcon"].ToString();
        }
        else
        {
            selected = null; // ViewState entry is null
        }

        switch (selected)
        {
            case "1": btnCat.CssClass += " selected"; break;
            case "2": btnDog.CssClass += " selected"; break;
            case "3": btnBunny.CssClass += " selected"; break;
            case "4": btnCow.CssClass += " selected"; break;
            case "5": btnUnicorn.CssClass += " selected"; break;
        }
    }

    private void GetUserProfileIcon(string connectionString, string userID)
    {
        string query = "SELECT iconNum FROM Users WHERE userID = @userID";

        using (OleDbConnection con = new OleDbConnection(connectionString))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@userID", userID);
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                int iconNum;
                if (result != null && int.TryParse(result.ToString(), out iconNum))
                {
                    string iconPath = GetProfileImagePath(iconNum);
                    string circleClass = GetCircleClass(iconNum);

                    profilePet.ImageUrl = ResolveUrl(iconPath);
                    profileCircle.Attributes["class"] = "profileCircle " + circleClass;

                    ViewState["SelectedIcon"] = iconNum.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting profile icon: " + ex.Message);
            }
        }
    }

    private string GetProfileImagePath(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "~/Images/ProfilePictures/CatPfp.png";
            case 2: return "~/Images/ProfilePictures/DogPfp.png";
            case 3: return "~/Images/ProfilePictures/BunnyPfp.png";
            case 4: return "~/Images/ProfilePictures/CowPfp.png";
            case 5: return "~/Images/ProfilePictures/UnicornPfp.png";
            default: return "~/Images/ProfilePictures/CatPfp.png";
        }
    }
    private string GetCircleClass(int iconNum)
    {
        switch (iconNum)
        {
            case 1: return "circle-cat";
            case 2: return "circle-dog";
            case 3: return "circle-bunny";
            case 4: return "circle-cow";
            case 5: return "circle-unicorn";
            default: return "circle-cat";
        }
    }
}