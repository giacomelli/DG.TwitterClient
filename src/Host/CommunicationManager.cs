#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
#endregion

namespace DG.TwitterClient.Host
{
    /// <summary>
    /// Gerenciador da comunicação com o Twitter.
    /// </summary>
    public static class CommunicationManager
    {
        #region Eventos
        public static event EventHandler LoginStarting;
        public static event EventHandler<LoginFailedEventArgs> LoginFailed;
        public static event EventHandler LoginFinished;
        public static event EventHandler<StatusEventArgs> StatusReceived;
        public static event EventHandler StatusUpdating;
        public static event EventHandler<StatusUpdateFailedEventArgs> StatusUpdateFailed;
        public static event EventHandler<StatusUpdatedEventArgs> StatusUpdated;
        #endregion

        #region Campos
        /// <summary>
        /// As autorizações de todas as contas com o Twitter.
        /// </summary>
        private static List<PinAuthorizer> s_auths;

        /// <summary>
        /// A data/hora da última atualização de status.
        /// </summary>
        private static DateTime s_lastStatusUpdating = DateTime.MinValue;

        private static Timer s_statusUpdatingTimer;

        private static Queue<string> s_userNamesToLoginQueue;
        private static Queue<string> s_accessTokenQueue;
        private static Queue<string> s_oauthTokenQueue;

        private static int s_millsecondsIntervalToUpdate;
        #endregion

        #region Propriedades
        /// <summary>
        /// Obtém os contextos de consulta de todas as contas autorizadas.
        /// </summary>
        private static IList<TwitterContext> Contexts
        {
            get;
            set;
        }

        public static IList<DGAccount> Accounts
        {
            get;
            private set;
        }

        public static bool Initialized
        {
            get;
            private set;
        }
        #endregion

        #region Métodos

        #region Inicialização
        /// <summary>
        /// Realiza a inicialização.
        /// </summary>
        public static void Initialize()
        {
            s_userNamesToLoginQueue = new Queue<string>(ConfigurationManager.AppSettings["twitterUserName"].Split(';'));
            s_accessTokenQueue = new Queue<string>(ConfigurationManager.AppSettings["twitterAccessToken"].Split(';'));
            s_oauthTokenQueue = new Queue<string>(ConfigurationManager.AppSettings["twitterOAuthToken"].Split(';'));
            s_auths = new List<PinAuthorizer>(s_userNamesToLoginQueue.Count);
            Contexts = new List<TwitterContext>();
            Accounts = new List<DGAccount>();
            s_millsecondsIntervalToUpdate = Convert.ToInt32(ConfigurationManager.AppSettings["secondsIntervalToUpdate"]) * 1000;

            Initialized = true;
        }
        #endregion

        #region Autenticação
        /// <summary>
        /// Loga todas as contas ao Twitter.
        /// </summary>
        public static void Login()
        {
            if (s_userNamesToLoginQueue.Count == 0)
            {
                return;
            }

            try
            {
                if (s_statusUpdatingTimer != null)
                {
                    s_statusUpdatingTimer.Change(int.MaxValue, int.MaxValue);
                }

                OnLoginStarting(EventArgs.Empty);

                while (s_userNamesToLoginQueue.Count > 0)
                {
                    // Remove usuário e senha do início da fila.
                    var userName = s_userNamesToLoginQueue.Dequeue();
                    var accessToken = s_accessTokenQueue.Count > 0 ? s_accessTokenQueue.Dequeue() : null;
                    var oauthToken = s_oauthTokenQueue.Count > 0 ? s_oauthTokenQueue.Dequeue() : null;

                    try
                    {
                        var ctx = CreateContext(accessToken, oauthToken);
                        Contexts.Add(ctx);

                        var query = from a in ctx.Account
                                    where a.Type == AccountType.VerifyCredentials
                                    select a;

                        Accounts.Add(new DGAccount(query.First()));
                    }
                    catch (Exception ex)
                    {
                        OnLoginFailed(new LoginFailedEventArgs(ex));
                        return;
                    }                    
                    Thread.Sleep(1000);
                }
            }
            finally
            {
                if (s_statusUpdatingTimer != null)
                {
                    s_statusUpdatingTimer.Change(s_millsecondsIntervalToUpdate, s_millsecondsIntervalToUpdate);
                }
            }

            s_statusUpdatingTimer = new Timer(UpdateStatusTimerCallback, null, s_millsecondsIntervalToUpdate, s_millsecondsIntervalToUpdate);
            OnLoginFinished(EventArgs.Empty);
        }

        private static TwitterContext CreateContext(string accessToken, string oAuthToken)
        {
            var auth = new PinAuthorizer()
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "rgRMroMnWQ8IoUNYcNIX0BsLV",
                    ConsumerSecret = "vfX2T7V9kwmwDzCyBW8ZP91jElRhGnCNnG6fbfcvz1ysOrt8Bz",
                    AccessToken = accessToken,
                    OAuthToken = oAuthToken
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");

                    var pin =  Console.ReadLine();

                    return pin;
                }
            };

            auth.AuthAccessType = AuthAccessType.Write;
            auth.Authorize();

            return new TwitterContext(auth);

        }

        private static void OnLoginStarting(EventArgs e)
        {
            if (LoginStarting != null)
            {
                LoginStarting(typeof(CommunicationManager), e);
            }
        }

        private static void OnLoginFailed(LoginFailedEventArgs e)
        {
            if (LoginFailed != null)
            {
                LoginFailed(typeof(CommunicationManager), e);
            }
        }

        private static void OnLoginFinished(EventArgs e)
        {
            if (LoginFinished != null)
            {
                LoginFinished(typeof(CommunicationManager), e);
            }
        }
        #endregion

        #region Status
        public static void ChangeStatusUpdateInterval(int intervalSeconds)
        {
            if (s_statusUpdatingTimer != null)
            {
                s_millsecondsIntervalToUpdate = intervalSeconds * 1000;
                s_statusUpdatingTimer.Change(s_millsecondsIntervalToUpdate, s_millsecondsIntervalToUpdate);
            }
        }

        public static void UpdateStatus()
        {
            Login();

            OnStatusUpdating(EventArgs.Empty);

            var status = new List<DGStatus>();
            IOrderedEnumerable<DGStatus> newStatus = null;

            try
            {
                for (int i = 0; i < Contexts.Count; i++)
                {
                    var query = from t in Contexts[i].Status
                                where t.Type == StatusType.Home && t.CreatedAt >= s_lastStatusUpdating
                                select new DGStatus(t, Accounts[i]);


                    status.AddRange(query);
                }

                newStatus = status.Distinct(new StatusEqualityComparer()).OrderBy(s => s.CreatedAt);

            }
            catch (Exception ex)
            {
                OnStatusUpdateFailed(new StatusUpdateFailedEventArgs(ex));
                return;
            }

            s_lastStatusUpdating = DateTime.UtcNow;

            foreach (var s in newStatus)
            {
                OnStatusReceived(new StatusEventArgs(s));
            }

            OnStatusUpdated(new StatusUpdatedEventArgs(newStatus.ToArray()));
        }

        private static void OnStatusReceived(StatusEventArgs e)
        {
            if (StatusReceived != null)
            {
                StatusReceived(typeof(CommunicationManager), e);
            }
        }

        private static void OnStatusUpdating(EventArgs e)
        {
            if (StatusUpdating != null)
            {
                StatusUpdating(typeof(CommunicationManager), e);
            }
        }

        private static void OnStatusUpdateFailed(StatusUpdateFailedEventArgs e)
        {
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(typeof(CommunicationManager), e);
            }
        }

        private static void OnStatusUpdated(StatusUpdatedEventArgs e)
        {
            if (StatusUpdated != null)
            {
                StatusUpdated(typeof(CommunicationManager), e);
            }
        }

        private static void UpdateStatusTimerCallback(object state)
        {
            UpdateStatus();
        }
        #endregion

        #endregion        
    }
}
