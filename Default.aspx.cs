using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] != null) 
        {
            lblLoggedInUserName.Text = Session["Username"].ToString() + "!";
        }
        else
        {
            lblLoggedInUserName.Text = "You are not logged in"; //will change this at some point in the future
        }
    }
}