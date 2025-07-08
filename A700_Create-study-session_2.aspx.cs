using System;
using System.Collections.Generic;
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
            if (Session["sessionTitle"] != null)
            {
                txtTitle.Text = Session["sessionTitle"].ToString();
            }

            if (Session["sessionTag"] != null)
            {
                dropdownTag.SelectedValue = Session["sessionTag"].ToString();
            }
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("A700_Create-study-session.aspx");
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        Session["sessionTitle"] = txtTitle.Text;
        Session["sessionTag"] = dropdownTag.SelectedValue;

        Response.Redirect("A700_Create-study-session_3.aspx");
    }
}