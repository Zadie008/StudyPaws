using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["timerTitle"] != null)
            {
                txtTitle.Text = Session["timerTitle"].ToString();
            }

            if (Session["timerTag"] != null)
            {
                dropdownTag.SelectedValue = Session["timerTag"].ToString();
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A100_Create-timer.aspx");
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        Session["timerTitle"] = txtTitle.Text;
        Session["timerTag"] = dropdownTag.SelectedValue;

        Response.Redirect("A100_Create-timer_3.aspx");
    }

    protected void btnViewPastTimers_Click(object sender, EventArgs e)
    {

    }
}