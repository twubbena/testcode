using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;



public class YWSample
{

    const string cURL = "https://weather-ydn-yql.media.yahoo.com/forecastrss";
    const string cAppID = "6OZjvl4s";
    const string cConsumerKey = "dj0yJmk9NVREQ1MzTmdGSEtCJmQ9WVdrOU5rOWFhblpzTkhNbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PWMy";
    const string cConsumerSecret = "c1c728f6ef35872a2cbea203d8608229d8655302";
    const string cOAuthVersion = "1.0";
    const string cOAuthSignMethod = "HMAC-SHA1";
    const string cWeatherID = "woeid=2388929";  
    const string cUnitID = "u=f";         
    const string cFormat = "json";

    static string _get_timestamp()
    {
        TimeSpan lTS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64(lTS.TotalSeconds).ToString();
    }  // end _get_timestamp

    static string _get_nonce()
    {
        return Convert.ToBase64String(
         new ASCIIEncoding().GetBytes(
          DateTime.Now.Ticks.ToString()
         )
        );
    }  // end _get_nonce

    // NOTE: whenever the value of a parameter is changed, say cUnitID "u=c" => "location=sunnyvale,ca"
    // The order in lSign needs to be updated, i.e. re-sort lSign
    // Please don't simply change value of any parameter without re-sorting.
    static string _get_auth()
    {
        string retVal;
        string lNonce = _get_nonce();
        string lTimes = _get_timestamp();
        string lCKey = string.Concat(cConsumerSecret, "&");
        string lSign = string.Format(  // note the sort order !!!
         "format={0}&" +
         "oauth_consumer_key={1}&" +
         "oauth_nonce={2}&" +
         "oauth_signature_method={3}&" +
         "oauth_timestamp={4}&" +
         "oauth_version={5}&" +
         "{6}&{7}",
         cFormat,
         cConsumerKey,
         lNonce,
         cOAuthSignMethod,
         lTimes,
         cOAuthVersion,
         cUnitID,
         cWeatherID
        );

        lSign = string.Concat(
         "GET&", Uri.EscapeDataString(cURL), "&", Uri.EscapeDataString(lSign)
        );

        using (var lHasher = new HMACSHA1(Encoding.ASCII.GetBytes(lCKey)))
        {
            lSign = Convert.ToBase64String(
             lHasher.ComputeHash(Encoding.ASCII.GetBytes(lSign))
            );
        }  // end using

        return "OAuth " +
               "oauth_consumer_key=\"" + cConsumerKey + "\", " +
               "oauth_nonce=\"" + lNonce + "\", " +
               "oauth_timestamp=\"" + lTimes + "\", " +
               "oauth_signature_method=\"" + cOAuthSignMethod + "\", " +
               "oauth_signature=\"" + lSign + "\", " +
               "oauth_version=\"" + cOAuthVersion + "\"";

    }  // end _get_auth

    public string GetWeather()
    {

        const string lURL = cURL + "?" + cWeatherID + "&" + cUnitID + "&format=" + cFormat;

        var lClt = new WebClient();

        lClt.Headers.Set("Content-Type", "application/" + cFormat);
        lClt.Headers.Add("X-Yahoo-App-Id", cAppID);
        lClt.Headers.Add("Authorization", _get_auth());

        Console.WriteLine("Downloading Yahoo weather report . . .");

        byte[] lDataBuffer = lClt.DownloadData(lURL);

        string lOut = Encoding.ASCII.GetString(lDataBuffer);

        Console.WriteLine(lOut);

        //Console.Write("Press any key to continue . . . ");
        //Console.ReadKey(true);

        return lOut;

    }  // end Main

}  // end YWSample
