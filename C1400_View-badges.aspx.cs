using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadBadges();
        }
    }

    public class BadgeIcon
    {
        public string badgeName { get; set; }
        public string  badgeDescBronze{ get; set; }
        public string badgeDescSilver { get; set; }
        public string badgeDescGold { get; set; }
        public string badgeIconNum { get; set; }
       
    }

    private void LoadBadges()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = "SELECT badgeName, badgeDescBronze, badgeIconNum FROM Badges";

        List<BadgeIcon> badgeList = new List<BadgeIcon>();

        using (OleDbConnection con = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            con.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    BadgeIcon badge = new BadgeIcon
                    {
                        badgeName = reader["badgeName"].ToString(),
                        badgeDescBronze = reader["badgeDescBronze"].ToString(),
                        badgeIconNum = GetBadgeImagePath(Convert.ToInt32(reader["badgeIconNum"]))
                    };
                    badgeList.Add(badge);
                }
            }
        }

        rpPets.DataSource = badgeList;
        rpPets.DataBind();
    }

    private string GetBadgeImagePath(int badgeImageID)
    {
        switch(badgeImageID)
        {
            case 1: return "~/Images/Badges/Axolotl.png";
            case 2: return "~/Images/Badges/Blobfish.png";
            case 3: return "~/Images/Badges/ChillGuy.png";
            case 4: return "~/Images/Badges/Hyrax.png";
            case 5: return "~/Images/Badges/KoiFish.png";
            case 6: return "~/Images/Badges/Marmot.png";
            case 7: return "~/Images/Badges/MooDeng.png";
            case 8: return "~/Images/Badges/Narwal.png";
            case 9: return "~/Images/Badges/PelicanAndCapy.png";
            case 10: return "~/Images/Badges/Pesto.png";
            case 11: return "~/Images/Badges/Pufferfish.png";
            case 12: return "~/Images/Badges/Sloth.png";
            case 13: return "~/Images/Badges/WesternCats.png";
            default: return "~/Images/Badges/WesternCats.png";
        }
    }
}