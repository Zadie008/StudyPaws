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
        public string badgeDescBronze { get; set; }
        public string badgeDescSilver { get; set; }
        public string badgeDescGold { get; set; }
        public string badgeIconNum { get; set; }
        public string badgeType { get; set; }
    }

    private void LoadBadges()
    {
        string cs = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string query = @"
                        SELECT b.badgeName, b.badgeDescBronze, b.badgeDescSilver, b.badgeDescGold, 
                        b.badgeIconNum, ub.badgeType
                        FROM Badges b
                        LEFT JOIN UserBadge ub ON b.badgeID = ub.badgeID
                        WHERE ub.userID = ? OR ub.userID IS NULL";

        List<BadgeIcon> badgeList = new List<BadgeIcon>();

        using (OleDbConnection con = new OleDbConnection(cs))
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            cmd.Parameters.AddWithValue("?", Session["userID"]);

            con.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    BadgeIcon badge = new BadgeIcon
                    {
                        badgeName = reader["badgeName"].ToString(),
                        badgeDescBronze = reader["badgeDescBronze"].ToString(),
                        badgeDescSilver = reader["badgeDescSilver"].ToString(),
                        badgeDescGold = reader["badgeDescGold"].ToString(),
                        badgeIconNum = GetBadgeImagePath(Convert.ToInt32(reader["badgeIconNum"])),
                        badgeType = reader["badgeType"] == DBNull.Value ? "none" : reader["badgeType"].ToString().ToLower()
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
        switch (badgeImageID)
        {
            case 1: return "~/Images/Badges/BusyBee.png";
            case 2: return "~/Images/Badges/LoneWoof.png";
            case 3: return "~/Images/Badges/EarlyBird.png";
            case 4: return "~/Images/Badges/NightOwl.png";
            case 5: return "~/Images/Badges/SlothingOnTheJob.png";
            case 6: return "~/Images/Badges/DreamTeam.png";
            case 7: return "~/Images/Badges/RignLeader.png";
            case 8: return "~/Images/Badges/Cancelotl.png";
            case 9: return "~/Images/Badges/NotMyBloblem.png";
            case 10: return "~/Images/Badges/AcademicWeapon.png";
            case 11: return "~/Images/Badges/11_Collector.png";
            case 12: return "~/Images/Badges/Meanie.png";
            case 13: return "~/Images/Badges/FriendsPurrever.png";
            case 14: return "~/Images/Badges/PeskyPelican.png";
            case 15: return "~/Images/Badges/PuffingPopular.png";
            case 16: return "~/Images/Badges/RisingStar.png";

            default: return "~/Images/Badges/PeskyPelican.png";
        }
    }
    protected string GetStarHtml(string badgeType)
    {
        int stars = 0;
        if (badgeType == "bronze") stars = 1;
        else if (badgeType == "silver") stars = 2;
        else if (badgeType == "gold") stars = 3;

        string filledStar = "<img src='Icons/icons8-star-filled-white-96.png' class='star-icon' />";
        string emptyStar = "<img src='Icons/icons8-star-white-96.png' class='star-icon' />";

        string html = "";
        for (int i = 0; i < 3; i++)
        {
            html += i < stars ? filledStar : emptyStar;
        }

        return html;
    }
}