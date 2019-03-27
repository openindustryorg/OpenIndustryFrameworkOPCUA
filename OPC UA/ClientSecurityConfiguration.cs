using Opc.Ua;

namespace OPC
{
    /// <summary>
    /// Class represents a security configuration used when connecting to an OPC server
    /// </summary>
    public class ClientSecurityConfiguration 
    {
        private MessageSecurityMode securityMode;
        private string securityPolicyUri;
        private string username;
        private string password;
        private bool anonymousAccessAllowed;
        private UserIdentity userIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSecurityConfiguration"/> class.
        /// </summary>
        /// <param name="mode">The message security mode</param>
        /// <param name="policyUri">The Uri to the security policy</param>
        /// <param name="username">The username, leave blank to use ananoymous access</param>
        /// <param name="password">The password, leave blank to use anonymous access</param>
        /// <param name="userTokenPolicies">A collection of user token policies that may be used, to identify is anonymous login is allowed</param>
        public ClientSecurityConfiguration(MessageSecurityMode mode, string policyUri = "", string username = "", string password = "", UserTokenPolicyCollection userTokenPolicies = null)
        {
            this.securityMode = mode;
            this.securityPolicyUri = policyUri;
            this.username = username;
            this.password = password;

            if (userTokenPolicies != null)
            {
                foreach (var token in userTokenPolicies)
                {
                    if (token.TokenType == UserTokenType.Anonymous)
                    {
                        this.anonymousAccessAllowed = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the user identity to use for login
        /// </summary>
        public UserIdentity UserIdentity()
        {

            if (this.userIdentity == null)
            {
                if (this.UsernameAndPasswordEntered)
                {
                    this.userIdentity = new UserIdentity(username, password);
                }
                else
                {
                    if (this.anonymousAccessAllowed)
                    {
                        this.userIdentity = new UserIdentity(new AnonymousIdentityToken());
                    }
                }
            }

            return this.userIdentity;

        }

        public bool UsernameAndPasswordEntered
        {
            get
            {
                return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            }
        }

    }
}
