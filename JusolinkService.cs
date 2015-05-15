using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using System.Json;
using Linkhub;

namespace Jusolink
{
    public class JusolinkService
    {
        private const String ServiceID = "JUSOLINK";
        private const String ServiceURL = "https://juso.linkhub.co.kr";
        private const String APIVersion = "1.0";
        private const String CRLF = "\r\n";

        private Token token;
        private Authority _LinkhubAuth;
        private List<String> _Scopes = new List<string>();
                

        public JusolinkService(String LinkID, String SecretKey)
        {
            _LinkhubAuth = new Authority(LinkID, SecretKey);
            _Scopes.Add("200");
        }

        public Double GetBalance()
        {
            try
            {
                return _LinkhubAuth.getPartnerBalance(getSession_Token(), ServiceID);
            }
            catch (LinkhubException le)
            {
                throw new JusolinkException(le);
            }
        }

        public Single GetUnitCost()
        {
            UnitCostResponse response = httpget<UnitCostResponse>("/Search/UnitCost", null, null);

            return Single.Parse(response.unitCost);
        }

        public SearchResult search(String index, int? PageNum, int? PerPage, bool noSuggest, bool noDiff)
        {
            String url;
            if(PerPage != null){
                if(PerPage <0) PerPage = 20;
            }

            if(index == null ) {
                throw new JusolinkException(99999999, "검색어가 입력되지 않았습니다");
            }
            url = "/Search?Searches="+index;

            if(PageNum != null){
                url = url + "&PageNum=" + PageNum;
            }

            if(PerPage != null){
                url = url + "&PerPage=" + PerPage;
            }

            if(noSuggest){
                url = url + "&noSuggest=true";
            }

            if(noDiff)
            {
                url = url + "&noDiff=true";
            }

            SearchResult response = httpget<SearchResult>(url, null, null);

            return response;
        }


        #region protected

        protected String toJsonString(Object graph)
        {
            return _LinkhubAuth.stringify(graph);

        }
        protected T fromJson<T>(Stream jsonStream)
        {
            return _LinkhubAuth.fromJson<T>(jsonStream);
        }

        private JsonValue getJsonValue(Stream jsonStream)
        {
            return JsonValue.Load(jsonStream);
        }


        private String getSession_Token()
        {
            Token tmpToken = null;
            
            if (token != null)
            {
                tmpToken = token;
            }

            bool expired = true;
            if (tmpToken != null)
            {
                DateTime expiration = DateTime.Parse(tmpToken.expiration);

                expired = expiration < DateTime.Now;
            }

            if (expired)
            {
                try
                {
                    tmpToken = _LinkhubAuth.getToken(ServiceID, null, _Scopes);

                    token = tmpToken;
                }
                catch (LinkhubException le)
                {
                    throw new JusolinkException(le);
                }
            }

            return token.session_token;
        }

        protected T httpget<T>(String url, String CorpNum, String UserID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + url);

            String bearerToken = getSession_Token();
            request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);
            
            request.Headers.Add("x-api-version", APIVersion);

            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stReadData = response.GetResponseStream();

                return fromJson<T>(stReadData);

            }
            catch (Exception we)
            {
                if (we is WebException && ((WebException)we).Response != null)
                {
                    Stream stReadData = ((WebException)we).Response.GetResponseStream();
                    JsonValue t = getJsonValue(stReadData);
                    throw new JusolinkException(t["code"], t["message"]);
                }
                throw new JusolinkException(-99999999, we.Message);
            }

        }

        #endregion
        
        public class UnitCostResponse
        {
            public string unitCost;
        }
    }
}
