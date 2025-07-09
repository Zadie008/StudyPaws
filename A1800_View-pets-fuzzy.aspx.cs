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

    }

    protected void btnCats_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets.aspx");
    }

    protected void btnDogs_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-dogs.aspx");
    }

    protected void btnFuzzy_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-fuzzy.aspx");
    }

    protected void btnFarm_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-farm.aspx");
    }

    protected void btnSpecial_Click(object sender, EventArgs e)
    {
        Response.Redirect("A1800_View-pets-special.aspx");
    }
}