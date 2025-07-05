using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
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
            if (Session["Username"] != null)
            {
                Console.WriteLine("✅ Logged in as: " + Session["Username"]);
               
                
            }
            else
            {
                Console.WriteLine("⚠️ Session[\"Username\"] is null");
            }
        }
    }





    protected void btnSearchFriends_Click(object sender, EventArgs e)
    {

    }
}