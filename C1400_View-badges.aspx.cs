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
            case 1: return "~/Badges/Axolotl.png";
            case 2: return "~/Badges/Blobfish.png";
            case 3: return "~/Badges/ChillGuy.png";
            case 4: return "~/Badges/Hyrax.png";
            case 5: return "~/Badges/KoiFish.png";
            case 6: return "~/Badges/Marmot.png";
            case 7: return "~/Badges/MooDeng.png";
            case 8: return "~/Badges/Narwal.png";
            case 9: return "~/Badges/PelicanAndCapy.png";
            case 10: return "~/Badges/Pesto.png";
            case 11: return "~/Badges/Pufferfish.png";
            case 12: return "~/Badges/Sloth.png";
            case 13: return "~/Badges/WesternCats.png";
            default: return "~/Badges/WesternCats.png";
        }
    }
}