using NuGet.Protocol.Plugins;
using Twilio.Rest.Api.V2010.Account;


namespace Company.G03.PL.Helpers.SMS
    {
    public interface ITwilioService
        {
        public MessageResource SendMessage(Sms sms);

        }
    }
