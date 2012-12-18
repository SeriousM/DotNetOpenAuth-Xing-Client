﻿namespace C1_net.Web
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;
    using DotNetOpenAuth.ApplicationBlock;
    using DotNetOpenAuth.OAuth;
    using System.Runtime.Serialization.Json;
    /// <summary>
    /// A page to demonstrate downloading a Gmail address book using OAuth.
    /// </summary>
    public partial class Xing : System.Web.UI.Page
    {
        private string AccessToken
        {
            get { return (string)Session["XingAccessToken"]; }
            set { Session["XingAccessToken"] = value; }
        }

        private InMemoryTokenManager TokenManager
        {
            get
            {
                var tokenManager = (InMemoryTokenManager)Application["XingTokenManager"];
                if (tokenManager == null)
                {
                    string consumerKey = "08c61964c9b6fb709394"; //ConfigurationManager.AppSettings["googleConsumerKey"];
                    string consumerSecret = "f4741103c52c7cae5ccda087d528117a50034305"; // ConfigurationManager.AppSettings["googleConsumerSecret"];
                    if (!string.IsNullOrEmpty(consumerKey))
                    {
                        tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);
                        Application["XingTokenManager"] = tokenManager;
                    }
                }
                return tokenManager;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.TokenManager != null)
            {
                this.MultiView1.ActiveViewIndex = 1;

                if (!IsPostBack)
                {
                    var xing = new WebConsumer(XingClient.ServiceDescription, this.TokenManager);

                    // Is Google calling back with authorization?
                    var accessTokenResponse = xing.ProcessUserAuthorization();
                    if (accessTokenResponse != null)
                    {
                        this.AccessToken = accessTokenResponse.AccessToken;
                        //Response.Redirect("/C1_netTestPage.aspx#/Home");
                        dynamic result = XingClient.GetMe(xing, this.AccessToken);
                        string display_name = result.display_name;
                        string id = result.id;
                        string first_name = result.first_name;
                        string last_name = result.last_name;
                        string picurl = result.photo_urls.maxi_thumb;
                        dynamic result2 = XingClient.GetMyCotacts(xing, this.AccessToken, "display_name", 50, 0);
                        dynamic a = result2.contacts;
                        int b = a.total;
                        dynamic c = a.users;
                        int d = c.Count;

                        string display_name1 = c[0].display_name;
                        string id1 = c[0].id;

                        dynamic result3 = XingClient.GetUser(xing, this.AccessToken, id1, "photo_urls.medium_thumb");
                        string urli = result3.photo_urls.medium_thumb;
                        dynamic result4 = XingClient.GetUser(xing, this.AccessToken, id1, "active_email");
                        string mail = result3.active_email;
                        dynamic result5 = XingClient.GetUser(xing, this.AccessToken, c[12].id);
                        string url = XingClient.GetScopeUri(XingClient.Applications.me);
                        //Response.Redirect("/#/Home"); 

                    }
                    else if (this.AccessToken == null)
                    {
                        // If we don't yet have access, immediately request it.
                        XingClient.RequestAuthorization(xing, XingClient.Applications.me);
                    }
                }
            }
        }

        
    }
}