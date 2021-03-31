using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCodeWebAPI.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class PathList
        {
            public const string GetUserList = Base + "/GetUserList";
        

        }
        public static class Posts
        {            
            public const string GetAll = Base + "/posts";
            public const string Create = Base + "/posts";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";
            public const string Get = Base + "/posts/{postId}";
        }
        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
            public const string UpdateUserInfo = Base + "/identity/updateUserInfo";
            public const string Reset = Base + "/identity/reset";
            public const string Change = Base + "/identity/change";
        }
        public static class Mongo
        {
            public const string Get = Base + "/mongo/post";
            public const string Create = Base + "/mongo/post";
    
        }

        public static class Payment
        {
            public const string Post = Base + "/payment/post";
          
            //public const string CheckPayment = Base + "/payment/checkPayment";
            public const string FetchPayment = Base + "/payment/FetchPayment/{payToken}";

            public const string PostSubscription = Base + "/payment/postSubscription";
            public const string FetchPaymentSubscription = Base + "/payment/FetchPaymentSubscription/{payToken}/{email}/{productName}/{academy}";


        }
    }
}
