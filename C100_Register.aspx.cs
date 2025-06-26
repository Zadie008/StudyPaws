using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class C100_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        

    }
   

    protected void btnRegister_Click1(object sender, EventArgs e)
    {
        pnlPopup.Visible = true;

        // Inject JS to run showPopup only once (no effect on refresh)
        ScriptManager.RegisterStartupScript(this, GetType(), "popup", "showPopup();", true);

    }
}