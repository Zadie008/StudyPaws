using System;
using System.Collections.Generic;
using System.Configuration;
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
        }

        // Clear all selections
        btnCat.CssClass = "iconItem circle-cat";
        btnDog.CssClass = "iconItem circle-dog";
        btnBunny.CssClass = "iconItem circle-bunny";
        btnCow.CssClass = "iconItem circle-cow";
        btnUnicorn.CssClass = "iconItem circle-unicorn";

        string selected = ViewState["SelectedIcon"] != null ? ViewState["SelectedIcon"].ToString() : "";

        switch (selected)
        {
            case "1": btnCat.CssClass += " selected"; break;
            case "2": btnDog.CssClass += " selected"; break;
            case "3": btnBunny.CssClass += " selected"; break;
            case "4": btnCow.CssClass += " selected"; break;
            case "5": btnUnicorn.CssClass += " selected"; break;
        }
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
                int userId = (int)Session["UserID"];  

                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Users SET iconNum = @IconNum WHERE userID = @UserID";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@IconNum", iconNum);
                            cmd.Parameters.AddWithValue("@UserID", userId);

                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                Response.Write("<script>alert('No rows updated. UserID might be incorrect.');</script>");
                            }
                            else
                            {
                                Response.Redirect("C100-C500_Profile.aspx");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('UserID is empty or null');</script>");
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
            Page_Load(null, null); 
        }
    }
}